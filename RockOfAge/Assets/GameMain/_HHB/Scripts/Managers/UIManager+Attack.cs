using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    public Image hpImg;

    public void PrintFillAmountRockHp(float currentHp, float maxHp)
    {
        hpImg.fillAmount = (currentHp / maxHp) * 0.55f;
    }
}
