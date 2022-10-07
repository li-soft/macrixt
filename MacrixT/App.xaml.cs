using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MacrixT
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public App()
        {
            Services = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build()
                .Services;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Services.GetRequiredService<MainWindow>().Show();
            base.OnStartup(e);
        }

        private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            foreach (var dependencyCandidate in GetDependencyCandidates())
            {
                if (dependencyCandidate.Interface == null 
                    || IsWindowType(dependencyCandidate.Implementation)
                    || IsViewModelType(dependencyCandidate.Implementation))
                {
                    services.AddTransient(dependencyCandidate.Implementation);
                }
                else
                {
                    services.AddTransient(dependencyCandidate.Interface, dependencyCandidate.Implementation);
                }
            }
        }

        private static bool IsViewModelType(Type type) => typeof(ObservableRecipient).IsAssignableFrom(type);

        private static bool IsWindowType(Type type) => typeof(MetroWindow).IsAssignableFrom(type);

        private static IEnumerable<DependencyCandidate> GetDependencyCandidates()
        {
            return typeof(App).Assembly
                .GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .SelectMany(t =>
                {
                    var interfaces = t.GetInterfaces();
                    return interfaces?.Length < 1 
                        ? new[] { new DependencyCandidate { Implementation = t } } 
                        : interfaces!.Select(i => new DependencyCandidate { Interface = i, Implementation = t });
                });
        }

        private class DependencyCandidate
        {
            public Type? Interface { get; init; }
            public Type Implementation { get; init; } = null!;
        }
    }
}