﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseFlyingState : State
{
    Enemy mAgent;
    Player mPlayer;

    Collider2D mPlayerCollider;

    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponentInParent<Enemy>();

    }

    public override void Enter()
    {
        mPlayer = mAgent.mZone.mPlayerTransform.GetComponent<Player>();
        mPlayerCollider = mPlayer.GetComponent<Collider2D>();

        mAgent.mSteering.TurnAllOff();
        mAgent.mSteering.SetActive(SteeringType.Arrive, true);
        mAgent.mSteering.SetActive(SteeringType.Seek, true);

      //  mAgent.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;

        mAgent.mAgent.mTarget = mPlayer.transform.position;
    }

    public override void MyUpdate()
    {
        if (!mAgent.myHealth.IsAlive())
        {
            mAgent.mStateMachine.ChangeState(EnemyStates.Die.ToString());
            return;
        }
        if (Vector3.Distance(mAgent.transform.position, mPlayer.transform.position) <= mAgent.mAttackRange.radius)
        {
            mAgent.mStateMachine.ChangeState(EnemyStates.Attack.ToString());
            return;
        }
        if (Vector3.Distance(mAgent.transform.position, mPlayer.transform.position) > mAgent.mPlayerVisibilityRange.radius)
        {
            mAgent.mStateMachine.ChangeState(EnemyStates.Wander.ToString());
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
        return EnemyStates.Chase.ToString();
    }

}
