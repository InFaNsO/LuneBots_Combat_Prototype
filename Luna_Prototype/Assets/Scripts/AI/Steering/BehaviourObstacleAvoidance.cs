﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAI
{
    public class BehaviourObstacleAvoidance : SteeringBehaviourBase
    {
        private struct Circle
        {
            public Vector2 center;
            public float radius;
        }

        private struct Data
        {
            public float distance;
            public Vector2 point;
        }

        private float Dot(Vector2 a, Vector2 b) { return (a.x * b.x) + (a.y * b.y); }
        private float Sqr(float a) { return a * a; }
        private float Sqrt(float a) { return Mathf.Sqrt(a); }

        private Data GetDistanceWall(Vector2 pos, World.Wall wall)
        {
            Vector2 a = pos;
            Vector2 b = wall.from;
            Vector2 c = wall.to;
            float wallLength = Sqrt(Sqr(wall.to.x - wall.from.x) + Sqr(wall.to.y - wall.from.y));

            float area = Mathf.Abs((a.x * (b.y - c.y) + b.x * (a.y - c.y) + c.x * (a.y - b.y)) * 0.5f);
            float h = ((area * 2) / wallLength);

            Vector2 point, dir;

            float dis = Sqrt(Sqr(h) + (Sqr(wall.from.x - pos.x) + Sqr(wall.from.y - pos.y)));
            dir = new Vector2(wall.to.x - wall.from.x, wall.to.y - wall.from.y);
            dir.Normalize();
            dir *= dis;
            point = wall.from + dir;
            Data d;
            d.distance = h;
            d.point = point;

            return d;
        }

        public bool LineIntersectsCircle(Agent agent, Vector2 sight1, Vector2 sight2, World.Obstacle obstacle)
        {
            return (obstacle.center - sight1).magnitude <= obstacle.radius + agent.GetSafeDistance() * 0.5f ||
                (obstacle.center - sight2).magnitude <= obstacle.radius + agent.GetSafeDistance() * 0.5f;
        }

        public bool LineIntersectsWall(Agent agent, Vector2 sight1, Vector2 sight2, World.Wall wall)
        {
            Data d = GetDistanceWall(agent.GetPosition(), wall);
            Vector2 point = d.point;

            return (point - sight1).magnitude <= agent.GetSafeDistance() * 0.5f ||
                (point - sight2).magnitude <= agent.GetSafeDistance() * 0.5f;
        }

        World.Obstacle FindNearestObstacle(Agent agent, Vector2 direction)
        {
            if (agent.GetWorld().GetObstacles().Count == 0)
            {
                World.Obstacle tempC;
                tempC.radius = -1;
                tempC.center = new Vector2(-1.0f, -1.0f);
                return tempC;
            }

            World.Obstacle nearestCircle = agent.GetWorld().GetObstacles()[0];
            float nearestDistance = (nearestCircle.center - agent.GetPosition()).SqrMagnitude();
            foreach (World.Obstacle obstacle in agent.GetWorld().GetObstacles())
            {
                float distance = (obstacle.center - agent.GetPosition()).SqrMagnitude();
                if (distance < nearestDistance)
                {
                    Vector2 heading = obstacle.center - agent.GetPosition();

                    float dot = Dot(heading, direction);
                    if (dot > 0)
                    {
                        nearestCircle = obstacle;
                        nearestDistance = distance;
                    }
                }
            }
            return nearestCircle;
        }

        World.Wall FindNearestWall(Agent agent, Vector2 direction)
        {
            World.Wall nearestWall = agent.GetWorld().GetWalls()[0];
            Data d = GetDistanceWall(agent.GetPosition(), nearestWall);
            float nearestDistance = d.distance;
            foreach (World.Wall wall in agent.GetWorld().GetWalls())
            {
                Data it = GetDistanceWall(agent.GetPosition(), wall);
                if (it.distance < nearestDistance)
                {
                    Vector2 heading = it.point - agent.GetPosition();
                    float dot = Dot(heading, direction);
                    if (dot < 0.0f)
                    {
                        nearestWall = wall;
                        nearestDistance = it.distance;
                    }
                }
            }
            return nearestWall;
        }

        public override Vector2 Calculate(Agent agent)
        {
            Vector2 normVel = agent.GetVelocity().normalized;
            Vector2 sight1 = agent.GetPosition() + (normVel * agent.GetSafeDistance());
            Vector2 sight2 = agent.GetPosition() + (normVel * (agent.GetSafeDistance() * 0.5f));
            Vector2 desiredVelocity = new Vector2();

            World.Obstacle nearestCircle = FindNearestObstacle(agent, agent.GetHeading());
            World.Wall nearestWall = FindNearestWall(agent, agent.GetHeading());

            bool wall = LineIntersectsWall(agent, sight1, sight2, nearestWall);
            if (nearestCircle.center.x < 0 && nearestCircle.center.y < 0)
            {
                if (wall)
                {
                    Data dd = GetDistanceWall(agent.GetPosition(), nearestWall);
                    desiredVelocity = sight1 - dd.point;
                    desiredVelocity.Normalize(); ;
                    desiredVelocity *= agent.GetMaxSpeed();
                    desiredVelocity /= agent.GetMass();
                    return desiredVelocity;
                }
                else
                    return new Vector2();
            }

            bool cir = LineIntersectsCircle(agent, sight1, sight2, nearestCircle);
            Data d = new Data();
            if (wall)
            {
                d = GetDistanceWall(agent.GetPosition(), nearestWall);
            }
            if (cir && wall)
            {
                float distanceCircleS1 = (nearestCircle.center - sight1).magnitude;
                float distanceWallS1 = (d.point - sight1).magnitude;

                if (distanceCircleS1 < distanceWallS1)
                {
                    desiredVelocity = sight1 - nearestCircle.center;
                }
                else
                {
                    desiredVelocity = sight1 - d.point;
                }

                desiredVelocity.Normalize();
                desiredVelocity *= agent.GetMaxSpeed();
                desiredVelocity /= agent.GetMass();
                return desiredVelocity;

            }
            else if (cir)
            {
                desiredVelocity = sight1 - nearestCircle.center;
                desiredVelocity.Normalize();
                desiredVelocity *= agent.GetMaxSpeed();
                desiredVelocity = (desiredVelocity / agent.GetMass());
                return desiredVelocity;
            }
            else if (wall)
            {
                desiredVelocity = sight1 - d.point;
                desiredVelocity.Normalize();
                desiredVelocity *= agent.GetMaxSpeed();
                desiredVelocity /= agent.GetMass();
                return desiredVelocity;
            }

            return new Vector2();

        }

        public override SteeringType GetName()
        {
            return SteeringType.ObstacleAvoidance;
        }
    }

}