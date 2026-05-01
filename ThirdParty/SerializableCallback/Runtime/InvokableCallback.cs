using System.Reflection;

namespace Siccity.SerializableCallback
{
    public class InvokableCallback<TReturn> : InvokableCallbackBase<TReturn>
    {
        private object target;
        private string methodName;

        public override TReturn Invoke(params object[] args)
        {
            if (target == null || string.IsNullOrEmpty(methodName))
                return default(TReturn);
            return (TReturn)target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(target, args);
        }

        public InvokableCallback(object target, string methodName)
        {
            this.target = target;
            this.methodName = methodName;
        }
    }
}
