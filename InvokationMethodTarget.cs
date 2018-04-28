using System;
using System.Reflection;
using NCrontab;

namespace CronScheduler
{
    public sealed class InvokationMethodTarget
    {
        private string cron;
        private bool enabled;

        public InvokationMethodTarget(string cron, Type type, MethodInfo info,
            object instanse = null, bool enabled = true)
        {
            Type = type;
            this.cron = cron;
            MethodInfo = info;
            Instanse = instanse;
            this.enabled = enabled;

            options = new CrontabSchedule.ParseOptions { IncludingSeconds = true };
            CreateSheduler();
        }

        public readonly Type Type;
        public readonly MethodInfo MethodInfo;
        public readonly object Instanse;

        private readonly CrontabSchedule.ParseOptions options;
        private CrontabSchedule sheduler;
        private DateTime next;

        public string FullName => $"{Type.Name}.{MethodInfo.Name}";

        public string Cron
        {
            get => cron;
            set
            {
                this.cron = value;
                CreateSheduler();
            }
        }

        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if(enabled) CreateSheduler();
            }
        }

        public bool IsElapsed()
        {
            return Enabled && DateTime.Now >= next;
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