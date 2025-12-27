using PersonalFinanceManager.Enum;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PersonalFinanceManager.Converters;

public class AmountToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TransactionType transactionType)
        {
            return transactionType == TransactionType.Доход
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))   
                : new SolidColorBrush(Color.FromRgb(244, 67, 54));  
        }

        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}