using System.Text.Json.Serialization;
using Npgsql.Internal.TypeHandlers;

namespace CarSpeedWebsite.Models;

public class CarSpeedMonitorState {
    public string state {get; set;}
    public float frameRate {get; set;}
    public bool detectionEnabled {get; set;}
    public int avgContours {get; set;}
    public int lightLevel {get; set;}
    public float cpu {get; set;}
    public byte[] jpg {get; set;}    
}

public class MonitorState {
    public string state {get; set;}
    public float frameRate {get; set;}
    public bool detectionEnabled {get; set;}
    public int avgContours {get; set;}
    public int lightLevel {get; set;}
    public float cpu {get; set;}

    public bool Update(CarSpeedMonitorState monitorState) {
        bool updated = false;
        if ( state!=monitorState.state) {
            state = monitorState.state;
            updated=true;
        }
        if ( Math.Abs(frameRate - monitorState.frameRate)>=0.1) {
            frameRate = monitorState.frameRate;
            updated = true;
        }
        if ( detectionEnabled!=monitorState.detectionEnabled) {
            detectionEnabled = monitorState.detectionEnabled;
            updated = true;
        }
        if ( avgContours!=monitorState.avgContours ) {
            avgContours = monitorState.avgContours;
            updated = true;
        }
        if ( lightLevel!=monitorState.lightLevel ) {
            lightLevel = monitorState.lightLevel;
            updated = true;
        }
        if ( cpu!=monitorState.cpu ) {
            cpu = monitorState.cpu;
            updated = true;
        }
        return updated;
    }
}

public class MonitorPreview {
    private static MonitorPreview? _instance=null;

    private static object _instanceLock = new object();

    public static MonitorPreview Instance {
        get {
            lock (_instanceLock) {
                if ( _instance==null) {
                    _instance  = new MonitorPreview();
                }
                return _instance;
            }
        }
    }

    private MonitorState _state;
    private byte[] _jpgImage;
    private object _stateLock = new object();

    private AutoResetEvent _imageAvailable = new AutoResetEvent(false);

    public MonitorPreview() {
        _state = new MonitorState();
        _jpgImage = new byte[0];
    }

    public bool SetState(CarSpeedMonitorState state) {
        lock( _stateLock) {
            bool stateUpdated = _state.Update(state);
            _jpgImage = state.jpg;
            _imageAvailable.Set();
            return stateUpdated;
        }
    }

    public MonitorState State {
        get {
            lock(_stateLock) {
                return _state;
            }
        }
    }

    public byte[] JpgImage {
        get {
            lock( _stateLock) {
                return _jpgImage;
            }
        }
    }

    public async Task<byte[]> GetNextImageAsync() {

        await Task.Run( ()=> { _imageAvailable.WaitOne();});
        return _jpgImage;

    }

}