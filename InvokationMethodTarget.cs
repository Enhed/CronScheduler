using System;
using System.Reflection;
using NCrontab;

namespace CronScheduler
{
    public sealed class InvokationMethodTarget
    {
        public InvokationMethodTarget(string cron, Type type, MethodInfo info, object instanse = null)
        {
            Type = type;
            Cron = cron;
            MethodInfo = info;
            Instanse = instanse;

            options = new CrontabSchedule.ParseOptions { IncludingSeconds = true };
            CreateSheduler();
        }

        public readonly Type Type;
        public readonly string Cron;
        public readonly MethodInfo MethodInfo;
        public readonly object Instanse;

        private readonly CrontabSchedule.ParseOptions options;
        private CrontabSchedule sheduler;
        private DateTime next;

        public bool IsElapsed()
        {
            return DateTime.Now >= next;
        }

        public void MoveNext()
        {
            next = sheduler.GetNextOccurrence(next);
        }

        public void Reset()
        {
            CreateSheduler();
        }

        private void CreateSheduler()
        {
            sheduler = CrontabSchedule.Parse(Cron, options);
            next = sheduler.GetNextOccurrence(DateTime.Now);
        }
    }
}