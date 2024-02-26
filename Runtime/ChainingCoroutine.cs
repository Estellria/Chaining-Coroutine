using UnityEngine;
using System;

namespace CCoroutine
{
    public class ChainingCoroutine : IChainingCoroutine
    {
        public ChainingCoroutine()
        {
            _onNextRoutine = new ReferenceAction();
            routineInfo = new RoutineInfo();
            _processor = new RoutineExecutor
            (
                routineInfo.Play,
                routineInfo.Stop
            );

            OnNextRoutine += routineInfo.AddCallCount;
        }

        #region Field

        private int _bindingLayer = 0;

        private ReferenceAction _onNextRoutine;
        private RoutineExecutor _processor;
        #endregion

        #region Property

        public Action OnNextRoutine 
        {
            get { return _onNextRoutine.onCallbackNext; }
            set { _onNextRoutine.onCallbackNext += value; }
        } 

        public Coroutine WaitCoroutine
        {
            get;
            private set;
        }

        public RoutineInfo routineInfo
        {
            get;
            private set;
        }

        public bool Playing
        {
            get { return WaitCoroutine != null; }
        }

        #endregion


        public IChainingCoroutine Bind(Action<RoutineInfo> action)
        {
            _processor.Bind(new Routine(action, routineInfo, _onNextRoutine), _bindingLayer);
            return this;
        }



        public IChainingCoroutine Wait(YieldInstruction wait)
        {
            _processor.Bind(new DelayRoutine<YieldInstruction>(() => wait, _onNextRoutine), _bindingLayer);
            return this;
        }



        public IChainingCoroutine WaitFor(Func<RoutineInfo, bool> triggerCondition)
        {
            if (triggerCondition != null)
            {
                Func<WaitForCompletion> cd = () => new WaitForCompletion(triggerCondition, routineInfo); //condition

                _processor.Bind(new DelayRoutine<WaitForCompletion>(cd, _onNextRoutine, true), _bindingLayer);
            }
            return this;
        }



        public IChainingCoroutine WaitFor(params Coroutine[] coroutines)
        {
            if (coroutines != null && coroutines.Length > 0)
            {
                _processor.Bind(new WaitRoutine(coroutines, _onNextRoutine), _bindingLayer);
            }
            return this;
        }



        public IChainingCoroutine WaitIf(YieldInstruction yield, Func<RoutineInfo, bool> condition)
        {
            if (condition != null)
            {
                _processor.Bind(new IfRoutine(yield, condition, routineInfo, _onNextRoutine), _bindingLayer);
            }

            return this;
        }



        public IChainingCoroutine BeginLoop(Func<Condition, bool> loopCondition)
        {
            if (loopCondition != null)
            {
                _processor.Bind(new RoutineIterator(loopCondition), _bindingLayer);
                _bindingLayer++;
            }
            else
            {
                throw new UnityException("Chaining Coroutine loop");
            }
            return this;
        }



        public IChainingCoroutine EndLoop(Action<RoutineInfo> endAction = null)
        {
            if (endAction != null)
            {
                _processor.Bind(new Routine(endAction, routineInfo, _onNextRoutine), _bindingLayer);
            }

            _bindingLayer = Mathf.Max(0, _bindingLayer - 1);
            return this;
        }



        public Coroutine Play()
        {
            if (!_processor.HaveRoutine)
            {
                return null;
            }


            if (_bindingLayer != 0)
            {
                throw new UnityException("Repeat level isn't zero");
            }

            if (!Playing)
            {
                WaitCoroutine = CoroutineManager.StartCoroutine(_processor.Execute());
            }
            else
            {
                Debug.LogError("already playing");
            }

            return WaitCoroutine;
        }



        public void Stop()
        {
            if (!Playing)
            {
                return;
            }

            CoroutineManager.StopCoroutine(WaitCoroutine);
        }
    }
}