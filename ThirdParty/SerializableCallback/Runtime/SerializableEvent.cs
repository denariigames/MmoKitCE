namespace Siccity.SerializableCallback
{
    [System.Serializable]
    public class SerializableEvent : SerializableEventBase
    {
        public void Invoke()
        {
            if (invokable == null) Cache();
            if (_dynamic)
            {
                InvokableEvent call = invokable as InvokableEvent;
                call.Invoke();
            }
            else
            {
                invokable.Invoke(Args);
            }
        }
    }

    public abstract class SerializableEvent<T0> : SerializableEventBase
    {
        public void Invoke(T0 arg0)
        {
            if (invokable == null) Cache();
            if (_dynamic)
            {
                invokable.Invoke(arg0);
            }
            else
            {
                invokable.Invoke(Args);
            }
        }
    }

    public abstract class SerializableEvent<T0, T1> : SerializableEventBase
    {
        public void Invoke(T0 arg0, T1 arg1)
        {
            if (invokable == null) Cache();
            if (_dynamic)
            {
                invokable.Invoke(arg0, arg1);
            }
            else
            {
                invokable.Invoke(Args);
            }
        }
    }

    public abstract class SerializableEvent<T0, T1, T2> : SerializableEventBase
    {
        public void Invoke(T0 arg0, T1 arg1, T2 arg2)
        {
            if (invokable == null) Cache();
            if (_dynamic)
            {
                invokable.Invoke(arg0, arg1, arg2);
            }
            else
            {
                invokable.Invoke(Args);
            }
        }
    }

    public abstract class SerializableEvent<T0, T1, T2, T3> : SerializableEventBase
    {
        public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (invokable == null) Cache();
            if (_dynamic)
            {
                invokable.Invoke(arg0, arg1, arg2, arg3);
            }
            else
            {
                invokable.Invoke(Args);
            }
        }
    }
}
