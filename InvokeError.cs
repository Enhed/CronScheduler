using System;
using System.Reflection;

namespace CronScheduler
{
    public sealed class InvokeError : Exception
    {
        public InvokeError(string message, InvokationMethodTarget target, Exception ex) : base(message, ex)
        {
            Type = target.Type;
            MethodInfo = target.MethodInfo;
            Instanse = target.Instanse;
        }

        public readonly Type Type;
        public readonly MethodInfo MethodInfo;
        public readonly object Instanse;
    }
}