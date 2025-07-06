using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HttpSurge.UI.Models;
using HttpSurge.UI.ViewModels;

namespace HttpSurge.UI.Views;

public partial class ApiTreeView : UserControl
{
    public ApiTreeView()
    {
        InitializeComponent();
    }

    private void RenameTextBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox { DataContext: TreeItem item } && DataContext is ApiTreeViewModel vm)
        {
            if (!item.IsRenaming) return;
            item.IsRenaming = false;
            vm.SaveChangesCommand.Execute(item);
        }
    }

    private void RenameTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (sender is not TextBox { DataContext: TreeItem item } || DataContext is not ApiTreeViewModel vm) return;

        if (!item.IsRenaming) return;
        item.IsRenaming = false;
        vm.SaveChangesCommand.Execute(item);
    }
}
