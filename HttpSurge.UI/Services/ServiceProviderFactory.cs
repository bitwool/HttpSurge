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
            options.UseSqlite("Data Source=http-surge.db"));

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ApiTreeViewModel>();
        services.AddSingleton<ApiTabViewModel>();
        services.AddTransient<VariableManagementViewModel>();
        services.AddTransient<PerformanceTestViewModel>();
        services.AddTransient<PerformanceTestDetailViewModel>();
        services.AddTransient<HistoryRecordsViewModel>();

        return services.BuildServiceProvider();
    }
}
