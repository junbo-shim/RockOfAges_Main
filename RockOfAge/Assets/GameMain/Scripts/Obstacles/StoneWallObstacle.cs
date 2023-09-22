using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : HoldObstacleBase, IHitObjectHandler
{
    private GameObject colliderParts = default;
    private bool isAttacked = false;

    private void Start()
    {
        colliderParts = transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
    }

    private void OnEnable()
    {
        StartBuild(BUILD_TIME);
    }

    //맵에 Build
    public override ObstacleBase Build(Vector3 position, Quaternion rotate)
    {
        ObstacleBase obstacle = Instantiate(this, position, rotate);
        obstacle.transform.localScale = Vector3.one * .1f;

        return obstacle;
    }

    void ChangePhase()
    {
        if (status.Health * 0f == currHealth)
        {

        } 
        else if (status.Health * 0.5 > currHealth)
        {
            
        } 
        else if (status.Health * 1 == currHealth)
        {

        }
}

    public void Hit(int damage)
    {
        if (!isAttacked)
        {
            return;
        }
    }

    public void HitReaction()
    {
        throw new System.NotImplementedException();
    }
}
