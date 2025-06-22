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
    [ObservableProperty]
    private ViewModelBase? _column2Content;

    [ObservableProperty]
    private ViewModelBase? _column3Content;

    public RequestTreeViewModel RequestTreeVm { get; }
    public ApiTabViewModel ApiTabVm { get; }
    public VariableManagementViewModel VariableManagementVm { get; }
    public PerformanceTestViewModel PerformanceTestVm { get; }
    public PerformanceTestDetailViewModel PerformanceTestDetailVm { get; }

    // This constructor is used by the XAML designer
    public MainWindowViewModel()
    {
        RequestTreeVm = new RequestTreeViewModel();
        ApiTabVm = new ApiTabViewModel();
        VariableManagementVm = new VariableManagementViewModel();
        PerformanceTestVm = new PerformanceTestViewModel();
        PerformanceTestDetailVm = new PerformanceTestDetailViewModel();
        Initialize();
    }

    // 主要构造函数，同时接收所有依赖
    public MainWindowViewModel(RequestTreeViewModel requestTreeVm, ApiTabViewModel apiTabVm,
        VariableManagementViewModel variableManagementVm, PerformanceTestViewModel performanceTestVm,
        PerformanceTestDetailViewModel performanceTestDetailVm)
    {
        RequestTreeVm = requestTreeVm;
        ApiTabVm = apiTabVm;
        VariableManagementVm = variableManagementVm;
        PerformanceTestVm = performanceTestVm;
        PerformanceTestDetailVm = performanceTestDetailVm;
        Initialize();
    }

    private void Initialize()
    {
        // 设置树形视图和标签页视图之间的通信
        RequestTreeVm.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(RequestTreeViewModel.SelectedItem) && RequestTreeVm.SelectedItem is Api api)
            {
                ApiTabVm.OpenApi(api);
            }
        };

        // Default view
        ShowApiManagement();
    }

    [RelayCommand]
    private void ShowApiManagement()
    {
        Column2Content = RequestTreeVm;
        Column3Content = ApiTabVm;
    }

    [RelayCommand]
    private void ShowVariableManagement()
    {
        Column2Content = VariableManagementVm;
        Column3Content = ApiTabVm; // Or another view model if needed
    }

    [RelayCommand]
    private void ShowPerformanceTest()
    {
        Column2Content = PerformanceTestVm;
        Column3Content = PerformanceTestDetailVm;
    }
}