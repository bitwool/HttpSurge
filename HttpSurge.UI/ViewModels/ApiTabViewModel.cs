using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HttpSurge.UI.Models;
using Avalonia;
using Avalonia.Controls;

namespace HttpSurge.UI.ViewModels;

public partial class ApiTabViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<ApiDetailViewModel> _openApiTabs = new();

    [ObservableProperty] private ApiDetailViewModel? _selectedApiTab;

    public ApiTabViewModel()
    {
        if (Design.IsDesignMode)
        {
            _openApiTabs = new ObservableCollection<ApiDetailViewModel>
            {
                new(new Api { Name = "Sample API" })
            };
            _selectedApiTab = _openApiTabs.First();
        }
    }

    public void OpenApi(Api api)
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

    [RelayCommand]
    private void CloseTab(ApiDetailViewModel? tab)
    {
        if (tab == null) return;
        OpenApiTabs.Remove(tab);
    }
}
