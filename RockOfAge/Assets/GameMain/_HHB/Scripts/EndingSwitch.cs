using UnityEngine;

public class EndingSwitch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            //Transform mother = transform.root;
            //if (mother.gameObject.name == "Team1")
            //{
            //    누구의 승리인지 판별해서CycleManager Enum Result 결정
            //    DefineWinner 이나
            //    DefineLoser 를 부르면됨
            //}
            //else if (mother.gameObject.name == "Team2")
            //{ 
            
            //}
            CycleManager.cycleManager.DefineWinner(); // 실험용
        }
    }
}
