using MacrixT.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MacrixT.Views
{
    public partial class PeopleView
    {
        public PeopleView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<PeopleViewModel>();
        }
    }
}