using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitObjectHandler
{
    void Hit(int damage);
    void HitReaction();
}
