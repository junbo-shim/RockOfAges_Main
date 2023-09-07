using UnityEngine;
using UnityEngine.UI;

public class Buttons_Title : MonoBehaviour
{
    public bool isStartButton;
    public bool isoptionButton;

    private Transform startButton;
    private Transform optionButton;

    private void Awake()
    {
        startButton = gameObject.transform.Find("Button_Start");
        optionButton = gameObject.transform.Find("Button_Option"); 
    }

    private void OnEnable()
    {
        

    }

    private void OnDisable()
    {
        
    }
}