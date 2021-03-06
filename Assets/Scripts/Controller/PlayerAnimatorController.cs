﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    enum PlayerAnimationState
    {
        PLAYER_IDLE = 0,
        PLAYER_RUN = 1
    }

    PlayerController mPlayerController;
    ParryAttackable mParryAttackableObj;
    Animator mAnimator;
    Rigidbody2D rb;
    float mLastYVel = 0.0f;
    float mLastAccelration = 0.0f;

    PlayerAnimationState mCurrentState = PlayerAnimationState.PLAYER_IDLE;
    void Awake()
    {
        mPlayerController = GetComponent<PlayerController>();
        mParryAttackableObj = GetComponent<ParryAttackable>();
        mAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        mCurrentState = mPlayerController.IsMoving() ? PlayerAnimationState.PLAYER_RUN : PlayerAnimationState.PLAYER_IDLE;

        mAnimator.SetInteger("playerState", (int)mCurrentState);

        if(mPlayerController.IsDashing())
            mAnimator.SetBool("IsDashing", true);

        if (mParryAttackableObj.IsParrying())
        {
            mAnimator.SetBool("IsParrying", true);
            if (mParryAttackableObj.IsStartParrying())
                mAnimator.SetTrigger("StartParry");
        }
        else
            mAnimator.SetBool("IsParrying", false);

        if (mParryAttackableObj.IsParryHit())
        {
            mAnimator.SetTrigger("ParryHit");
        }
    }

    private void FixedUpdate()
    {
        float currentVelY = rb.velocity.y;
        float currentAccelation = (currentVelY - mLastYVel);
       
        if (currentAccelation > 0.0f && currentVelY > 0.0f)
        {
            Core.Debug.Log(" Jump !!!!!!!!!!!!!!!!!");
            mAnimator.SetBool("IsDoubleJump", true);
        }
        mLastYVel = currentVelY;
        mLastAccelration = currentAccelation;
    }

    void LateUpdate()
    {
        if (mAnimator.gameObject.activeSelf)
        {
            mAnimator.SetInteger("playerState", (int)PlayerAnimationState.PLAYER_IDLE);
            mAnimator.SetBool("IsOnGround", mPlayerController.IsGrounded());
            mAnimator.SetBool("IsDoubleJump", false);
            mAnimator.SetBool("IsDashing", false);
            //mAnimator.SetBool("IsParrying", false);
        }
    }
}
