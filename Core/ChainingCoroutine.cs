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

        //���� ���� ����
        //1 : �ѹ� �������� ���
        //2 : �̱� �ݺ��� ����Ҷ�
        //3 : ���� �ݺ��� ����Ҷ�
        //4 ~ : �� �̻�
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
            //triggerCondition�� ������ �ȵ��԰ų�, ������ ���� ��쿡�� ���ε�.
            if (triggerCondition == null || triggerCondition.Invoke(_routineInfo))
            {
                _processor.Bind(new DelayRoutine(wait, OnNextRoutine), _repeatLevel);
            }
            else //������ ���Դµ�, ������ ������ ��� OnNextRoutine�� ȣ��
            {
                _processor.Bind(new CallbackMessageRoutine(OnNextRoutine), _repeatLevel);
            }

            return this;
        }

        //������ �޼��ɶ����� ���
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
            _repeatLevel++; //�ְ� ������ �ϳ� ����. _repeatLevel���� �ø��� index�� �����ϰ� list�� �ִ°Ͱ� ����. �̰� ����
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