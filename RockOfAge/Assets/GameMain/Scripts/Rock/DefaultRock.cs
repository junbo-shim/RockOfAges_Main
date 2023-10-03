using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultRock : RockBase
{
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        // ! PSC Editted
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
        // ! PSC Editted
        if (!photonView.IsMine)
        {
            return;
        }

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("PscTestScene"))
        {
            CameraManager.Instance.SetCameraMotionBlur(gameObject);
        }

        if (isFall || isDestroy)
        {
            return;
        }

        ChangeRockState();
        CheckGround();
        ChangeDrag();
        CheckFall();
        PlayMoveSound();

        //{ 0930 홍한범 조건 추가
        if (CycleManager.cycleManager == null || CycleManager.cycleManager._isESCed == false)
        {
            GetInput();
            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                Jump(DEFAULT_JUMP_FORCE);
            }


            if (Input.GetKeyUp(KeyCode.R))
            {
                isFall = true;
                StartCoroutine(ComebackCheckPointRoutine());
            }
        }
        //} 0930 홍한범 조건 추가
    }


    /*private void OnCollisionEnter(Collision collision)
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
    }*/

}