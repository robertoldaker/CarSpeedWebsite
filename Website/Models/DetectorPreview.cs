using System.Text.Json.Serialization;
using Npgsql.Internal.TypeHandlers;

namespace CarSpeedWebsite.Models;

public class DetectorState {
    public string state {get; set;}
    public float frameRate {get; set;}
    public bool detectionEnabled {get; set;}
    public int avgContours {get; set;}
    [JsonIgnore]
    public byte[] jpg {get; set;}
}

public class DetectorPreview {
    private static DetectorPreview? _instance=null;

    private static object _instanceLock = new object();

    public static DetectorPreview Instance {
        get {
            lock (_instanceLock) {
                if ( _instance==null) {
                    _instance  = new DetectorPreview();
                }
                return _instance;
            }
        }
    }

    private DetectorState _state;
    private object _stateLock = new object();

    private AutoResetEvent _stateAvailable = new AutoResetEvent(false);

    public DetectorPreview() {
        _state = new DetectorState();
    }

    public void setState(DetectorState state) {
        lock( _stateLock) {
            _state = state;
        }
    }

    public DetectorState State {
        get {
            lock (_stateLock) {
                return _state;
            }
        }
        set {
            lock(_stateLock) {
                _state = value;
                _stateAvailable.Set();
            }
        }
    }

    public async Task<byte[]> GetNextImageAsync() {

        await Task.Run( ()=> { _stateAvailable.WaitOne();});
        return _state.jpg;

    }
}