using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Demo.Protocol;

namespace Demo.Client
{



    public class StepStatusConvertStatusToColor : IValueConverter
    {

        public Color[] statusColors = new Color[] { Colors.LawnGreen, Colors.Red, Colors.LightBlue, Colors.LightGray };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Step.StatusTypes status = (Step.StatusTypes)value;

            Color farge = Colors.White;
            //Switchen til erik her, setter farge til riktig farge utifra status.

            switch (status)
            {
                case Step.StatusTypes.PASSED:
                    farge = statusColors[(int)Step.StatusTypes.PASSED]; //LawnGreen 0
                    break;
                case Step.StatusTypes.FAILED:
                    farge = statusColors[(int)Step.StatusTypes.FAILED]; //Red 1
                    break;
                case Step.StatusTypes.DONE:
                    farge = statusColors[(int)Step.StatusTypes.DONE]; //LightBlue 2
                    break;
                case Step.StatusTypes.SKIPPED:
                    farge = statusColors[(int)Step.StatusTypes.SKIPPED]; //LightGray 3
                    break;

            }

            return new SolidColorBrush(farge);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
