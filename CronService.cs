﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection;

/*
* * * * * *
- - - - - -
| | | | | |
| | | | | +--- day of week (0 - 6) (Sunday=0)
| | | | +----- month (1 - 12)
| | | +------- day of month (1 - 31)
| | +--------- hour (0 - 23)
| +----------- min (0 - 59)
+------------- sec (0 - 59)
*/

namespace CronScheduler
{
    public static class CronService
    {
        public const BindingFlags ShareFlags
            = BindingFlags.Public | BindingFlags.NonPublic;

        public static event Action<InvokeError> Errored;
        private readonly static Timer timer;
        private readonly static ConcurrentBag<InvokationMethodTarget> targets;
        public static InvokationMethodTarget[] RegisterTargets
            => targets.ToArray();

        static CronService()
        {
            targets = new ConcurrentBag<InvokationMethodTarget>();
            timer = new Timer(1000);
            timer.Elapsed += OnElapsed;
            timer.Start();
        }

        public static void InitInstanse(object instanse)
        {
            GetInstanseTargets(instanse).ToList().ForEach(Add);
        }

        public static void InitStatic(Type type)
        {
            GetStaticTargets(type).ToList().ForEach(Add);
        }

        public static CronAttribute GetCron(MethodInfo methodInfo)
        {
            var type = typeof(CronAttribute);
            return (CronAttribute) methodInfo.GetCustomAttribute(type);
        }

        public static bool IsTarget(MethodInfo methodInfo)
        {
            return GetCron(methodInfo) != null;
        }

        public static bool HasTarget(Type type)
        {
            return type.GetMethods().Any(IsTarget);
        }

        public static void Add(InvokationMethodTarget target)
        {
            if(targets.Any(t => t.Type == target.Type
                && t.MethodInfo == target.MethodInfo && t.Instanse == target.Instanse)) return;

            targets.Add(target);
        }

        public static InvokationMethodTarget Take(Type type, string methodName, object instanse = null)
        {
            return targets.FirstOrDefault( ct => ct.Type == type
                && ct.MethodInfo.Name == methodName && ct.Instanse == instanse );
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if(targets.Count == 0) return;
            else Invoke();
        }

        private static async void Invoke()
        {
            void invoke(InvokationMethodTarget target)
            {
                target.MoveNext();

                try
                {
                    target.MethodInfo.Invoke(target.Instanse, null);
                }
                catch(Exception ex)
                {
                    Errored?.Invoke( new InvokeError("Error invokation", target, ex) );
                }
            }

            Task getTask(InvokationMethodTarget target)
            {
                return Task.Run( () => invoke(target) );
            }

            var tasks = targets.Where(t => t.IsElapsed())
                .Select(getTask);

            await Task.WhenAll(tasks);
        }

        public static void InitCrones(this object o)
        {
            InitInstanse(o);
        }

        public static InvokationMethodTarget[] GetInstanseTargets(object instanse)
        {
            return instanse.GetType()
                .GetMethods(ShareFlags | BindingFlags.Instance)
                .Select( method => ( mi : method, cron : GetCron(method) ) )
                .Where( t => t.cron != null )
                .Select( t => new InvokationMethodTarget(t.cron.Value, instanse.GetType(), t.mi, instanse) )
                .ToArray();
        }

        public static InvokationMethodTarget[] GetStaticTargets(Type type)
        {
            return type.GetMethods(ShareFlags | BindingFlags.Static)
                .Select( method => ( mi : method, cron : GetCron(method) ) )
                .Where( t => t.cron != null )
                .Select( t => new InvokationMethodTarget(t.cron.Value, type, t.mi) )
                .ToArray();
        }
    }
}