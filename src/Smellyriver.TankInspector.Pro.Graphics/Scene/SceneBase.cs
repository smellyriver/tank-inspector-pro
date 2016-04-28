using System;
using System.ComponentModel;
using Smellyriver.TankInspector.Common.Wpf;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    public abstract class SceneBase<T> : DependencyNotificationObject , IDirect3D , IDisposable
		where T : D3D
	{
		/// <summary>
		/// 
		/// </summary>
		private T m_context;

		/// <summary>
		/// 
		/// </summary>
		public virtual T Renderer 
		{
			get { return m_context; }
			set
			{
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Renderer != null)
                    {
                        Renderer.Rendering -= ContextRendering;
                        this.Detach();
                    }
                    m_context = value;
                    if (Renderer != null)
                    {
                        Renderer.Rendering += ContextRendering;
                        this.Attach();
                    }
                }
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public abstract void RenderScene(DrawEventArgs args);

		/// <summary>
		/// 
		/// </summary>
        public virtual void Dispose()
        {
            Disposer.RemoveAndDispose(ref m_context);
        }

		/// <summary>
		/// 
		/// </summary>
		protected abstract void Attach();

		/// <summary>
		/// 
		/// </summary>
		protected abstract void Detach();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		void IDirect3D.Reset(DrawEventArgs args)
		{
			if (Renderer != null)
				Renderer.Reset(args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		void IDirect3D.Render(DrawEventArgs args)
		{
			if (Renderer != null)
				Renderer.Render(args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aCtx"></param>
		/// <param name="args"></param>
		private void ContextRendering(object aCtx, DrawEventArgs args) { RenderScene(args); }


	}
}
