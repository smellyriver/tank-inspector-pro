using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator.Effects
{
    public class CamouflageEffect : ShaderEffect
    {
        private static PixelShader _shader = new PixelShader() { UriSource = new Uri(@"pack://application:,,,/Smellyriver.TankInspector.Pro.CustomizationConfigurator;component/Effects/Camouflage.ps") };

        static CamouflageEffect()
        {
            // invoke the PackUriHelper static constructor to register the "pack://" protocal
            RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
        }

        public CamouflageEffect()
        {
            this.PixelShader = _shader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(Color1Property);
            this.UpdateShaderValue(Color2Property);
            this.UpdateShaderValue(Color3Property);
            this.UpdateShaderValue(Color4Property);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty 
            = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(CamouflageEffect), 0);

        public Color BaseColor
        {
            get { return (Color)GetValue(BaseColorProperty); }
            set { SetValue(BaseColorProperty, value); }
        }

        public static readonly DependencyProperty BaseColorProperty
            = DependencyProperty.Register("BaseColor",
                                          typeof(Color),
                                          typeof(CamouflageEffect),
                                          new UIPropertyMetadata(Colors.Transparent, ShaderEffect.PixelShaderConstantCallback(0)));

        public Color Color1
        {
            get { return (Color)GetValue(Color1Property); }
            set { SetValue(Color1Property, value); }
        }

        public static readonly DependencyProperty Color1Property 
            = DependencyProperty.Register("Color1", 
                                          typeof(Color), 
                                          typeof(CamouflageEffect), 
                                          new UIPropertyMetadata(Colors.Transparent, ShaderEffect.PixelShaderConstantCallback(1)));

        public Color Color2
        {
            get { return (Color)GetValue(Color2Property); }
            set { SetValue(Color2Property, value); }
        }

        public static readonly DependencyProperty Color2Property
            = DependencyProperty.Register("Color2",
                                          typeof(Color),
                                          typeof(CamouflageEffect),
                                          new UIPropertyMetadata(Colors.Transparent, ShaderEffect.PixelShaderConstantCallback(2)));

        public Color Color3
        {
            get { return (Color)GetValue(Color3Property); }
            set { SetValue(Color3Property, value); }
        }

        public static readonly DependencyProperty Color3Property
            = DependencyProperty.Register("Color3",
                                          typeof(Color),
                                          typeof(CamouflageEffect),
                                          new UIPropertyMetadata(Colors.Transparent, ShaderEffect.PixelShaderConstantCallback(3)));

        public Color Color4
        {
            get { return (Color)GetValue(Color4Property); }
            set { SetValue(Color4Property, value); }
        }

        public static readonly DependencyProperty Color4Property
            = DependencyProperty.Register("Color4",
                                          typeof(Color),
                                          typeof(CamouflageEffect),
                                          new UIPropertyMetadata(Colors.Transparent, ShaderEffect.PixelShaderConstantCallback(4)));
    }
}
