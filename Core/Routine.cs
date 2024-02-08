using StellaFox;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StellaFox
{
    public class RoutineInfo
    {
        private int _routineCallingCount;
        private float _startTime;


        public float elapsedTime
        {
            get { return Time.time - _startTime; }
        }

        public int CallingCount
        {
            get { return _routineCallingCount; }
        }

        public void AddCallCount()
        {
            _routineCallingCount++;
        }

        public void Initialize()
        {
            _startTime = Time.time;
            _routineCallingCount = 0;
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
            _action?.Invoke(_routineInfo);
            _callback?.Invoke();

            return null;
        }
    }


    public class WaitRoutine : IRoutine
    {
        private Coroutine[] _coroutines;
        private Action _callback;

        public WaitRoutine(Coroutine[] coroutines, Action callback)
        {
            _coroutines = coroutines;
            _callback = callback;
        }

        public IEnumerator Execute()
        {
            foreach (var routine in _coroutines)
            {
                yield return routine;
            }

            _callback?.Invoke();
        }
    }


    public class DelayRoutine<T> : IRoutine where T : class
    {
        private T _yieldDelay;
        private Action _callback;

        public DelayRoutine(T yieldDelay, Action callback)
        {
            _yieldDelay = yieldDelay;
            _callback = callback;
        }

        public IEnumerator Execute()
        {
            yield return _yieldDelay;
            _callback?.Invoke();
        }
    }


    public class ConditionalDelayRoutine<T> : IRoutine where T : class 
    {
        private T _yieldDelay;
        private Action _callback;
        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _condition;

        public ConditionalDelayRoutine(T yieldDelay, Action callback, RoutineInfo routineInfo, Func<RoutineInfo, bool> condition)
        {
            _yieldDelay = yieldDelay;
            _callback = callback;
            _routineInfo = routineInfo;
            _condition = condition;
        }

        public IEnumerator Execute()
        {
            if (_condition.Invoke(_routineInfo))
            {
                yield return _yieldDelay;
            }

            _callback?.Invoke();
        }
    }
}
