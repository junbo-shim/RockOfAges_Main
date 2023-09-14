using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacleBase : ObstacleBase
{
    protected Vector3 standPosition = Vector3.zero;

    public virtual void Move(){}

    public virtual void Return(){}
}
