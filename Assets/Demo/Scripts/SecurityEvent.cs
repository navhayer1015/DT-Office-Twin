using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SecurityEvent
{
    public int Time;
    public DateTime TimeOfEvent;
    public string Id;
    public List<string> DeviceIds;
    public string EventType;
}

[Serializable]
public class SecurityDemoEvent
{
    public bool isCamera;
    public int Time;
    public DateTime TimeOfEvent;
    public string AlarmId;
    public string CameraId;
    public string EventType;
    public string Uri;
}

[Serializable]
public class SequentialDemoSteps
{
    public List<SecurityDemoEvent> SecurityEvents;
}