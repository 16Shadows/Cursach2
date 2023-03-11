using System.Windows;

namespace WPFToolbox.Condition
{
    /// <summary>
    /// Implementation of equality operator using dependency system. Supports comparison via bindings
    /// </summary>
    public class ConditionEquals : DependencyObject, ICondition
    {
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(object), typeof(ConditionEquals));
        public static readonly DependencyProperty RightProperty = DependencyProperty.Register(nameof(Right), typeof(object), typeof(ConditionEquals));

        public object Left
        {
            get => GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public object Right
        {
            get => GetValue(RightProperty);
            set => SetValue(RightProperty, value);
        }

        public bool Evaluate()
        {
            return Left?.Equals(Right) ?? false;
        }
    }
}
