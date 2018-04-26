# CronScheduler

```c#

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

using System;
using CronScheduler;

namespace CronFoo
{
    class Program
    {
        static void Main(string[] args)
        {
            CronService.Errored += OnError;
            CronService.InitStatic(typeof(Program));
            var p = new Program();
            p.InitCrones();
            Console.ReadLine();
        }

        private static void OnError(InvokeError obj)
        {
            System.Console.WriteLine($"[{obj.Type.Name}.{obj.MethodInfo.Name}] throw error: {obj.Message}");
        }

        [Cron("*/5 * * * * *")]
        private static void Foo()
        {
            System.Console.WriteLine($"[{DateTime.Now}]: hello cron");
        }

        [Cron("*/7 * * * * *")]
        private static void Foo2()
        {
            System.Console.WriteLine($"[{DateTime.Now}]: hello cron foo 2");

        }

        [Cron("*/3 * * * * *")]
        private void FooIn()
        {
            System.Console.WriteLine($"[{DateTime.Now}]: hello cron foo instanse");
            throw new Exception("just test");
        }
    }
}

```