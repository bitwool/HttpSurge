using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HttpSurge.UI.Models;

public class QueryParam : ObservableObject
{
    public int Id { get; set; }
    private string _key = "";
    public string Key
    {
        get => _key;
        set => SetProperty(ref _key, value);
    }

    private string _value = "";
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
    public int ApiId { get; set; }
    [JsonIgnore]
    public Api? Api { get; set; }
}