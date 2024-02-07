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
        private int _repeatLevel = 1;
        private RoutineExecutor _processor;
        #endregion


        public IChainingCoroutine Bind(Action<RoutineInfo> action)
        {
            _processor.Bind(new Routine(action, _routineInfo, OnNextRoutine), _repeatLevel);
            return this;
        }

        public IChainingCoroutine Wait(YieldInstruction wait, Func<RoutineInfo, bool> triggerCondition = null)
        {
            if (triggerCondition.Invoke(_routineInfo))
            {
                _processor.Bind(new DelayRoutine(wait), _repeatLevel);
            }

            return this;
        }

        public IChainingCoroutine Wait(Func<RoutineInfo, bool> triggerCondition = null)
        {
            if (triggerCondition.Invoke(_routineInfo))
            {
                var customYI = new ChainYieldInstructure(triggerCondition, _routineInfo);
                _processor.Bind(new CustomDelayRoutine(customYI), _repeatLevel);
            }

            return this;
        }

        public IChainingCoroutine Wait(Coroutine[] coroutines)
        {
            _processor.Bind(new WaitRoutines(coroutines), _repeatLevel);
            return this;
        }

        public IChainingCoroutine BeginLoop(Func<RoutineInfo, bool> loopCondition)
        {
            _repeatLevel++;

            _processor.Bind(new RoutineProcessor(loopCondition, _routineInfo), _repeatLevel);
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