using Microsoft.AspNetCore.SignalR;

namespace SMS.Web.Hubs
{
    public class StudentUploadHub : Hub
    {
        public async Task SendProgress(string message, int percentage)
        {
            await Clients.All.SendAsync("ReceiveProgress", message, percentage);
        }
    }
}
