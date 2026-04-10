using FoodBook_SS.Desktop.ViewModels;
using System.Windows;

namespace FoodBook_SS.Desktop.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _vm;
        public RegisterWindow(RegisterViewModel vm)
        {
            InitializeComponent(); _vm = vm; DataContext = _vm;
        }
        private void PwdPass_Changed(object s, RoutedEventArgs e)    => _vm.Password  = PwdPass.Password;
        private void PwdConfirm_Changed(object s, RoutedEventArgs e) => _vm.Confirmar = PwdConfirm.Password;
    }
}
