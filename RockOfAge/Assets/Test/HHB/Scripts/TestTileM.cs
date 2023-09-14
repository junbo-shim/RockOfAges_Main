using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTileM : MonoBehaviour
{
    private void Awake()
    {
        ResourceManager.Instance.CreateThisManager();
    }
}
