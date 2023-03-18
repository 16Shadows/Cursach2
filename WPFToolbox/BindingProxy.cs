using System.Windows;

namespace WPFToolbox
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register(nameof(Context), typeof(object), typeof(BindingProxy));

        public object Context
        {
            get => GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}