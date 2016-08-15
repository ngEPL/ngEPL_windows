using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NewEPL {
    public static class ListboxExtensions {
        public static DependencyProperty IgnoreScrollProperty = DependencyProperty.RegisterAttached("IgnoreScroll", typeof(bool), typeof(ListboxExtensions), new UIPropertyMetadata(false, IgnoreScrollChanged));

        public static bool GetIgnoreScroll(DependencyObject dependencyObject) {
            return (bool)dependencyObject.GetValue(IgnoreScrollProperty);
        }

        public static void SetIgnoreScroll(DependencyObject dependencyObject, bool value) {
            dependencyObject.SetValue(IgnoreScrollProperty, value);
        }

        private static void IgnoreScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;

            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null) return;

            if (!newValue || oldValue || frameworkElement.IsFocused) return;

            var lb = frameworkElement as ListBox;
            if (lb == null) return;

            lb.PreviewMouseWheel += LbOnPreviewMouseWheel;
        }

        private static void LbOnPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            if (!(sender is ListBox) || e.Handled) return;

            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };

            var parent = ((Control)sender).Parent as UIElement;
            if (parent != null) parent.RaiseEvent(eventArg);
        }
    }
}
