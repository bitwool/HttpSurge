using CommunityToolkit.Mvvm.ComponentModel;

namespace HttpSurge.UI.ViewModels;

public partial class VariableManagementViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "变量管理";
}
