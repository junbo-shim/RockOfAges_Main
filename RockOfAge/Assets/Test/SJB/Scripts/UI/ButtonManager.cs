using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Button optionButton;
    private Button startButton;
    private Button quitButton;
    
    private void Awake()
    {
        optionButton = GameObject.Find("Button_Option").GetComponent<Button>();
        startButton = GameObject.Find("Button_Start").GetComponent<Button>();
        quitButton = GameObject.Find("Button_Quit").GetComponent<Button>();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }
}