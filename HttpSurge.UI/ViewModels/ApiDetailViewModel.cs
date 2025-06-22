using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HttpSurge.UI.Models;

namespace HttpSurge.UI.ViewModels;

public partial class ApiDetailViewModel : ViewModelBase
{
    private static readonly HttpClient HttpClient = new();

    [ObservableProperty]
    private Api _api;

    [ObservableProperty]
    private string _responseContent = string.Empty;

    [ObservableProperty]
    private int _statusCode;

    public ApiDetailViewModel(Api api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task SendRequest()
    {
        if (Api.Url == null)
        {
            return;
        }

        try
        {
            var request = new HttpRequestMessage(new HttpMethod(Api.Method ?? "GET"), Api.Url);
            var response = await HttpClient.SendAsync(request);
            ResponseContent = await response.Content.ReadAsStringAsync();
            StatusCode = (int)response.StatusCode;
        }
        catch (HttpRequestException e)
        {
            ResponseContent = e.Message;
            StatusCode = 0;
        }
    }
}

