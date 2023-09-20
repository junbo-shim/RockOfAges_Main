using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitHolderButton : RockHolderButton
{ 
    //{ PackOnPointerClick()
    public override void PackOnPointerClick()
    {
        ItemManager.itemManager.unitSelected.Remove(id);
        ItemManager.itemManager.UnitRePrintHolder();
        UnitButton[] unitButtons = FindObjectsOfType<UnitButton>();
        foreach (UnitButton unitButton in unitButtons)
        {
            if (unitButton.id == id)
            {
                unitButton.BackToOriginalColor(id);
            }
        }
    }
    //} PackOnPointerClick()
}
