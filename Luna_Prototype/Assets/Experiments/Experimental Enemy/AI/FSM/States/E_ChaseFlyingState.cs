﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_ChaseFlyingState : E_State
{
    E_Enemy mAgent;
    Player mPlayer;

    Collider2D mPlayerCollider;

    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponentInParent<E_Enemy>();

    }

    public override void Enter()
    {
        mPlayer = mAgent.mZone.mPlayerTransform.GetComponent<Player>();
        mPlayerCollider = mPlayer.GetComponent<Collider2D>();

        mAgent.mSteering.TurnAllOff();
        mAgent.mSteering.SetActive(SteeringType.Arrive, true);

        mAgent.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;

        mAgent.mAgent.mTarget = mPlayer.transform.position;
    }

    public override void MyUpdate()
    {
        if (mAgent.mAttackRange.IsTouching(mPlayerCollider))
        {
            mAgent.mStateMachine.ChangeState("Attack");
            return;
        }
        if (!mAgent.mPlayerVisibilityRange.IsTouching(mPlayerCollider))
        {
            mAgent.mStateMachine.ChangeState("Wander");
            return;
        }
        mAgent.mAgent.mTarget = mPlayer.transform.position;
        mAgent.mAgent.mTarget.y += mAgent.mNodeRange.radius;
    }

    public override void Exit()
    {
    }

    public override string GetName()
    {
        return "Chase";
    }

}
