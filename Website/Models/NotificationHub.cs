
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

        public async Task PreviewState(DetectorState state) {
            var task =  new Task(()=>{
                DetectorPreview.Instance.State = state;
            });
            task.Start();
            await task;

        }

    }
}
