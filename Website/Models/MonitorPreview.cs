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
    public int exposureTime {get; set;} 
    public float analogueGain {get; set;}
    public List<float> cpus {get; set;}
}

public class MonitorState {
    public string state {get; set;}
    public float frameRate {get; set;}
    public bool detectionEnabled {get; set;}
    public int avgContours {get; set;}
    public int lightLevel {get; set;}
    public float cpu {get; set;}
    public int exposureTime {get; set;}
    public float analogueGain {get; set;}
    public List<float> cpus {get; set;}

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
        if ( Math.Abs(analogueGain -monitorState.analogueGain)>=0.01) {
            analogueGain = monitorState.analogueGain;
            updated = true;
        }
        if ( exposureTime!=monitorState.exposureTime) {
            exposureTime = monitorState.exposureTime;
            updated = true;
        }
        if ( cpus==null) {
            cpus=monitorState.cpus;
            updated=true;
        } else {
            for(int i=0;i<monitorState.cpus.Count;i++) {
                if ( Math.Abs( cpus[i]-monitorState.cpus[i]) > 0.1) {
                    cpus[i]=monitorState.cpus[i];
                    updated=true;
                }
            }
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

    private class StateInfo {
        public StateInfo(MonitorState state, byte[] jpgImage) {
            State = state;
            JpgImage = jpgImage;
            ImageAvailable = new AutoResetEvent(false);
        }
        public MonitorState State {get; set;}
        public byte[] JpgImage {get; set;}
        public AutoResetEvent ImageAvailable {get; private set;}
    }

    private Dictionary<string,StateInfo> _stateDict = new Dictionary<string, StateInfo>();
    private object _stateLock = new object();

    private byte[] _emptyImage = new byte[0];

    public MonitorPreview() {
    }

    public bool SetState(string monitorName, CarSpeedMonitorState state) {
        lock( _stateLock) {
            bool stateUpdated;
            if ( _stateDict.TryGetValue(monitorName,out StateInfo? stateInfo)) {
                stateInfo.JpgImage = state.jpg;
            } else {
                var monitorState = new MonitorState();
                stateInfo = new StateInfo(monitorState,state.jpg);
                _stateDict.Add(monitorName, stateInfo);
            }
            stateUpdated = stateInfo.State.Update(state);
            stateInfo.ImageAvailable.Set();
            return stateUpdated;
        }
    }

    public MonitorState? GetState(string monitorName) {
        lock(_stateLock) {
            _stateDict.TryGetValue(monitorName, out StateInfo? stateInfo);
            return stateInfo?.State;
        }
    }

    public byte[] GetJpgImage(string monitorName) {
        lock( _stateLock) {
            if ( _stateDict.TryGetValue(monitorName, out StateInfo? stateInfo) ) {
                return stateInfo.JpgImage;
            } else {
                return _emptyImage;
            }
        }
    }

    public async Task<byte[]> GetNextImageAsync(string monitorName) {

        StateInfo? stateInfo=null;
        await Task.Run( ()=> { 
            if ( _stateDict.TryGetValue(monitorName, out stateInfo) ) {
                stateInfo.ImageAvailable.WaitOne();
            }
        });
        if ( stateInfo!=null) {
            return stateInfo.JpgImage;
        } else {
            return _emptyImage;
        }
    }

}