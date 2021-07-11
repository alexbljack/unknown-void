using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lib.Behaviours.AIStates
{
    public class RoamingState : State<AIRoaming>
    {
        public RoamingState(AIRoaming owner) : base(owner) {}

        Coroutine _roamRoutine;
        Vector2 _roamCenter;

        public override void Enter()
        {
            _roamCenter = owner.transform.position;
            _roamRoutine = owner.StartCoroutine(RoamRoutine());
            owner.SetStatusIcon(owner.RoamIcon);
        }

        public override void Update()
        {
            DebugDrawRoamArea();
            CheckIfDetectedTarget();
        }

        public override void Exit()
        {
            owner.StopCoroutine(_roamRoutine);
            _roamRoutine = null;
        }

        void CheckIfDetectedTarget()
        {
            if (owner.IsDetectedTarget && owner.CanChangeState)
            {
                owner.Chase();
            }
        }

        IEnumerator RoamRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(owner.RoamDelay);
                var path = owner.BuildPath(owner.GetNewRoamPoint(_roamCenter));
                foreach (Vector3 point in path)
                {
                    while ((owner.transform.position - point).magnitude > owner.PathfindStep + 0.01f)
                    {
                        owner.MoveToPoint(point, owner.RoamSpeed);
                        yield return null;
                    }
                }
            }
        }

        void DebugDrawRoamArea()
        {
            Helpers.DebugDrawCircle(_roamCenter, owner.RoamRadius, 30, Color.blue);
        }
    }
}