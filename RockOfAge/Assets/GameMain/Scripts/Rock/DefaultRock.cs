using System.Collections.Generic;
using UnityEngine;

public class DefaultRock : RockBase
{
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (isDestroy)
        {
            return;
        }

        if (isFall)
        {
            return;
        }

        if (playerInput.magnitude > 0)
        {
            Move(playerInput);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (isFall || isDestroy)
        {
            return;
        }

        GetInput();
        ChangeRockState();
        CheckGround();
        ChangeDrag();
        CheckFall();


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump(DEFAULT_JUMP_FORCE);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (IsMove(2))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles")) 
            {
                Attack(collision);
            }
            if (IsMove(10))
            {
                Hit(10);
            }
        }
    }

}