using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacleBase : ObstacleBase
{
    //해당 클래스는 움직이는 obstacle에 상속 바랍니다.

    //움직이기 때문에 움직인 후에 타겟이 없으면 복귀해야 한다.
    protected Vector3 standPosition = Vector3.zero;

    //이동
    public virtual void Move(){}

    //복귀
    public virtual void Return(){}
}
