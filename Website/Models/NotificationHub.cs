
using Microsoft.AspNetCore.SignalR;
using HaloSoft.EventLogger;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CarSpeedWebsite.Models {    
    public class NotificationHub : Hub {
        public override Task OnConnectedAsync()
        {
            var monitorName = getMonitorName();
            Console.WriteLine($"Connected!!!! [{monitorName}] [{this.Context.ConnectionId}]");
            if ( monitorName!=null) {
                // Add to central list of valid monitor connections
                ConnectionManager.Instance.AddMonitorConnection(monitorName,this.Context.ConnectionId);
                // Send out message to browsers
                this.getAllBrowsers().SendAsync("MonitorConnected",new {monitorName=monitorName});
            }
            return base.OnConnectedAsync();
        }

        private string? getMonitorName() {
            string? monitorName=null;
            var httpContext=this.Context.GetHttpContext();
            if ( httpContext!=null && httpContext.Request.Headers.TryGetValue("monitor-name", out var values) ) {
                monitorName=values.First();
            }
            return monitorName;
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var monitorName = getMonitorName();
            if ( monitorName!=null) {
                // Remove from central list
                ConnectionManager.Instance.RemoveMonitorConnection(monitorName,this.Context.ConnectionId);
                // Send out message to browsers
                this.getAllBrowsers().SendAsync("MonitorDisconnected",new {monitorName=monitorName});
            }
            Console.WriteLine($"Disconnected!!!! [{monitorName}] [{this.Context.ConnectionId}]");
            return base.OnDisconnectedAsync(exception);
        }

        private IClientProxy getAllBrowsers() {
            var monitorIds = ConnectionManager.Instance.GetAllMonitorIds();
            return this.Clients.AllExcept(monitorIds);
        }

        public async Task PreviewState(CarSpeedMonitorState state) {
            var task =  new Task(()=>{
                var monitorName = getMonitorName();
                if ( monitorName!=null) {
                    if ( MonitorPreview.Instance.SetState(monitorName,state)) {
                        // Send out message to browsers
                        this.getAllBrowsers().SendAsync("MonitorStateUpdated",
                            new {monitorName=monitorName, monitorState=MonitorPreview.Instance.GetState(monitorName)});
                    }
                }
            });
            task.Start();
            await task;
        }
        

        public async Task LogMessage(string message) {
            var task =  new Task(()=>{
                var monitorName = getMonitorName();
                if ( monitorName!=null) {
                    // Send out message
                    this.getAllBrowsers().SendAsync("LogMessage",new {monitorName=monitorName, message=message});
                }
            });
            task.Start();
            await task;
        }

    }

    public class ConnectionManager {
        private static ConnectionManager? _instance=null;
        private static object _instanceLock = new object();

        public static ConnectionManager Instance {
            get {
                lock( _instanceLock) {
                    if ( _instance==null) {
                        _instance = new ConnectionManager();
                    }
                    return _instance;
                }
            }
        }

        private ConnectionManager() {

        }
        private Dictionary<string,List<string>> _connectedMonitorsDict = new Dictionary<string, List<string>>();
        private Dictionary<string,string> _connectionIdsDict = new Dictionary<string, string>();
        private object _connectedMonitorsLock = new object();

        public void AddMonitorConnection(string monitorName, string connectionId) {
            lock( _connectedMonitorsLock) {
                // connectionIds by monitorName
                if ( !_connectedMonitorsDict.ContainsKey(monitorName)) {
                    _connectedMonitorsDict.Add(monitorName,new List<string>());
                }
                _connectedMonitorsDict[monitorName].Add(connectionId);
                // monitorName by connectionId
                if ( !_connectionIdsDict.ContainsKey(connectionId) ) {
                    _connectionIdsDict.Add(connectionId,monitorName);
                } else {
                    _connectionIdsDict[connectionId]=monitorName;
                }
            }
        }

        public void RemoveMonitorConnection(string monitorName, string connectionId) {
            lock(_connectedMonitorsLock) {
                if ( _connectedMonitorsDict.ContainsKey(monitorName)) {
                    var connections = _connectedMonitorsDict[monitorName];
                    connections.Remove(connectionId);
                    // remove entry in dictionary if last connection remaining
                    if ( connections.Count==0) {
                        _connectedMonitorsDict.Remove(monitorName);
                    }
                }
                if ( _connectionIdsDict.ContainsKey(connectionId)) {
                    _connectionIdsDict.Remove(connectionId);
                }
            }
        }

        public IReadOnlyList<string> GetAllMonitorIds() {
            lock ( _connectedMonitorsLock) {
                var list=new List<string>();            
                foreach( var key in _connectedMonitorsDict.Keys) {
                    list.AddRange(_connectedMonitorsDict[key]);
                }
                return list;
            }
        }

        public IReadOnlyList<string> GetMonitorIds(string monitorName) {
            lock(_connectedMonitorsLock) {
                if ( _connectedMonitorsDict.TryGetValue(monitorName, out List<string>? connectionIds)) {
                    return connectionIds;
                } else {
                    return new List<string>();
                }
            }
        }

        public bool IsConnected(string monitorName) {
            lock ( _connectedMonitorsLock) {
                return _connectedMonitorsDict.ContainsKey(monitorName);
            }
        }
        
        public List<string> GetConnectionIds(string monitorName) {
            lock ( _connectedMonitorsLock) {
                if ( _connectedMonitorsDict.TryGetValue(monitorName, out List<string>? connectedIds ) ) {
                    return connectedIds;
                } else {
                    return new List<string>();
                }
            }
        }
    }
}
