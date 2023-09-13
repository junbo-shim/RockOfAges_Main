using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attack UI
public partial class UIManager : MonoBehaviour
{
    public GameObject attackUI;

    public void PrintAttackUI()
    {
        attackUI.transform.localScale = Vector3.one;
    }

    public void TurnOffAttackUI()
    {
        attackUI.transform.localScale = Vector3.one * 0.001f;
    }
}
