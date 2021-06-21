﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WolvenManager.App.ViewModels;

namespace WolvenManager.UI.Converters
{
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ArchiveViewModel mod)
            {
                //var input = mod.Enabled;

                //return input 
                //    ? new SolidColorBrush(Colors.DarkSeaGreen) 
                //    : DependencyProperty.UnsetValue;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
