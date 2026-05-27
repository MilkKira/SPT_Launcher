/* StringIsNullOrEmptyToBoolConverter.cs
 * License: NCSA Open Source License
 *
 * Copyright: SPT
 * AUTHORS:
 */

using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SPT.Launcher.Converters
{
    public class StringIsNullOrEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}