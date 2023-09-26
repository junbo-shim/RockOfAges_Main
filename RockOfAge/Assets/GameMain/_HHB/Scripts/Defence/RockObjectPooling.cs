using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObjectPooling : MonoBehaviour
{
    public static RockObjectPooling objectPooling;

    [SerializeField]
    private GameObject[] rockObj;


    Queue<GameObject> rocksQueue = new Queue<GameObject>();

    private void Awake()
    {
        objectPooling = this;
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                rocksQueue.Enqueue(CreateRock(j));
            }
        }
    }

    public void SetTransformUI(GameObject rock)
    { 
        GameObject mother = Global_PSC.FindTopLevelGameObject("DefenceUI");
        Transform container = mother.transform.GetChild(0).GetChild(2);
        rock.transform.SetParent(container.transform);
    }

    public void SetTransformPooling(GameObject rock)
    {
        GameObject mother = Global_PSC.FindTopLevelGameObject("PoolingObjectQueue");
        rock.transform.SetParent(mother.transform);
    }


    private GameObject CreateRock(int index)
    {
        var rock = Instantiate(rockObj[index]);
        SetTransformPooling(rock);
        rock.gameObject.SetActive(false);
        return rock;
    }

    public GameObject GetRock()
    {
        if (objectPooling.rocksQueue.Count > 0)
        {
            var rock = objectPooling.rocksQueue.Dequeue();
            SetTransformUI(rock);
            rock.gameObject.SetActive(true);
            return rock;
        }
        else
        {
            int rndIdx = Random.Range(0, 4);
            var newRock = CreateRock(rndIdx);
            newRock.gameObject.SetActive(true);
            SetTransformUI(newRock);
            return newRock;
        }
    }

    public void ReturnRockWithTimer(GameObject obj)
    {
        StartCoroutine(ReturnTime(obj));
    }

    IEnumerator ReturnTime(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f);
        obj.gameObject.SetActive(false);
        SetTransformPooling(obj);
        rocksQueue.Enqueue(obj);
    }

    public bool LimitRockCount()
    {
        GameObject mother = Global_PSC.FindTopLevelGameObject("DefenceUI");
        Transform container = mother.transform.GetChild(0).GetChild(2);
        int fragmentCount = container.childCount;
        if (fragmentCount > 20)
        {
            return false;
        }
        else { return true; }
    }
}
