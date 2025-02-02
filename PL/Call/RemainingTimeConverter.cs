using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PL.Call
{
    public class RemainingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan < TimeSpan.Zero)
                {
                    return "Call terminé";
                }

                return $"{timeSpan.Days} jours, {timeSpan.Hours} heures, {timeSpan.Minutes} minutes";
            }
            return "Non défini";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack n'est pas implémenté.");
        }
    }
}
