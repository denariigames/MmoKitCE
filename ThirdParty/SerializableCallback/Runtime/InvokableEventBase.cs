namespace Siccity.SerializableCallback
{
    public abstract class InvokableEventBase
    {
        public abstract void Invoke(params object[] args);
    }
}
