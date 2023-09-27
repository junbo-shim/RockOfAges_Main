using PlayFab.ClientModels;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    public Image hpImg;

    // 돌깎는 애니메이션
    public GameObject creators;
    public GameObject myRock;
    public Image startRock;
    public GameObject rockReadyInfo;

    public void SwitchRockReadyInfo()
    {
        if (rockReadyInfo.gameObject.activeSelf)
        {
            rockReadyInfo.gameObject.SetActive(false);
        }
        else { rockReadyInfo.gameObject.SetActive(true); }
    }

    public void PrintFillAmountRockHp(float currentHp, float maxHp)
    {
        hpImg.fillAmount = (currentHp / maxHp) * 0.55f;
    }

    public void FillAmountRockRoutine(float creatTime)
    {
        StartCoroutine(FillAmountRock(creatTime));
        StartCoroutine(MoveCreators(creatTime));
        StartCoroutine(PoolingRockFragments(creatTime));
    }

    IEnumerator FillAmountRock(float creatTime)
    {
        float myTime = 0f;
        while (myTime < creatTime)
        {
            myTime += Time.deltaTime;
            startRock.fillAmount = (creatTime - myTime) / creatTime;
            yield return null;
        }
    }

    IEnumerator MoveCreators(float creatTime)
    {
        RectTransform rectTransform = creators.GetComponent<RectTransform>();
        //Vector2 startPos = rectTransform.anchoredPosition;
        if (creators.transform.localScale != Vector3.one)
        { 
            creators.transform.localScale = Vector3.one;
        }

        Transform creator_L = creators.transform.GetChild(0);
        Transform creator_R = creators.transform.GetChild(1);

        Animator creator_L_Ani = creator_L.GetComponent<Animator>();
        Animator creator_R_Ani = creator_R.GetComponent<Animator>();

        creator_L_Ani.SetBool("isCreating", true);
        creator_R_Ani.SetBool("isCreating", true);

        float currentY = rectTransform.anchoredPosition.y;
        float targetY = -120f;
        float myTime = 0f;

        while(myTime < creatTime)
        { 
            float alpha = myTime / creatTime;
            float realTimeY = Mathf.Lerp(currentY, targetY, alpha);

            Vector2 nowPos = rectTransform.anchoredPosition;
            nowPos.y = realTimeY;
            creators.transform.localPosition = nowPos;

            myTime += Time.deltaTime;
            yield return null;

        }

        creator_L_Ani.SetBool("isCreating", false);
        creator_R_Ani.SetBool("isCreating", false);
        creators.transform.localScale = Vector3.zero;
    }

    public IEnumerator PoolingRockFragments(float creatTime)
    {
        float myTime = 0f;
        float spawnTime = 0.3f;
        while (myTime < creatTime)
        { 
            GameObject rock = RockObjectPooling.objectPooling.GetRock();
            FragmentControl fragmentControl = rock.GetComponent<FragmentControl>();
            fragmentControl.PackFragmentMovement();
            yield return new WaitForSeconds(spawnTime);
            RockObjectPooling.objectPooling.ReturnRockWithTimer(rock);
            myTime += spawnTime;
        }
        SwitchRockReadyInfo();
    }
}
