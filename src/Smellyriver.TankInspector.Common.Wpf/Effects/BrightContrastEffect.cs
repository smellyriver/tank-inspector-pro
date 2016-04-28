using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Smellyriver.TankInspector.Common.Wpf.Effects
{

    public class BrightContrastEffect : ShaderEffect
    {
        private static PixelShader m_shader = new PixelShader() { UriSource = new Uri(@"pack://application:,,,/Smellyriver.TankInspector.Common.Wpf;component/Shaders/BrightnessAndContrast.ps") };

        static BrightContrastEffect()
        {
            // invoke the PackUriHelper static constructor to register the "pack://" protocal
            RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
        }

        public BrightContrastEffect()
        {
            this.PixelShader = m_shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);

        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BrightContrastEffect), 0);

        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(double), typeof(BrightContrastEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));

        public double Contrast
        {
            get { return (double)GetValue(ContrastProperty); }
            set { SetValue(ContrastProperty, value); }
        }

        public static readonly DependencyProperty ContrastProperty = DependencyProperty.Register("Contrast", typeof(double), typeof(BrightContrastEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(1)));

       
    }
}
