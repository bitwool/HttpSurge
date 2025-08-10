using Avalonia;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HttpSurge.UI.Data;
using HttpSurge.UI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;

namespace HttpSurge.UI.ViewModels;

public partial class ApiTreeViewModel : ViewModelBase
{
    private readonly AppDbContext? _dbContext;

    [ObservableProperty]
    private ObservableCollection<TreeItem> _observableItems = new();

    [ObservableProperty]
    private TreeItem? _selectedItem;

    public ApiTreeViewModel()
    {
        if (Design.IsDesignMode)
        {
            _observableItems = new ObservableCollection<TreeItem>
            {
                new Collection
                {
                    Name = "Sample Collection 1",
                    Children =
                    {
                        new Folder
                        {
                            Name = "Subfolder 1",
                            Children = { new Api { Name = "Sample GET API", Method = "GET" } }
                        }
                    }
                },
                new Collection
                {
                    Name = "Sample Collection 2",
                    Children = { new Api { Name = "Another API" } }
                }
            };
        }
    }

    public ApiTreeViewModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();

        _dbContext.TreeItems.Load();
        _dbContext.Apis
            .Include(a => a.Headers)
            .Include(a => a.QueryParams)
            .Load();


        var allItems = _dbContext.TreeItems.Local.ToList();
        foreach (var item in allItems)
        {
            if (item is Api api)
            {
                SubscribeToApiChanges(api);
            }
        }

        var collectionItems = allItems.Where(i => i.ParentId == null).ToList();

        foreach (var item in collectionItems)
        {
            LoadChildren(item, allItems);
        }

        ObservableItems = new ObservableCollection<TreeItem>(collectionItems);

        if (!ObservableItems.Any())
        {
            // Add sample data if database is empty
            var collection1 = new Collection { Name = "Collection 1" };
            var folder1 = new Folder { Name = "Subfolder 1" };
            var api1 = new Api { Name = "Get Users", Method = "GET", Url = "https://reqres.in/api/users" };
            SubscribeToApiChanges(api1);
            folder1.Children.Add(api1);
            collection1.Children.Add(folder1);

            var collection2 = new Collection { Name = "Collection 2" };
            var api2 = new Api { Name = "Get Single User", Method = "GET", Url = "https://reqres.in/api/users/2" };
            SubscribeToApiChanges(api2);
            collection2.Children.Add(api2);

            _dbContext.TreeItems.Add(collection1);
            _dbContext.TreeItems.Add(collection2);
            _dbContext.SaveChanges();

            allItems = _dbContext.TreeItems.ToList();
            collectionItems = allItems.Where(i => i.ParentId == null).ToList();
            foreach (var item in collectionItems)
            {
                LoadChildren(item, allItems);
            }
            ObservableItems = new ObservableCollection<TreeItem>(collectionItems);
        }
    }

    private void SubscribeToApiChanges(Api api)
    {
        api.PropertyChanged += (s, e) => SaveChanges(api);

        if (api.Headers is INotifyCollectionChanged headers)
        {
            headers.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.NewItems)
                    {
                        item.PropertyChanged += (s, e) => SaveChanges(api);
                    }
                }
                SaveChanges(api);
            };
        }

        if (api.QueryParams is INotifyCollectionChanged queryParams)
        {
            queryParams.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.NewItems)
                    {
                        item.PropertyChanged += (s, e) => SaveChanges(api);
                    }
                }
                SaveChanges(api);
            };
        }

        foreach (var header in api.Headers)
        {
            (header as INotifyPropertyChanged).PropertyChanged += (s, e) => SaveChanges(api);
        }

        foreach (var queryParam in api.QueryParams)
        {
            (queryParam as INotifyPropertyChanged).PropertyChanged += (s, e) => SaveChanges(api);
        }
    }

    partial void OnSelectedItemChanged(TreeItem? value)
    {
        if (value is Collection or Folder)
        {
            value.IsExpanded = !value.IsExpanded;
        }
    }

    [RelayCommand]
    private void Rename(TreeItem? item)
    {
        if (item is not null)
        {
            item.IsRenaming = true;
        }
    }

    [RelayCommand]
    private void AddCollection()
    {
        if (_dbContext is null) return;
        var newCollection = new Collection { Name = "New Collection" };
        _dbContext.Collections.Add(newCollection);
        _dbContext.SaveChanges();
        ObservableItems.Add(newCollection);
    }

    [RelayCommand]
    private void AddSubfolder(TreeItem? parent)
    {
        if (parent is not (Collection or Folder)) return;
        if (_dbContext is null) return;

        var newFolder = new Folder { Name = "New Folder", ParentId = parent.Id };
        _dbContext.Folders.Add(newFolder);
        _dbContext.SaveChanges();
        // parent.Children.Add(newFolder);
    }

    [RelayCommand]
    private void AddApi(TreeItem? parent)
    {
        if (parent is not (Collection or Folder)) return;
        if (_dbContext is null) return;

        var newApi = new Api { Name = "New Api", ParentId = parent.Id };
        SubscribeToApiChanges(newApi);
        _dbContext.Apis.Add(newApi);
        _dbContext.SaveChanges();
        // parent.Children.Add(newApi);
    }

    [RelayCommand]
    private void DeleteItem(TreeItem? item)
    {
        if (item == null) return;
        if (_dbContext is null) return;

        var parent = item.Parent;
        if (parent != null)
        {
            parent.Children.Remove(item);
        }
        else
        {
            ObservableItems.Remove(item);
        }

        DeleteItemAndChildren(item);
        _dbContext.SaveChanges();

        if (SelectedItem == item)
        {
            SelectedItem = null;
        }
    }

    private void DeleteItemAndChildren(TreeItem item)
    {
        if (_dbContext is null) return;
        foreach (var child in item.Children.ToList())
        {
            DeleteItemAndChildren(child);
        }

        _dbContext.TreeItems.Remove(item);
    }

    [RelayCommand]
    private void SaveChanges(TreeItem? item)
    {
        if (item == null) return;
        if (_dbContext is null) return;
        _dbContext.Entry(item).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }

    private void LoadChildren(TreeItem parent, List<TreeItem> allItems)
    {
        var children = allItems.Where(i => i.ParentId == parent.Id).ToList();
        parent.Children = new ObservableCollection<TreeItem>(children);
        foreach (var child in children)
        {
            child.Parent = parent;
            LoadChildren(child, allItems);
        }
    }
}
