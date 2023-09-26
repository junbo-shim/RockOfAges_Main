using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FallText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textObject;
    [SerializeField]
    private Transform rockObject;
    [SerializeField]
    private string fallingText;

    private Queue<TMP_Text> textObjectList;

    
    // Start is called before the first frame update
    void Awake()
    {
        rockObject = transform.parent.Find("RockObject");
        textObjectList = new Queue<TMP_Text>();
    }
    
    public void ClearText()
    {
        int size = textObjectList.Count;
        for(int i = 0; i < size; i++)
        {
            Destroy(textObjectList.Dequeue().gameObject);
        }
    }

    public void StartFallText()
    {
        StartCoroutine(FallTextRoutine());
    }


    IEnumerator FallTextRoutine()
    {
        for(int i = 0; i < fallingText.Length; i++)
        {
            yield return new WaitForSeconds(.1f);
            TMP_Text text = Instantiate(textObject, rockObject.position, Quaternion.identity, transform);
            text.transform.rotation = Camera.main.transform.rotation;   
            text.text = fallingText[i].ToString();
            textObjectList.Enqueue(text);
        }
    }
}