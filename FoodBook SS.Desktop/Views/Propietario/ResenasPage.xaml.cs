using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Propietario;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class ResenasPage : Page
    {
        public RelayCommand SelectCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ResenasPage(IResenaService svc)
        {
            InitializeComponent();
            var vm = new ResenasViewModel(svc);
            DataContext = vm;
            
            SelectCommand = new RelayCommand(obj => {
                if (obj is ReviewDto review) vm.Selected = review;
            });
            
            CancelCommand = new RelayCommand(_ => {
                vm.Selected = null;
                vm.RespuestaText = "";
            });

            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
