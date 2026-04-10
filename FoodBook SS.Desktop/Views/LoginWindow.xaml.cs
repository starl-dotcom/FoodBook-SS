using FoodBook_SS.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
namespace FoodBook_SS.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;
        public LoginWindow(LoginViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;
        }
        private void PwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
            => _vm.Password = PwdPassword.Password;
    }
}

