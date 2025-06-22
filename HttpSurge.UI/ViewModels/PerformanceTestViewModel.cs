using CommunityToolkit.Mvvm.ComponentModel;

namespace HttpSurge.UI.ViewModels;

public partial class PerformanceTestViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "性能测试计划";
}
