using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HttpSurge.UI.Models;
using HttpSurge.UI.ViewModels;

namespace HttpSurge.UI.Views;

public partial class RequestTreeView : UserControl
{
    public RequestTreeView()
    {
        InitializeComponent();
    }

    private void RenameTextBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox { DataContext: TreeItem item } && DataContext is RequestTreeViewModel vm)
        {
            if (!item.IsRenaming) return;
            item.IsRenaming = false;
            vm.SaveChangesCommand.Execute(item);
        }
    }

    private void RenameTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (sender is not TextBox { DataContext: TreeItem item } || DataContext is not RequestTreeViewModel vm) return;

        if (!item.IsRenaming) return;
        item.IsRenaming = false;
        vm.SaveChangesCommand.Execute(item);
    }
}
