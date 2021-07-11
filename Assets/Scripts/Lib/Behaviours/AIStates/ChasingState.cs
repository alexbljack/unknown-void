using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lib.Behaviours.AIStates
{
    public class ChasingState : State<AIRoaming>
    {
        public ChasingState(AIRoaming owner) : base(owner) {}

        List<Vector3> _path;
        int _pointIndex;
        bool _shouldRebuildPath = true;

        public override void Enter()
        {
            owner.SetStatusIcon(owner.ChaseIcon);
        }

        public override void Update()
        {
            if (owner.TargetEscaped && owner.CanChangeState)
            {
                owner.Roam();
            }
            
            if (_shouldRebuildPath)
            {
                _path = owner.BuildPath(owner.ChaseTarget.transform.position);
                _path.RemoveAt(0);
                _pointIndex = 0;
                owner.StartCoroutine(RebuildDelayRoutine());
            }
            
            if ((owner.transform.position - _path[_pointIndex]).magnitude < 0.1f)
            {
                _pointIndex += 1;
            }
            
            owner.MoveToPoint(_path[_pointIndex], owner.ChaseSpeed);
        }

        IEnumerator RebuildDelayRoutine()
        {
            _shouldRebuildPath = false;
            yield return new WaitForSeconds(1f);
            _shouldRebuildPath = true;
        }
    }
}