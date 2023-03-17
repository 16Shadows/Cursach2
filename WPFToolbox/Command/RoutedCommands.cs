using System.Windows.Input;

namespace WPFToolbox.Command
{
    public static class RoutedCommands
    {
        public static readonly RoutedCommand CommandNext = new ("Next", 
                                                                typeof(RoutedCommands),
                                                                new InputGestureCollection() { new KeyGesture(Key.Enter) });
        
        public static readonly RoutedCommand CommandEscape = new ("Escape", 
                                                                typeof(RoutedCommands),
                                                                new InputGestureCollection() { new KeyGesture(Key.Escape) });


    }
}
