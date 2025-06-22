using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HttpSurge.UI.Data;
using HttpSurge.UI.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpSurge.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AppDbContext? _dbContext;

    [ObservableProperty]
    private ObservableCollection<TreeItem> _items;

    [ObservableProperty]
    private TreeItem? _selectedItem;

    [ObservableProperty] private ObservableCollection<ApiDetailViewModel> _openApiTabs = new();

    [ObservableProperty] private ApiDetailViewModel? _selectedApiTab;

    public RequestTreeViewModel RequestTreeVm { get; }
    public ApiTabViewModel ApiTabVm { get; }

    // This constructor is used by the XAML designer
    public MainWindowViewModel()
    {
        RequestTreeVm = new RequestTreeViewModel();
        ApiTabVm = new ApiTabViewModel();
    }

    // 主要构造函数，同时接收所有依赖
    public MainWindowViewModel(AppDbContext dbContext, RequestTreeViewModel requestTreeVm, ApiTabViewModel apiTabVm)
    {
        _dbContext = dbContext;
        RequestTreeVm = requestTreeVm;
        ApiTabVm = apiTabVm;

        _dbContext.Database.EnsureCreated();

        var allItems = _dbContext.TreeItems.ToList();
        var rootItems = allItems.Where(i => i.ParentId == null).ToList();

        foreach (var item in rootItems)
        {
            LoadChildren(item, allItems);
        }

        Items = new ObservableCollection<TreeItem>(rootItems);

        // 设置树形视图和标签页视图之间的通信
        RequestTreeVm.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(RequestTreeViewModel.SelectedItem) && RequestTreeVm.SelectedItem is Api api)
            {
                ApiTabVm.OpenApi(api);
            }
        };

        if (!Items.Any())
        {
            // Add sample data if database is empty
            var collection1 = new Collection { Name = "Collection 1" };
            var folder1 = new Folder { Name = "Subfolder 1" };
            var api1 = new Api { Name = "Get Users", Method = "GET", Url = "https://reqres.in/api/users" };
            folder1.Children.Add(api1);
            collection1.Children.Add(folder1);

            var collection2 = new Collection { Name = "Collection 2" };
            var api2 = new Api { Name = "Get Single User", Method = "GET", Url = "https://reqres.in/api/users/2" };
            collection2.Children.Add(api2);

            _dbContext.TreeItems.Add(collection1);
            _dbContext.TreeItems.Add(collection2);
            _dbContext.SaveChanges();

            allItems = _dbContext.TreeItems.ToList();
            rootItems = allItems.Where(i => i.ParentId == null).ToList();
            foreach (var item in rootItems)
            {
                LoadChildren(item, allItems);
            }
            Items = new ObservableCollection<TreeItem>(rootItems);
        }

        // 初始化 RequestTreeVm 和 ApiTabVm
        if (requestTreeVm.Items == null || !requestTreeVm.Items.Any())
        {
            requestTreeVm.Items = Items;
        }
    }

    partial void OnSelectedItemChanged(TreeItem? value)
    {
        if (value is Collection or Folder)
        {
            value.IsExpanded = !value.IsExpanded;
        }
        else if (value is Api api)
        {
            var existingTab = OpenApiTabs.FirstOrDefault(t => t.Api.Id == api.Id);
            if (existingTab == null)
            {
                var newTab = new ApiDetailViewModel(api);
                OpenApiTabs.Add(newTab);
                SelectedApiTab = newTab;
            }
            else
            {
                SelectedApiTab = existingTab;
            }
        }
    }

    [RelayCommand]
    private void CloseTab(ApiDetailViewModel? tab)
    {
        if (tab == null) return;
        OpenApiTabs.Remove(tab);
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
        var newCollection = new Collection { Name = "New Collection" };
        Items.Add(newCollection);
        _dbContext?.TreeItems.Add(newCollection);
        _dbContext?.SaveChanges();
    }

    [RelayCommand]
    private void AddSubfolder(TreeItem? parent)
    {
        if (parent is not (Collection or Folder)) return;

        var newFolder = new Folder { Name = "New Folder" };
        newFolder.Parent = parent;
        newFolder.ParentId = parent.Id;
        parent.Children.Add(newFolder);
        _dbContext?.TreeItems.Add(newFolder);
        _dbContext?.SaveChanges();
    }

    [RelayCommand]
    private void AddApi(TreeItem? parent)
    {
        if (parent is not (Collection or Folder)) return;

        var newApi = new Api { Name = "New Api", Method = "GET" };
        newApi.Parent = parent;
        newApi.ParentId = parent.Id;
        parent.Children.Add(newApi);
        _dbContext?.TreeItems.Add(newApi);
        _dbContext?.SaveChanges();
    }

    [RelayCommand]
    private void DeleteItem(TreeItem? item)
    {
        if (item == null) return;

        var parent = item.Parent;

        // Recursively remove from DB
        DeleteItemAndChildren(item);
        _dbContext?.SaveChanges();

        // Remove from UI
        if (parent != null)
        {
            parent.Children.Remove(item);
        }
        else
        {
            Items.Remove(item);
        }

        if (SelectedItem == item)
        {
            SelectedItem = null;
        }
    }

    private void DeleteItemAndChildren(TreeItem item)
    {
        foreach (var child in item.Children.ToList())
        {
            DeleteItemAndChildren(child);
        }
        _dbContext?.TreeItems.Remove(item);
    }

    [RelayCommand]
    private void SaveChanges(TreeItem? item)
    {
        if (item != null)
        {
            _dbContext?.Update(item);
            _dbContext?.SaveChanges();
        }
    }

    private void LoadChildren(TreeItem parent, List<TreeItem> allItems)
    {
        parent.Children = new ObservableCollection<TreeItem>(allItems.Where(i => i.ParentId == parent.Id));
        foreach (var child in parent.Children)
        {
            child.Parent = parent; // Set parent for UI logic
            LoadChildren(child, allItems);
        }
    }
}