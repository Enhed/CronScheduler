using System;

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
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CronAttribute : Attribute
    {
        public readonly string Value;

        /// <summary>
        /// separator whitespace
        /// sec (0 - 59)
        /// min (0 - 59)
        /// hour (0 - 23)
        /// day of month(1 - 31)
        /// month (1 - 12)
        /// day of week (0 - 6) (Sunday=0)
        /// </summary>
        /// <param name="cron"></param>
        public CronAttribute(string cron)
        {
            Value = cron;
        }
    }
}