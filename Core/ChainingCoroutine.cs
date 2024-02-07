using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace StellaFox
{
    public interface IChainingCoroutine
    {
        public IChainingCoroutine Bind(Action<RoutineInfo> action);
        public IChainingCoroutine Wait(YieldInstruction wait, Func<RoutineInfo, bool> triggetCondition = null);

        public IChainingCoroutine BeginLoop(Func<RoutineInfo, bool> exitCondition);
        public IChainingCoroutine EndLoop();

        public void Play();
        public void Stop();
    }

    public class ChainingRoutine : IChainingCoroutine
    {
        public ChainingRoutine()
        {
            _processor = new RoutineExecutor();
            OnNextRoutine = () => _routineInfo.Count++;
        }

        #region 
        public enum RoutineMergeType
        {
            First,
            Last
        }

        public Action OnNextRoutine;

        public Coroutine WaitCoroutine 
        {
            get;
            private set;
        }

        public bool Playing
        {
            private set; 
            get;
        }

        private RoutineInfo _routineInfo = new ();

        //현재 실행 레벨
        //1 : 한번 실행으로 등록
        //2 : 싱글 반복문 등록할때
        //3 : 이중 반복문 등록할때
        //4 ~ : 그 이상
        private int _repeatLevel = 0;
        private RoutineExecutor _processor;
        #endregion


        public IChainingCoroutine Bind(Action<RoutineInfo> action)
        {
            _processor.Bind(new Routine(action, _routineInfo, OnNextRoutine), _repeatLevel);
            return this;
        }

        public IChainingCoroutine Wait(YieldInstruction wait, Func<RoutineInfo, bool> triggerCondition = null)
        {
            //triggerCondition에 조건이 안들어왔거나, 조건이 참일 경우에만 바인딩.
            if (triggerCondition == null || triggerCondition.Invoke(_routineInfo))
            {
                _processor.Bind(new DelayRoutine(wait, OnNextRoutine), _repeatLevel);
            }
            else //조건이 들어왔는데, 조건이 거짓인 경우 OnNextRoutine만 호출
            {
                _processor.Bind(new CallbackMessageRoutine(OnNextRoutine), _repeatLevel);
            }

            return this;
        }

        //조건이 달성될때까지 대기
        public IChainingCoroutine Wait(Func<RoutineInfo, bool> triggerCondition = null)
        {
            if (triggerCondition == null)
            {
                return this;
            }

            if (triggerCondition.Invoke(_routineInfo))
            {
                var customYI = new ChainYieldInstructure(triggerCondition, _routineInfo);
                _processor.Bind(new CustomDelayRoutine(customYI), _repeatLevel);
            }

            return this;
        }

        public IChainingCoroutine Wait(Coroutine[] coroutines)
        {
            _processor.Bind(new WaitRoutines(coroutines, OnNextRoutine), _repeatLevel);
            return this;
        }

        public IChainingCoroutine BeginLoop(Func<RoutineInfo, bool> loopCondition)
        {
            _processor.Bind(new RoutineProcessor(loopCondition, _routineInfo), _repeatLevel);
            _repeatLevel++; //넣고 루프를 하나 증가. _repeatLevel부터 올리면 index를 증가하고 list에 넣는것과 같음. 이게 맞음
            return this;
        }

        public IChainingCoroutine EndLoop()
        {
            _repeatLevel--;

            _repeatLevel = Mathf.Max(0, _repeatLevel);
            return this;
        }

        public void Play()
        {
            WaitCoroutine = CoroutineHelper.StartCoroutine(_processor.Execute());
        }

        public void Stop()
        {
            if (!Playing)
            {
                return;
            }

            CoroutineHelper.StopAllCoroutines();
        }
    }
}