using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global_PSC
{

    public static int FindLayerToName(string layerName)
    {

        int layerIndex = LayerMask.NameToLayer(layerName);

        if (layerIndex == -1)
        {
            Debug.LogWarning("레이어 " + layerName + "를 찾을 수 없습니다.");
            return layerIndex;
        }
        
        return 1 << layerIndex;
    }

    public static Vector3 GetWorldMousePositionFromMainCamera(float depth)
    {
        Vector3 mouseCurrPos = Input.mousePosition;
        mouseCurrPos.z = depth;
        return Camera.main.ScreenToWorldPoint(mouseCurrPos);

    }
}
