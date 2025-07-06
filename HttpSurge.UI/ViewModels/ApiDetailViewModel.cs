using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HttpSurge.UI.Models;

namespace HttpSurge.UI.ViewModels;

public partial class ApiDetailViewModel : ViewModelBase
{
    public static List<string> HttpMethods { get; } = new() { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };

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
            var url = Api.Url;
            if (Api.QueryParams.Any())
            {
                var query = string.Join("&", Api.QueryParams.Select(p => $"{p.Key}={p.Value}"));
                url += $"?{query}";
            }

            var request = new HttpRequestMessage(new HttpMethod(Api.Method ?? "GET"), url);

            foreach (var header in Api.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(Api.Body))
            {
                request.Content = new StringContent(Api.Body, Encoding.UTF8, "application/json");
            }

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

    [RelayCommand]
    private void AddQueryParam()
    {
        Api.QueryParams.Add(new QueryParam());
    }

    [RelayCommand]
    private void RemoveQueryParam(QueryParam? param)
    {
        if (param is null) return;
        Api.QueryParams.Remove(param);
    }

    [RelayCommand]
    private void AddHeader()
    {
        Api.Headers.Add(new Header());
    }

    [RelayCommand]
    private void RemoveHeader(Header? header)
    {
        if (header is null) return;
        Api.Headers.Remove(header);
    }
}
