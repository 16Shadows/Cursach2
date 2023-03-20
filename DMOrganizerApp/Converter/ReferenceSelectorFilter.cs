using DMOrganizerModel.Interface.Items;
using DMOrganizerViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DMOrganizerApp.Converter
{
    internal class ReferenceSelectorFilter : IValueConverter
    {
        private static IEnumerable<ItemViewModel> Filter(IEnumerable<ItemViewModel> items)
        {
            foreach (ItemViewModel item in items)
                if (item is not BookViewModel)
                    yield return item;
            yield break;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            IEnumerable<ItemViewModel> items = (IEnumerable<ItemViewModel>)value;
            return Filter(items);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
