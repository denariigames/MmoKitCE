using System.Reflection;

namespace Siccity.SerializableCallback
{
    public class InvokableEvent : InvokableEventBase
    {
        private object target;
        private string methodName;

        public override void Invoke(params object[] args)
        {
            if (target == null || string.IsNullOrEmpty(methodName))
                return;
            target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(target, args);
        }

        public InvokableEvent(object target, string methodName)
        {
            this.target = target;
            this.methodName = methodName;
        }
    }
}
