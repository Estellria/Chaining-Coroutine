using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System;

namespace CCoroutine
{
    public class WaitForCompletion : CustomYieldInstruction
    {
        public WaitForCompletion(Func<RoutineInfo, bool> waitingCondition, RoutineInfo routineInfo)
        {
            _waitingCondition = waitingCondition;
            _routineInfo = routineInfo;
        }

        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _waitingCondition;

        public override bool keepWaiting => !_waitingCondition.Invoke(_routineInfo);
    }
}
