using System;
using System.Windows;

namespace Smellyriver.TankInspector.Common.Wpf.Effects
{
    public class ProgressiveBrightnessEffect : BrightContrastEffect
    {
        public double HighlightBrightness
        {
            get { return (double)GetValue(HighlightBrightnessProperty); }
            set { SetValue(HighlightBrightnessProperty, value); }
        }

        public static readonly DependencyProperty HighlightBrightnessProperty =
            DependencyProperty.Register("HighlightBrightness", typeof(double), typeof(ProgressiveBrightnessEffect), new PropertyMetadata(0.1));

        public double ShadowBrightness
        {
            get { return (double)GetValue(ShadowBrightnessProperty); }
            set { SetValue(ShadowBrightnessProperty, value); }
        }

        public static readonly DependencyProperty ShadowBrightnessProperty =
            DependencyProperty.Register("ShadowBrightness", typeof(double), typeof(ProgressiveBrightnessEffect), new PropertyMetadata(-0.1));

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressiveBrightnessEffect), new PropertyMetadata(0.0, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressiveBrightnessEffect)d).OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            if (this.Progress < 0)
                this.Brightness = Math.Abs(this.Progress) * this.ShadowBrightness;
            else
                this.Brightness = this.Progress * this.HighlightBrightness;
        }

    }
}
