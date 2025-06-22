using CommunityToolkit.Mvvm.ComponentModel;

namespace HttpSurge.UI.ViewModels;

public partial class PerformanceTestDetailViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "性能测试过程";
}
