using System;
using System.Collections.Generic;
using IotDevices;
using UnityEngine;

public class SequentialDemo : SceneSingleton<SequentialDemo>
{
    [SerializeField] private SequentialDemoData _sequencedEvents;

    [SerializeField] private ProWatchIotDeviceManager proWatchIotDeviceManager;
    
    IDictionary<string, CameraEvent> _cameraEventCache = new Dictionary<string, CameraEvent>();
    
    IDictionary<string, string> replacedCameraEventCacahe = new Dictionary<string, string>();

    [SerializeField] private float elapsedSeconds;

    [SerializeField] List<SecurityDemoEvent> demoEventCopy;
    public event Action DemoReset;

    private void Start()
    {
        IotAlarmManager.Instance.AlarmAcknowledged += RestoreDefaultCameraURI;
        IotAlarmManager.Instance.AlarmDismissed += RestoreDefaultCameraURI;
    }

    private void OnDestroy()
    {
        IotAlarmManager.Instance.AlarmAcknowledged -= RestoreDefaultCameraURI;
        IotAlarmManager.Instance.AlarmDismissed -= RestoreDefaultCameraURI;
    }
    
    public void ProgressDemoOneStep(int delay)
    {
        CreateDemoSequence();
    }

    private void CreateDemoSequence()
    {
        demoEventCopy = new List<SecurityDemoEvent>(_sequencedEvents.security);
        foreach (SecurityDemoEvent demoEvent in demoEventCopy)
        {
            CameraEvent newCameraEvent = new CameraEvent
            {
                Uri = demoEvent.Uri,
                Id = demoEvent.CameraId
            };

            if (!_cameraEventCache.ContainsKey(demoEvent.CameraId))
            {
                //Debug.Log("Adding cam to cache " + demoEvent.CameraId + " URI - " + demoEvent.Uri);
                _cameraEventCache.Add(newCameraEvent.Id, newCameraEvent);
            }
        }
    }

    public void ResetDemo()
    {
        elapsedSeconds = 0;
        IotAlarmManager.Instance.DismissAllAlarms();
        LayerManager.Instance.SetAllLayersVisible(true);
        CreateDemoSequence();
        DemoReset?.Invoke();
    }

    private void Update()
    {
        if (AuthenticationManager.Instance.IsDemoMode && demoEventCopy.Count != 0)
        {
            elapsedSeconds += Time.deltaTime;
            ProgressDemo();
        }
    }

    private void ProgressDemo()
    {
        foreach (SecurityDemoEvent demoEvent in demoEventCopy)
        {
            if (elapsedSeconds >= demoEvent.Time && !demoEvent.isCamera)
            {
                //Debug.Log("firing new event" + demoEvent.AlarmId);
                SecurityEvent demoSecurityEvent = new SecurityEvent
                {
                    Id = demoEvent.AlarmId,
                    DeviceIds = new List<string>() { demoEvent.AlarmId, demoEvent.CameraId },
                    Time = demoEvent.Time,
                    EventType = demoEvent.EventType
                };

                IotAlarmDto iotAlarmEvent = IotAlarmHelper.FromSecurityEvent(demoSecurityEvent);
                proWatchIotDeviceManager.GenerateAlarm(iotAlarmEvent);

                CameraEvent newCameraEvent = new CameraEvent
                {
                    Uri = demoEvent.Uri,
                    Id = demoEvent.CameraId
                };
                if (!_cameraEventCache.ContainsKey(demoEvent.CameraId) && demoEvent.CameraId != string.Empty)
                {
                    //Debug.Log("Adding cam to cache " + demoEvent.CameraId);
                    _cameraEventCache.Add(newCameraEvent.Id, newCameraEvent);
                }
                else
                {
                    //Debug.Log("Replacing URL " + demoEvent.CameraId);
                    ReplaceSecurityCameraDemoUri(demoEvent.CameraId, newCameraEvent.Uri);
                }

                SecurityDemoEvent eventToRemove = demoEventCopy.Find(x => x.AlarmId == demoEvent.AlarmId);
                demoEventCopy.Remove(eventToRemove);
                break;
            }
        }
    }

    private void RestoreDefaultCameraURI(object sender, IotAlarmDto e)
    {
        //Debug.Log("Event video needs to be changed");

        List<string> keysToRemove = new List<string>();

        foreach (KeyValuePair<string, string> entry in replacedCameraEventCacahe)
        {
            //Debug.Log(e.DeviceIds.Length);

            foreach (var device in e.DeviceIds)
            {
                //Debug.Log(device);

                if (device == entry.Key)
                {
                    var defaultDevice = demoEventCopy.Find(x => x.CameraId == device);
                    //Debug.Log("Key " + entry.Key + " Value " + defaultDevice.Uri);
                    ReplaceSecurityCameraDemoUri(entry.Key, defaultDevice.Uri);
                    keysToRemove.Add(entry.Key);
                }
            }
        }

        // Remove the keys after iterating through the dictionary
        foreach (var key in keysToRemove)
        {
            replacedCameraEventCacahe.Remove(key);
        }
    }


    void PrintCachedCameras()
    {
        foreach (KeyValuePair<string, CameraEvent> entry in _cameraEventCache)
        {
            Debug.Log("Key  - " + entry.Key + " Value - " + entry.Value.Uri);
        }
    }


     void ReplaceSecurityCameraDemoUri(string cameraId, string newURI)
    {
        //Debug.Log(cameraId);
        if (_cameraEventCache.ContainsKey(cameraId))
        {
            //Debug.Log("found Camera feed replacing URL with new URI " + newURI);

            if (newURI != _cameraEventCache[cameraId].Uri)
            {
                //Debug.Log("New URL adding to cache");
                if (!replacedCameraEventCacahe.ContainsKey(cameraId))
                {
                    replacedCameraEventCacahe.Add(cameraId, _cameraEventCache[cameraId].Uri);
                }
            }
            //Debug.Log("Replacing URI with " + newURI);
            _cameraEventCache[cameraId].Uri = newURI;
        }

        //PrintCachedCameras();
    }

    public string GetSecurityCameraDemoUri(string cameraId)
    {
        Debug.Log(cameraId);
        if (_cameraEventCache.ContainsKey(cameraId))
        {
            Debug.Log("found Camera feed");
            return _cameraEventCache[cameraId].Uri;
        }
        else
        {
            return null;
        }
    }
}
