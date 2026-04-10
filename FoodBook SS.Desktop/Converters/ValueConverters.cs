using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace FoodBook_SS.Desktop.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public static readonly StringToVisibilityConverter Instance = new();
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is string s && !string.IsNullOrEmpty(s) ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter Instance = new();
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is true ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public static readonly InverseBoolToVisibilityConverter Instance = new();
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is false ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
    public class EstadoColorConverter : IValueConverter
    {
        public static readonly EstadoColorConverter Instance = new();
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            return (value?.ToString()?.ToLower()) switch
            {
                "confirmada" or "completada" or "entregada" or "aprobado" => "#22C55E",
                "pendiente"                                                => "#EAB308",
                "cancelada" or "rechazado" or "noshow"                    => "#EF4444",
                _                                                          => "#94A3B8"
            };
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
    public class BoolToTextConverter : IValueConverter
    {
        public static readonly BoolToTextConverter Instance = new();
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is true ? "Ocultar" : "Mostrar";
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
    public class NullToVisibilityConverter : IValueConverter
    {
        public static readonly NullToVisibilityConverter Instance = new();
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            bool isNull = value == null || (value is string s && string.IsNullOrEmpty(s));
            bool inverted = p?.ToString() == "Inverted";
            bool visible = inverted ? !isNull : isNull; 
            bool show = (inverted) ? isNull : !isNull;
            return show ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}

