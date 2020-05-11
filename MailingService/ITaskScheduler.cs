using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingService
{
    interface ITaskScheduler
    {
        string Name { get; }
        void Run();
        void Stop();
    }
}
