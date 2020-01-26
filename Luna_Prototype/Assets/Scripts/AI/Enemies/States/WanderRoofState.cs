﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAI
{
    [System.Serializable]
    public class WanderRoofState : State
    {
        [SerializeField] Platform roof = new Platform();

        Vector3 left = new Vector3();
        Vector3 right = new Vector3();

        float yPrv = 0.0f;

        bool isGoingLeft = false;
        bool shouldFall = false;

        public override States Name()
        {
            return States.Wander;
        }

        public override void Enter(Enemy agent)
        {
            agent.GetComponent<SpriteRenderer>().color = Color.green;
            shouldFall = false;
            agent.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), 180.0f);

            agent.GetComponent<Rigidbody2D>().gravityScale = -1.0f;
            left = roof.transform.position;
            right = roof.transform.position;

            left.x -= roof.Width * 0.5f;
            right.x += roof.Width * 0.5f;

            left.y -= roof.Height * 0.5f;
            right.y -= roof.Height * 0.5f;

            float distanceLeft = Vector3.Distance(agent.transform.position, left);
            float distanceRight = Vector3.Distance(agent.transform.position, right);

            if(distanceLeft > distanceRight)
            {
                isGoingLeft = false;
                agent.SetDestination(right);
            }
            else
            {
                isGoingLeft = true;
                agent.SetDestination(left);
            }
        }

        public override void Update(Enemy agent)
        {
            if(shouldFall)
            {
                //check if its grounded
                if(agent.transform.position.y > yPrv - 0.01f && agent.transform.position.y < yPrv + 0.01f)
                {
                    agent.mStateMachine.ChangeState(States.GoToPlayer);
                    return;
                }
                else
                {
                    yPrv = agent.transform.position.y;
                    return;
                }
            }

            float findRange = agent.GetSafeDistanceExtended(); 
            if (agent.IsNearPlayer(findRange) && agent.GetWorld().HasLineOfSight(new World.Wall(agent.transform.position, agent.GetWorld().mPlayer.transform.position)))
            {
                agent.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                yPrv = agent.transform.position.y;
                shouldFall = true;
                agent.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), 180.0f);
                return;
            }

            bool isClose = agent.IsNearDestination(agent.GetSafeDistanceReduced());

            if (isClose)
            {
                if (isGoingLeft)
                {
                    agent.SetDestination(right);
                    isGoingLeft = false;
                }
                else
                {
                    agent.SetDestination(left);
                    isGoingLeft = true;
                }
            }
        }

        public override void Exit(Enemy agent)
        {
            agent.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }
    }
}