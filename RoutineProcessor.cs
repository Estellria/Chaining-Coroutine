using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellaFox
{
    public interface IRoutine
    {
        public IEnumerator Execute();
        //public IEnumerator Stop();
    }


    public class RoutineInfo
    {
        private float startTime = Time.unscaledTime;

        public float elapsedTime
        {
            get => Time.unscaledTime - startTime;
        }

        public int Count
        {
            get;
            set;
        }
    }

    public enum eRoutineMode
    {
        //조건이 참일때만 OnNextRoutine 호출.
        //조건이 거짓이여도 OnNextRoutine 호출.
    }


    [Serializable]
    public class RoutineProcessor : IRoutine
    {
        public RoutineProcessor(Func<RoutineInfo, bool> condition, RoutineInfo info)
        {
            _exitCondition = condition;
            _routineInfo = info;
        }


        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _exitCondition;

        protected List<IRoutine> _routineList = new List<IRoutine>(); 


        public virtual IEnumerator Execute()
        {
            while (_exitCondition.Invoke(_routineInfo))
            {
                foreach (IRoutine routine in _routineList)
                {
                    yield return CoroutineHelper.StartCoroutine(routine.Execute());
                }
            }
        }

        public void Bind(IRoutine newRoutine, int repeatLevel)
        {
            var tempList = _routineList; 

            for (int i = 0; i < repeatLevel; ++i)
            {
                var rp = tempList?.LastOrDefault(e => e is RoutineProcessor);
                tempList = (rp as RoutineProcessor)?._routineList;
            }

            tempList.Add(newRoutine);
        }
    }

    public class RoutineExecutor : RoutineProcessor
    {
        public RoutineExecutor() : base (null, null) { }

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
        private Action _callback;

        public WaitRoutines(Coroutine[] coroutines, Action callback)
        {
            _coroutines = coroutines;
            _callback = callback;
        }

        public IEnumerator Execute()
        {
            foreach (var routine in _coroutines)
            {
                yield return routine;
                _callback.Invoke();
            }
        }
    }


    public class DelayRoutine : IRoutine
    {
        private YieldInstruction _yieldInstruction;
        private Action _callback;

        public DelayRoutine(YieldInstruction yieldInstruction, Action callback)
        {
            _yieldInstruction = yieldInstruction;
            _callback = callback;
        }

        public IEnumerator Execute()
        {
            yield return _yieldInstruction;
            _callback.Invoke();
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


    public class CallbackMessageRoutine : IRoutine
    {
        private Action _callback;

        public CallbackMessageRoutine(Action callback)
        {
            _callback = callback;
        }

        public IEnumerator Execute()
        {
            _callback.Invoke();
            yield break;
        }
    }
}
