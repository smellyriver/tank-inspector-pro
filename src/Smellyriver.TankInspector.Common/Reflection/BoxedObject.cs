using System;
using System.Runtime.InteropServices;

namespace Smellyriver.TankInspector.Common.Reflection
{
    public class BoxedObject : IDisposable
    {

        public static void SetValue(object target, object value)
        {
            new BoxedObject(target).Value = value;
        }

        private object _boxedObject;
        private GCHandle _memData;

        public object Value
        {
            get { return _boxedObject; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (value.GetType() != _boxedObject.GetType())
                    throw new NotSupportedException(string.Format("Can not set [{0}] value to [{1}] object", value.GetType().Name, _boxedObject.GetType().Name));

                if (!_memData.IsAllocated)
                    _memData = GCHandle.Alloc(_boxedObject);

                IntPtr pMemData = GCHandle.ToIntPtr(_memData);
                IntPtr pBox = new IntPtr((Marshal.ReadIntPtr(pMemData).ToInt64() + IntPtr.Size));
                Marshal.StructureToPtr(value, pBox, false);
            }
        }

        public BoxedObject(object boxObject)
        {
            if (boxObject == null)
                throw new ArgumentNullException();

            _boxedObject = boxObject;
        }

        ~BoxedObject()
        {
            (this as IDisposable).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (_memData.IsAllocated)
                _memData.Free();
        }
    }

}
