using System.Windows;
using System.Windows.Media;

namespace WPFToolbox
{
    internal static class WPFHelper
    {
        public static T? GetVisualParent<T>(this DependencyObject item) where T : DependencyObject
        {
            item = VisualTreeHelper.GetParent(item);
            while (item != null && item is not T)
                item = VisualTreeHelper.GetParent(item);
            return (T?)item;
        }

        public static T? GetParent<T>(this FrameworkElement item) where T : FrameworkElement
        {
            item = item.Parent as FrameworkElement;
            while (item != null && item is not T)
                item = item.Parent as FrameworkElement;
            return (T?)item;
        }
    }
}
