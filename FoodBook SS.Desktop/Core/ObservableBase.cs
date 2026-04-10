using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace FoodBook_SS.Desktop.Core
{
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { SetField(ref _isBusy, value); OnPropertyChanged(nameof(IsNotBusy)); }
        }
        public bool IsNotBusy => !_isBusy;
        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }
        private string? _successMessage;
        public string? SuccessMessage
        {
            get => _successMessage;
            set => SetField(ref _successMessage, value);
        }
        protected void SetError(string msg)   { ErrorMessage = msg; SuccessMessage = null; }
        protected void SetSuccess(string msg) { SuccessMessage = msg; ErrorMessage = null; }
        protected void ClearMessages()        { ErrorMessage = null; SuccessMessage = null; }
    }
}

