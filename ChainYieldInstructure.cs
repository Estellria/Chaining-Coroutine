using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace StellaFox
{
    public class ChainYieldInstructure : CustomYieldInstruction
    {
        public ChainYieldInstructure(Func<RoutineInfo, bool> waitingCondition, RoutineInfo routineInfo)
        {
            _waitingCondition = waitingCondition;
        }

        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _waitingCondition;

        public override bool keepWaiting => _waitingCondition.Invoke(_routineInfo);
    }
}
