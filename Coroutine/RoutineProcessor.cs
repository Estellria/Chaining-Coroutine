﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;

namespace StellaFox
{
    public class RoutineProcessor : IRoutine
    {
        public RoutineProcessor(Func<RoutineInfo, bool> condition, RoutineInfo info)
        {
            _routineList = new List<IRoutine>();
            _exitCondition = condition;
            _routineInfo = info;
        }

        public eRoutineType GetRoutineType 
        {
            get { return eRoutineType.Delay; }
        }

        protected List<IRoutine> _routineList;

        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _exitCondition;


        public virtual IEnumerator Execute()
        {
            while (_exitCondition.Invoke(_routineInfo))
            {
                for (int i = 0; i < _routineList.Count; ++i)
                {
                    switch(_routineList[i].GetRoutineType)
                    {
                        case eRoutineType.Delay:
                            yield return _routineList[i].Execute();
                            break;

                        case eRoutineType.NonDelay:
                            _routineList[i].Execute();
                            break;
                    }
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
        public RoutineExecutor(Action startAction, Action finishAction) : base(null, null) 
        {
            _startAction = startAction;
            _finishAction = finishAction;
        }

        private Action _startAction;
        private Action _finishAction;

        //런타임 실행
        public override IEnumerator Execute()
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

            _finishAction.Invoke();
        }
    }
}