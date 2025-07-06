using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HttpSurge.UI.Models;





public class Api : TreeItem
{
    public string? Url { get; set; }
    public string? Method { get; set; }
    public string? Body { get; set; }

    public virtual ICollection<Header> Headers { get; set; } = new ObservableCollection<Header>();
    public virtual ICollection<QueryParam> QueryParams { get; set; } = new ObservableCollection<QueryParam>();
}
