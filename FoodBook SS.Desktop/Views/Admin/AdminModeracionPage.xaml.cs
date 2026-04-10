using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Admin;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Admin
{
    public partial class AdminModeracionPage : Page
    {
        public AdminModeracionPage(IResenaService svc)
        {
            InitializeComponent();
            var vm = new AdminModeracionViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
