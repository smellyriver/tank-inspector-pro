using System;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.Graphics.Scene;

namespace Smellyriver.TankInspector.Pro.Graphics.Smaa
{
    class SMAA : IDisposable
    {
        Device _device;
        int _width, _height;

        Preset _preset;
        Effect _effect;

        public Texture _edgeTex;
        public Texture _blendTex;
        Surface _edgeSurface;
        Surface _blendSurface;

        Texture _areaTex;
        Texture _searchTex;

        QuadRender _quadRender;

        public enum Preset 
        { 
            PRESET_LOW = 0, 
            PRESET_MEDIUM = 1, 
            PRESET_HIGH = 2, 
            PRESET_ULTRA = 3, 
            PRESET_CUSTOM = 4,
        };

        static Macro[] _presetMacros = new Macro[]
            {
                new Macro(){ Name =  "SMAA_PRESET_LOW", Definition = "1" },
                new Macro(){ Name = "SMAA_PRESET_MEDIUM", Definition = "1" },
                new Macro(){ Name = "SMAA_PRESET_HIGH", Definition = "1" },
                new Macro(){ Name = "SMAA_PRESET_ULTRA", Definition = "1" },
                new Macro(){ Name = "SMAA_PRESET_CUSTOM",Definition =  "1" }
            };

        public SMAA(Device device, int width, int height, Preset preset)
        {
            _device = device;
            _width = width;
            _height = height;
            _preset = preset;


            Macro[] defines = new Macro[]
            {
               new Macro(){ Name = "SMAA_DIRECTX9_LINEAR_BLEND", Definition = "1"},
               _presetMacros[(int)preset],
               new Macro(),
            };

            _effect = Effect.FromFile(device, @"Graphics\Effect\SMAA.fx", defines, null, "", ShaderFlags.None);


            _areaTex = Texture.FromFile(device, @"Graphics\Texture\AreaTexDX9.dds");
            _searchTex = Texture.FromFile(device, @"Graphics\Texture\SearchTex.dds");
        }


        internal void Reset(int width, int height, QuadRender quadRender)
        {
            _width = width;
            _height = height;
            _quadRender = quadRender;

            Disposer.RemoveAndDispose(ref _edgeSurface);
            Disposer.RemoveAndDispose(ref _edgeTex);
            _edgeTex = new Texture(_device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            _edgeSurface = _edgeTex.GetSurfaceLevel(0);

            Disposer.RemoveAndDispose(ref _blendSurface);
            Disposer.RemoveAndDispose(ref _blendTex);
            _blendTex = new Texture(_device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            _blendSurface = _blendTex.GetSurfaceLevel(0);
        }


        public void Apply(Texture edges, Texture src, Surface dst)
        {
            EdgesDetectionPass(edges);
            BlendingWeightsCalculationPass();
            NeighborhoodBlendingPass(src, dst);
        }

        public void NeighborhoodBlendingPass(Texture src, Surface dst)
        {
            _device.SetRenderTarget(0, dst);
            _effect.SetTexture("colorTex2D", src);
            _effect.SetTexture("blendTex2D", _blendTex);
            _effect.SetValue("pixelSize", new float[] { 1.0f / _width, 1.0f / _height });
            _effect.Technique = _effect.GetTechnique("NeighborhoodBlending");

            _effect.Begin();
            _effect.BeginPass(0);
            _quadRender.Render(_device);
            _effect.EndPass();
            _effect.End();
        }

        public void BlendingWeightsCalculationPass()
        {
            _device.SetRenderTarget(0, _blendSurface);
            _device.Clear(ClearFlags.Target, new Color(0, 0, 0, 0), 1.0f, 0);
            _effect.SetTexture("edgesTex2D", _edgeTex);
            _effect.SetTexture("areaTex2D", _areaTex);
            _effect.SetTexture("searchTex2D", _searchTex);
            _effect.SetValue("pixelSize",new float[]{1.0f/_width,1.0f/_height});

            _effect.Technique = _effect.GetTechnique("BlendWeightCalculation");

            _effect.Begin();
            _effect.BeginPass(0);
            _quadRender.Render(_device);
            _effect.EndPass();
            _effect.End();
        }

        public void EdgesDetectionPass(Texture edges)
        {
            _device.SetRenderTarget(0, _edgeSurface);
            _device.Clear(ClearFlags.Target, new Color(0, 0, 0, 0), 1.0f, 0);
            //_effect.SetValue("threshld", _threshold);
            //_effect.SetValue("maxSearchSteps", (float)_maxSearchSteps);
            //_effect.SetValue("maxSearchStepsDiag", (float)_maxSearchStepsDiag);
            //_effect.SetValue("cornerRounding", _cornerRounding);
            //可优化为深度检索
            _effect.SetTexture("colorTex2D",edges);
            _effect.SetValue("pixelSize", new float[] { 1.0f / _width, 1.0f / _height });
            _effect.Technique = _effect.GetTechnique("ColorEdgeDetection");

            _effect.Begin();
            _effect.BeginPass(0);
            _quadRender.Render(_device);
            _effect.EndPass();
            _effect.End();
        }

        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref _quadRender);
            Disposer.RemoveAndDispose(ref _edgeSurface);
            Disposer.RemoveAndDispose(ref _edgeTex);
            Disposer.RemoveAndDispose(ref _blendSurface);
            Disposer.RemoveAndDispose(ref _blendTex);
            Disposer.RemoveAndDispose(ref _areaTex);
            Disposer.RemoveAndDispose(ref _searchTex);
            Disposer.RemoveAndDispose(ref _effect);
        }
    }
}
