using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HttpSurge.UI.Models;

public abstract partial class TreeItem : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty] private string _name = "";

    public TreeItem? Parent { get; set; }
    public int? ParentId { get; set; }
    public ObservableCollection<TreeItem> Children { get; set; } = new();

    [ObservableProperty] private bool _isExpanded;

    [ObservableProperty] private bool _isRenaming;
}
