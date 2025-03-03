using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.Interfaces
{
    public interface IProgressNotifier
    {
        Task SendProgress(string message, int percentage);
    }
}
