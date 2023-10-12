
using Microsoft.AspNetCore.SignalR;
using HaloSoft.EventLogger;

namespace CarSpeedWebsite.Models {
    public class NotificationHub : Hub {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected!!!! {this.Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Disconnected!");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task PreviewState(CarSpeedMonitorState state) {
            var task =  new Task(()=>{
                if ( MonitorPreview.Instance.SetState(state)) {
                    // Send out message
                    this.Clients.All.SendAsync("MonitorStateUpdated",MonitorPreview.Instance.State);
                }
            });
            task.Start();
            await task;

        }

        public async Task LogMessage(string message) {
            var task =  new Task(()=>{
                // Send out message
                this.Clients.All.SendAsync("LogMessage",message);
            });
            task.Start();
            await task;
        }
    }
}
