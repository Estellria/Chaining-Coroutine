using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCoroutine
{
    public class RoutineIterator : IRoutine
    {
        public RoutineIterator(Func<Condition, bool> condition)
        {
            GetRoutineList = new List<IRoutine>();
            _condition = new Condition();

            _exitCondition = condition;
        }

        public eRoutineType GetRoutineType 
        {
            get { return eRoutineType.Delay; }
        }

        public List<IRoutine> GetRoutineList 
        { 
            get;
            private set; 
        }

        private Condition _condition;
        private Func<Condition, bool> _exitCondition;


        public IEnumerator Execute()
        {
            while (_exitCondition.Invoke(_condition))
            {
                for (int i = 0; i < GetRoutineList.Count; ++i)
                {
                    switch(GetRoutineList[i].GetRoutineType)
                    {
                        case eRoutineType.Delay:
                            yield return GetRoutineList[i].Execute();
                            break;

                        case eRoutineType.Immediately:
                            GetRoutineList[i].Execute();
                            break;
                    }
                }
            }
        }
    }
}
