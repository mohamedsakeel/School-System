using Microsoft.AspNetCore.SignalR;
using SMS.AppCore.Interfaces;
using SMS.Web.Hubs;

namespace SMS.Web.Services
{
    public class ProgressNotifier : IProgressNotifier
    {
        private readonly IHubContext<StudentUploadHub> _hubContext;
        public ProgressNotifier(IHubContext<StudentUploadHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendProgress(string message, int percentage)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveProgress", message, percentage);
        }
    }
}
