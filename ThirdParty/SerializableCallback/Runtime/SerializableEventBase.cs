using System;

namespace Siccity.SerializableCallback
{
    public abstract class SerializableEventBase : SerializableCallbackBase
    {
        public InvokableEventBase invokable;

        public override void ClearCache()
        {
            base.ClearCache();
            invokable = null;
        }

        protected override void Cache()
        {
            if (_target == null || string.IsNullOrEmpty(_methodName))
            {
                invokable = new InvokableEvent(null, null);
            }
            else
            {
                if (_dynamic)
                {
                    invokable = new InvokableEvent(target, methodName);
                }
                else
                {
                    invokable = GetPersistentMethod();
                }
            }
        }

        protected InvokableEventBase GetPersistentMethod()
        {
            Type[] types = new Type[ArgTypes.Length];
            Array.Copy(ArgTypes, types, ArgTypes.Length);
            return new InvokableEvent(target, methodName);
        }
    }
}