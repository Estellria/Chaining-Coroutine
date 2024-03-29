﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CCoroutine
{
    public class RoutineExecutor
    {
        public RoutineExecutor(Action startAction, Action finishAction)
        {
            _routineList = new List<IRoutine>();

            _startAction = startAction;
            _endAction = finishAction;
        }

        private List<IRoutine> _routineList;
        private Action _startAction;
        private Action _endAction;

        private bool HaveRoutine
        {
            get { return _routineList.Count > 0; }
        }


        //런타임 실행
        public IEnumerator Execute()
        {
            _startAction.Invoke();

            for (int i = 0; i < _routineList.Count; ++i)
            {
                switch (_routineList[i].GetRoutineType)
                {
                    case eRoutineType.Delay:
                        yield return _routineList[i].Execute();
                        break;

                    case eRoutineType.Immediately:
                        _routineList[i].Execute();
                        break;
                }
            }

            _endAction.Invoke();
        }

        public void Bind(IRoutine newRoutine, int repeatLevel)
        {
            var accessList = _routineList;

            for (int i = 0; i < repeatLevel; ++i)
            {
                var rp = accessList?.LastOrDefault(e => e is RoutineIterator);
                accessList = (rp as RoutineIterator)?.GetRoutineList;

                if (accessList == null)
                {
                    throw new UnityException("thread error");
                }
            }

            accessList.Add(newRoutine);
        }
    }
}
