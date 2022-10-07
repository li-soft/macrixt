using MacrixT.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MacrixT
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainViewModel>();
        }
    }
}