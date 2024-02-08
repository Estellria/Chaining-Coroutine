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


    public class RoutineProcessor : IRoutine
    {
        public RoutineProcessor(Func<RoutineInfo, bool> condition, RoutineInfo info)
        {
            _routineList = new List<IRoutine>();
            _exitCondition = condition;
            _routineInfo = info;
        }

        protected List<IRoutine> _routineList;

        private RoutineInfo _routineInfo;
        private Func<RoutineInfo, bool> _exitCondition;


        public virtual IEnumerator Execute()
        {
            while (_exitCondition.Invoke(_routineInfo))
            {
                foreach (IRoutine routine in _routineList)
                {
                    yield return routine.Execute();
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
                yield return routiner.Execute();
            }
        }
    }
}
