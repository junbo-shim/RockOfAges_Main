using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BuildHighLight : MonoBehaviour
{
    Dictionary<string, GameObject> highLight;
    GameObject currHighLight = null;
    // Start is called before the first frame update

    void Awake()
    {
        highLight = new Dictionary<string, GameObject>();
        foreach(var child in GetComponentsInChildren<MeshRenderer>())
        {
            highLight.Add(child.name, child.gameObject);
            child.gameObject.SetActive(false);
        }

    }

    //모든 highlight비활성화
    private void HideAllHighLight()
    {
        foreach (var child in highLight)
        {
            child.Value.SetActive(false);
        }
    }


    //하이라이트 변경(상위에서 변경한다.(BuildViewer))
    public void ChangeHighLight(Vector2 size)
    {
        string key = "Highlight_" + size.x + "X" + size.y;
        ChangeHighLight(key);
    }
    public void ChangeHighLight(HighLightSize size)
    {
        string key = size.ToString();
        ChangeHighLight(key);
    }
    public void ChangeHighLight(string highLightName)
    {
        HideCurrHighLight();
        if (!highLight.ContainsKey(highLightName))
        {
            Debug.LogWarning(highLightName);
            Debug.Assert(false, "사이즈에 맞는 크기의 highlight가 없음");
            return;
        }
        currHighLight = highLight[highLightName];
        ShowCurrHighLight();
    }



    public void ShowCurrHighLight()
    {
        if (currHighLight != null)
        {
            currHighLight.SetActive(true);
        }
    }
    public void HideCurrHighLight()
    {
        if (currHighLight != null)
        {
            currHighLight.SetActive(false);
        }
    }
}

public enum HighLightSize
{
    Highlight_1X1 = 1,
    Highlight_2X2 = 2,
    Highlight_3X3 = 3
}
