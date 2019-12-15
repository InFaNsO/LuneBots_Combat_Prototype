﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBot : Enemy
{
    LAI.States mCurrentState = LAI.States.None;
    [SerializeField] LAI.WanderGroundState stateWander = new LAI.WanderGroundState();

    private new void Awake()
    {
        base.Awake();
        mSteeringModule.AddState<LAI.BehaviourSeek>();
        mSteeringModule.SetActive(LAI.SteeringType.Seek, true);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        mStateMachine.SetAgent(this);
        mStateMachine.AddState<LAI.AttackState>();
        mStateMachine.AddState<LAI.SupriseState>();
        mStateMachine.AddState<LAI.GoToPlayerState>();
        mStateMachine.AddState<LAI.WanderGroundState>(stateWander);
        mCurrentState = LAI.States.Wander;
        mStateMachine.ChangeState((int)mCurrentState);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        //state machine update will handle switching state
    }
}
