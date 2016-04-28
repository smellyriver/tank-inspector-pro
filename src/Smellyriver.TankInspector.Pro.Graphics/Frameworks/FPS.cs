using System;
using System.Collections.Generic;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.Graphics.Frameworks
{
    public class FPS : NotificationObject
    {
        private double _value;

        private readonly List<TimeSpan> _frames = new List<TimeSpan>();

        private TimeSpan _averagingInterval = TimeSpan.FromSeconds(1);
        public TimeSpan AveragingInterval
        {
            get { return _averagingInterval; }
            set
            {
                if (value == _averagingInterval)
                    return;
                if (value < TimeSpan.FromSeconds(0.1))
                    throw new ArgumentOutOfRangeException();
                
                _averagingInterval = value;
                this.RaisePropertyChanged(() => this.AveragingInterval);
            }
        }
        

        public void AddFrame(TimeSpan ts)
        {
            var sec = AveragingInterval;
            var index = _frames.FindLastIndex(aTS => ts - aTS > sec);
            if (index > -1)
                _frames.RemoveRange(0, index);
            _frames.Add(ts);

            UpdateValue();
        }


        public void Clear()
        {
            _frames.Clear();
            UpdateValue();
        }


        public double Value
        {
            get { return _value; }
            private set
            {
                if (value == _value)
                    return;
                _value = value;
                this.RaisePropertyChanged(() => this.Value);
            }
        }

        private void UpdateValue()
        {
            if (_frames.Count < 2)
            {
                Value = -1;
            }
            else
            {
                var dt = _frames[_frames.Count - 1] - _frames[0];
                Value = dt.Ticks > 100 ? _frames.Count / dt.TotalSeconds : -1;
            }
        }



    }
}
