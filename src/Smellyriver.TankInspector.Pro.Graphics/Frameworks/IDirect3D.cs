namespace Smellyriver.TankInspector.Pro.Graphics.Frameworks
{
	/// <summary>
	/// The DirectX renderer displayed by the DXElement
	/// </summary>
	public interface IDirect3D
	{
		void Reset(DrawEventArgs args);
		void Render(DrawEventArgs args);
	}

}
