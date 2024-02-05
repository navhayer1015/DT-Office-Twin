using System;
using System.Collections;
using System.Collections.Generic;
using IotDevices;
using UnityEngine;

public class LayerManager : SceneSingleton<LayerManager>
{
    private const string ViewShedLayer = "Camera Views";
    private const string WallsLayer = "Walls";
    private const string FurnitureLayer = "Furniture";

    public List<string> Layers
    {
        get
        {
            return new List<string>()
            {
                ViewShedLayer,
                WallsLayer,
                FurnitureLayer
            };
        }
    }

    public GameObject WallsRoot;
    public GameObject FurnitureRoot;

    public delegate void OnLayerVisibilityChange(string layerName, bool show);

    public OnLayerVisibilityChange LayerVisibilityChange;

    public void ShowHideLayer(string showHideLayerName, bool showLayer)
    {
            switch (showHideLayerName)
            {
                case ViewShedLayer:
                    SetAllViewShedsVisible(showLayer);
                    break;
                case WallsLayer:
                    SetllAllWallsVisible(showLayer);
                    break;
                case FurnitureLayer:
                    SerAllFurnitureVisibile(showLayer);
                    break;
            }
    }

    public void SetAllLayersVisible(bool isVisible)
    {
        foreach (var layer in Layers)
        {
            ShowHideLayer(layer, isVisible);
        }
    }

    public void SetllAllWallsVisible(bool newValue)
    {
        WallsRoot.SetActive(newValue);
        LayerVisibilityChange?.Invoke(WallsLayer, newValue);
    }
    public void SerAllFurnitureVisibile(bool newValue)
    {
        FurnitureRoot.SetActive(newValue);
        LayerVisibilityChange?.Invoke(FurnitureLayer, newValue);
    }

    public void SetAllViewShedsVisible(bool newValue)
    {
        foreach (var device in IotDeviceManager.Instance.Devices.Values)
        {
            var cameraViewshed = device?.GetComponentInChildren<DeviceMarker>()?.meshEffect;
            if (cameraViewshed != null)
            {
                cameraViewshed.SetActive(newValue);
            }
            var cameraViewShedOutline = device?.GetComponentInChildren<DeviceMarker>()?.meshEffectOutline;
            if (cameraViewShedOutline != null)
            {
                cameraViewShedOutline.SetActive(newValue);
            }   
        }
        LayerVisibilityChange?.Invoke(ViewShedLayer, newValue);
    }
}
