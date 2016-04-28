using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    public interface ISnapshotProvider
    {
        BitmapSource Snapshot(Rect rect, double sampleRatio, Color? backgroundColor = null);
        BitmapSource[] YawAnimationSnapshot(Rect rect, double sampleRatio, Color? backgroundColor, double rotationSpeed, double frameRate, IProgressScope progress, Func<bool> getIsCancelled);
    }
}
