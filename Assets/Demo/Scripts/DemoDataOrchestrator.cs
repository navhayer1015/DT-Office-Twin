using System.Collections.Generic;
using IotDevices;
using UnityEngine;

public class DemoDataOrchestrator : SceneSingleton<DemoDataOrchestrator>
{
    [SerializeField]
    private AlarmDemoData _alarmDemoData;

    [SerializeField]
    private CameraDemoData _cameraDemoData;


    private const string GENERATE_EVENTS = "GenerateEvents";

    private int _elapsedSeconds;
    IList<AlarmEvent> _alarmEvents;
    IList<CameraEvent> _cameraEvents;


    IList<IotAlarmDto> _iotAlarmEventCache = new List<IotAlarmDto>();

    IDictionary<string, CameraEvent> _cameraEventCache = new Dictionary<string, CameraEvent>();


    public void StartGeneratingData()
    {
        _alarmEvents = new List<AlarmEvent>(_alarmDemoData.AlarmEvents);
        _cameraEvents = new List<CameraEvent>(_cameraDemoData.CameraEvents);

        Debug.Log(
            $"DemoDataOrchestrator start  Alarm Events: {_alarmEvents.Count}  Camera Events: {_cameraEvents.Count}");

        _elapsedSeconds = 0;
        InvokeRepeating(GENERATE_EVENTS, 0, 1);
    }

    private void OnDestroy()
    {
        CancelInvoke(GENERATE_EVENTS);
    }


    public IList<IotAlarmDto> GetNewIotAlarmEvents()
    {
        List<IotAlarmDto> newList = new List<IotAlarmDto>(_iotAlarmEventCache);
        _iotAlarmEventCache.Clear();
        return newList;
    }

    public string GetSecurityCameraDemoUri(string cameraId)
    {
        if (_cameraEventCache.ContainsKey(cameraId))
            return _cameraEventCache[cameraId].Uri;
        else
            return null;
    }


    private void GenerateEvents()
    {
        _elapsedSeconds++;


        for (int i = _alarmEvents.Count - 1; i > -1; i--)
        {
            if (_alarmEvents[i].Time < _elapsedSeconds)
            {
                _iotAlarmEventCache.Add(IotAlarmHelper.FromAlarmEvent(_alarmEvents[i]));
                _alarmEvents.Remove(_alarmEvents[i]);
            }
        }


        for (int i = _cameraEvents.Count - 1; i > -1; i--)
        {
            if (_cameraEvents[i].Time < _elapsedSeconds)
            {
                Debug.Log(
                    $"DemoDataOrchestrator GenerateEvents  add camera {_cameraEvents[i].Id}  uri: {_cameraEvents[i].Uri}");

                if (_cameraEventCache.ContainsKey(_cameraEvents[i].Id))
                    _cameraEventCache[_cameraEvents[i].Id] = _cameraEvents[i];
                else
                    _cameraEventCache.Add(_cameraEvents[i].Id, _cameraEvents[i]);

                _cameraEvents.Remove(_cameraEvents[i]);
            }
        }
    }
}