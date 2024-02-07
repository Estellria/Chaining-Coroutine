using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;

namespace StellaFox
{
    public interface IRoutine
    {
        public IEnumerator Execute();
        //public IEnumerator Stop();
    }

    public class RoutineProcessor : IRoutine
    {
        public RoutineProcessor(Func<RoutineInfo, bool> condition, RoutineInfo info)
        {
            _exitCondition = condition;
            _routineInfo = info;
        }


        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _exitCondition;

        //stack은 데이터를 유지하려면 여러가지 귀찮
        private List<IRoutine> _routineList = new List<IRoutine>(); 


        public virtual IEnumerator Execute()
        {
            while (_exitCondition.Invoke(_routineInfo))
            {
                foreach (IRoutine routiner in _routineList)
                {
                    yield return CoroutineHelper.StartCoroutine(routiner.Execute());
                }
            }
        }

        public void Bind(IRoutine routine, int repeatLevel)
        {
            var tempList = _routineList; 
            tempList.Add(routine);
        }
    }

    public class RoutineExecutor : RoutineProcessor
    {
        public RoutineExecutor() : base (null, null) { }

        private List<IRoutine> _routineList = new List<IRoutine>();


        //런타임 실행
        public override IEnumerator Execute()
        {
            foreach (IRoutine routiner in _routineList)
            {
                yield return CoroutineHelper.StartCoroutine(routiner.Execute());
            }
        }
    }

    public class Routine : IRoutine
    {
        public Routine(Action<RoutineInfo> action, RoutineInfo info, Action callback)
        {
            _action = action;
            _callback = callback;
            _routineInfo = info;
        }

        private Action _callback;
        private Action<RoutineInfo> _action;
        private RoutineInfo _routineInfo;

        public IEnumerator Execute()
        {
            _action.Invoke(_routineInfo);
            _callback.Invoke();
            yield break;
        }
    }


    public class WaitRoutines : IRoutine
    {
        private Coroutine[] _coroutines;

        public WaitRoutines(Coroutine[] coroutines)
        {
            _coroutines = coroutines;
        }

        public IEnumerator Execute()
        {
            foreach (var routine in _coroutines)
            {
                yield return routine;
            }
        }
    }


    public class DelayRoutine : IRoutine
    {
        private YieldInstruction _yieldInstruction;

        public DelayRoutine(YieldInstruction yieldInstruction)
        {
            _yieldInstruction = yieldInstruction;
        }

        public IEnumerator Execute()
        {
            yield return _yieldInstruction;
        }
    }


    public class CustomDelayRoutine : IRoutine
    {
        private ChainYieldInstructure _yieldInstruction;

        public CustomDelayRoutine(ChainYieldInstructure yieldInstruction)
        {
            _yieldInstruction = yieldInstruction;
        }

        public IEnumerator Execute()
        {
            yield return _yieldInstruction;
        }
    }
}
