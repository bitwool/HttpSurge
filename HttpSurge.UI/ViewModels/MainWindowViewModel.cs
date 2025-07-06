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
    [ObservableProperty] private ViewModelBase? _leftPanelContent;

    [ObservableProperty] private ViewModelBase? _rightPanelContent;

    public ApiTreeViewModel ApiTreeVm { get; }
    public ApiTabViewModel ApiTabVm { get; }
    public VariableManagementViewModel VariableManagementVm { get; }
    public PerformanceTestViewModel PerformanceTestVm { get; }
    public PerformanceTestDetailViewModel PerformanceTestDetailVm { get; }

    // This constructor is used by the XAML designer
    public MainWindowViewModel()
    {
        ApiTreeVm = new ApiTreeViewModel();
        ApiTabVm = new ApiTabViewModel();
        VariableManagementVm = new VariableManagementViewModel();
        PerformanceTestVm = new PerformanceTestViewModel();
        PerformanceTestDetailVm = new PerformanceTestDetailViewModel();
        Initialize();
    }

    // 主要构造函数，同时接收所有依赖
    public MainWindowViewModel(
        ApiTreeViewModel apiTreeVm,
        ApiTabViewModel apiTabVm,
        VariableManagementViewModel variableManagementVm,
        PerformanceTestViewModel performanceTestVm,
        PerformanceTestDetailViewModel performanceTestDetailVm)
    {
        ApiTreeVm = apiTreeVm;
        ApiTabVm = apiTabVm;
        VariableManagementVm = variableManagementVm;
        PerformanceTestVm = performanceTestVm;
        PerformanceTestDetailVm = performanceTestDetailVm;
        Initialize();
    }

    private void Initialize()
    {
        ApiTreeVm.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(ApiTreeViewModel.SelectedItem) && ApiTreeVm.SelectedItem is Api api)
            {
                ApiTabVm.OpenApi(api);
            }
        };

        ShowApiManagement();
    }

    [RelayCommand]
    private void ShowApiManagement()
    {
        LeftPanelContent = ApiTreeVm;
        RightPanelContent = ApiTabVm;
    }

    [RelayCommand]
    private void ShowVariableManagement()
    {
        LeftPanelContent = VariableManagementVm;
    }

    [RelayCommand]
    private void ShowPerformanceTest()
    {
        LeftPanelContent = PerformanceTestVm;
        RightPanelContent = PerformanceTestDetailVm;
    }
}