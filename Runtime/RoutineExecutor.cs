using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace StellaFox
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

                    case eRoutineType.NonDelay:
                        _routineList[i].Execute();
                        break;
                }
            }

            _endAction.Invoke();
        }

        public void Bind(IRoutine newRoutine, int repeatLevel)
        {
            var accessList = _routineList;

            //repeatLevel의 수는 반복문 계층을 의미하고 한 계층당 하나의 반복문이 존재하기에 반복문에서 tempList가 null이 될 수 없다.
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
