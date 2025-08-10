using CommunityToolkit.Mvvm.ComponentModel;

namespace HttpSurge.UI.ViewModels;

public partial class HistoryRecordsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "历史记录";

}