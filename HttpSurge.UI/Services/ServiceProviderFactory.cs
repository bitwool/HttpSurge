using HttpSurge.UI.Data;
using HttpSurge.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HttpSurge.UI.Services;

public static class ServiceProviderFactory
{
    public static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=httpsurge.db"));

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<RequestTreeViewModel>();
        services.AddSingleton<ApiTabViewModel>();

        return services.BuildServiceProvider();
    }
}
