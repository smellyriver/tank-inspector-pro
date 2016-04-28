using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public static class Progress
    {
        public static IProgressScope GetScope(DependencyObject obj)
        {
            return (IProgressScope)obj.GetValue(ScopeProperty);
        }

        public static void SetScope(DependencyObject obj, IProgressScope value)
        {
            obj.SetValue(ScopeProperty, value);
        }

        public static readonly DependencyProperty ScopeProperty =
            DependencyProperty.RegisterAttached("Scope", typeof(IProgressScope), typeof(Progress), new PropertyMetadata(null, Progress.OnScopeChanged));

        private static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressBar = d as ProgressBar;
            if (progressBar == null)
                return;

            var scope = e.NewValue as IProgressScope;

            if (scope == null)
            {
                BindingOperations.ClearBinding(progressBar, RangeBase.ValueProperty);
                BindingOperations.ClearBinding(progressBar, ProgressBar.IsIndeterminateProperty);
            }
            else
            {
                progressBar.Maximum = 1.0;
                progressBar.Minimum = 0.0;

                var valueBinding = new Binding("Progress");
                valueBinding.Source = scope;
                BindingOperations.SetBinding(progressBar, RangeBase.ValueProperty, valueBinding);

                var isIndeterminateBinding = new Binding("IsIndeterminate");
                isIndeterminateBinding.Source = scope;
                BindingOperations.SetBinding(progressBar, ProgressBar.IsIndeterminateProperty, isIndeterminateBinding);
            }
        }

    }
}
