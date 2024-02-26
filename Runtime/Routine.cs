using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CCoroutine
{
    public class RoutineInfo
    {
        private int _routineCallingCount;
        private Stopwatch _stopwatch = new Stopwatch();


        public float elapsedTime
        {
            get { return _stopwatch.ElapsedMilliseconds / 1000f; }
        }

        public int CalledCount
        {
            get { return _routineCallingCount; }
        }

        public void AddCallCount()
        {
            _routineCallingCount++;
        }

        public void Play()
        {
            _stopwatch.Start();
            _routineCallingCount = 0;
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }
    }


    public class Routine : IRoutine
    {
        public Routine(Action<RoutineInfo> action, RoutineInfo info, ReferenceAction onCallbackNext)
        {
            _action = action;
            _routineInfo = info;
            _onCallbackNext = onCallbackNext;
        }

        public eRoutineType GetRoutineType 
        {
            get { return eRoutineType.Immediately; }
        }

        private RoutineInfo _routineInfo;
        private Action<RoutineInfo> _action;
        private ReferenceAction _onCallbackNext;

        public IEnumerator Execute()
        {
            _action.Invoke(_routineInfo);
            _onCallbackNext.Invoke();
            return null;
        }
    }


    public class WaitRoutine : IRoutine
    {
        public WaitRoutine(Coroutine[] coroutines, ReferenceAction onCallbackNext)
        {
            _coroutines = coroutines;
            _onCallbackNext = onCallbackNext;
        }

        public eRoutineType GetRoutineType => eRoutineType.Delay;

        private ReferenceAction _onCallbackNext;
        private Coroutine[] _coroutines;


        public IEnumerator Execute()
        {
            foreach (var routine in _coroutines)
            {
                yield return routine;
                _onCallbackNext.Invoke();
            }
        }
    }


    public class DelayRoutine<T> : IRoutine where T : class
    {
        public DelayRoutine(Func<T> yieldDelay, ReferenceAction onCallbackNext, bool waitAFrame = false)
        {
            _yieldDelay = yieldDelay;
            _additionalWait = waitAFrame;
            _onCallbackNext = onCallbackNext;
        }

        public eRoutineType GetRoutineType 
        {
            get { return eRoutineType.Delay; }
        }

        private bool _additionalWait;
        private Func<T> _yieldDelay;
        private ReferenceAction _onCallbackNext;

        public IEnumerator Execute()
        {
            var yield = _yieldDelay.Invoke();
            if (yield != null)
            {
                yield return yield;
            }

            //이렇게 한 번 쉬어주지 않으면 WaitForCompletion 쓸때 두번씩 호출되는데, 왜 그런지 모르겠다.
            if (_additionalWait)
            {
                yield return null;
            }

            _onCallbackNext.Invoke();
        }
    }


    public class IfRoutine : IRoutine
    {
        public IfRoutine(YieldInstruction wait, Func<RoutineInfo, bool> yieldDelay, RoutineInfo Info, ReferenceAction onCallbackNext)
        {
            _wait = wait;
            _routineInfo = Info;
            _yieldConidtion = yieldDelay;
            _onCallbackNext = onCallbackNext;
        }

        public eRoutineType GetRoutineType
        {
            get
            {
                bool canWait = _yieldConidtion.Invoke(_routineInfo);
                return canWait ? eRoutineType.Delay : eRoutineType.Immediately;
            }
        }

        private YieldInstruction _wait;
        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _yieldConidtion;
        private ReferenceAction _onCallbackNext;


        public IEnumerator Execute()
        {
            if (_yieldConidtion.Invoke(_routineInfo))
            {
                yield return _wait;
            }

            _onCallbackNext.Invoke();
        }
    }
}
