namespace Siccity.SerializableCallback
{
    public abstract class SerializableCallback<TReturn> : SerializableCallbackBase<TReturn>
    {
        public TReturn Invoke()
        {
            if (func == null) Cache();
            if (_dynamic)
            {
                return func.Invoke();
            }
            else
            {
                return func.Invoke(Args);
            }
        }
    }

    public abstract class SerializableCallback<T0, TReturn> : SerializableCallbackBase<TReturn>
    {
        public TReturn Invoke(T0 arg0)
        {
            if (func == null) Cache();
            if (_dynamic)
            {
                return func.Invoke(arg0);
            }
            else
            {
                return func.Invoke(Args);
            }
        }
    }

    public abstract class SerializableCallback<T0, T1, TReturn> : SerializableCallbackBase<TReturn>
    {
        public TReturn Invoke(T0 arg0, T1 arg1)
        {
            if (func == null) Cache();
            if (_dynamic)
            {
                return func.Invoke(arg0, arg1);
            }
            else
            {
                return func.Invoke(Args);
            }
        }
    }

    public abstract class SerializableCallback<T0, T1, T2, TReturn> : SerializableCallbackBase<TReturn>
    {
        public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
        {
            if (func == null) Cache();
            if (_dynamic)
            {
                return func.Invoke(arg0, arg1, arg2);
            }
            else
            {
                return func.Invoke(Args);
            }
        }
    }

    public abstract class SerializableCallback<T0, T1, T2, T3, TReturn> : SerializableCallbackBase<TReturn>
    {
        public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (func == null) Cache();
            if (_dynamic)
            {
                return func.Invoke(arg0, arg1, arg2, arg3);
            }
            else
            {
                return func.Invoke(Args);
            }
        }
    }
}
