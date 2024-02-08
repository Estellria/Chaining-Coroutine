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

        public IChainingCoroutine Wait(YieldInstruction wait);

        public IChainingCoroutine WaitIf(YieldInstruction wait, Func<RoutineInfo, bool> triggerCondition);

        public IChainingCoroutine WaitFor(Func<RoutineInfo, bool> triggerCondition);
        public IChainingCoroutine WaitFor(params Coroutine[] coroutines);

        public IChainingCoroutine BeginLoop(Func<RoutineInfo, bool> exitCondition);
        public IChainingCoroutine EndLoop(Action<RoutineInfo> exitAction = null);

        public Coroutine Play();
        public void Stop();
    }


    public class ChainingCoroutine : IChainingCoroutine
    {
        public ChainingCoroutine()
        {
            _routineInfo = new RoutineInfo();
            _processor = new RoutineExecutor();

            OnNextRoutine += _routineInfo.AddCallCount;
        }

        #region Field
        
        //���ε��Ҷ��� ���� ����. (���ε��Ҷ��� ����)
        //0     : �ѹ� �������� ���
        //1     : �̱� �ݺ��� ����Ҷ�
        //2     : ���� �ݺ��� ����Ҷ�
        //3 ~   : �� �̻�
        private int _repeatLevel = 0;
        public Action OnNextRoutine;

        private RoutineInfo _routineInfo;
        private RoutineExecutor _processor;
        #endregion

        #region Property
        public Coroutine WaitCoroutine 
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
            _processor.Bind(new Routine(action, _routineInfo, OnNextRoutine), _repeatLevel);
            return this;
        }



        public IChainingCoroutine Wait(YieldInstruction wait)
        {
            _processor.Bind(new DelayRoutine<YieldInstruction>(wait, OnNextRoutine), _repeatLevel);
            return this;
        }



        //������ �޼��ɶ����� ���
        public IChainingCoroutine WaitFor(Func<RoutineInfo, bool> triggerCondition)
        {
            if (triggerCondition != null)
            {
               var yieldWait = new WaitForComplete(triggerCondition, _routineInfo);
                _processor.Bind(new DelayRoutine<WaitForComplete>(yieldWait, OnNextRoutine), _repeatLevel);
            }
            return this;
        }



        public IChainingCoroutine WaitFor(params Coroutine[] coroutines)
        {
            _processor.Bind(new WaitRoutine(coroutines, OnNextRoutine), _repeatLevel);
            return this;
        }



        //������ ���� ��쿡�� ���
        public IChainingCoroutine WaitIf(YieldInstruction wait, Func<RoutineInfo, bool> triggerCondition)
        {
            if (triggerCondition != null) //���⼭ ������ null�� �ƴ϶��� �����
            {
                var cdr = new ConditionalDelayRoutine<YieldInstruction>(wait, OnNextRoutine, _routineInfo, triggerCondition);
                _processor.Bind(cdr, _repeatLevel);
            }

            return this;
        }



        public IChainingCoroutine BeginLoop(Func<RoutineInfo, bool> loopCondition)
        {
            _processor.Bind(new RoutineProcessor(loopCondition, _routineInfo), _repeatLevel);

            //�ְ� ������ �ϳ� ����. _repeatLevel���� �ø��� index�� �����ϰ� list�� �ִ°Ͱ� ����.
            _repeatLevel++; 
            return this;
        }



        public IChainingCoroutine EndLoop(Action<RoutineInfo> exitAction = null)
        {
            if (exitAction != null)
            {
                _processor.Bind(new Routine(exitAction, _routineInfo, null), _repeatLevel);
            }

            _repeatLevel = Mathf.Max(0, _repeatLevel - 1);
            return this;
        }



        public Coroutine Play()
        {
            if (_repeatLevel != 0)
            {
                throw new UnityException("Repeat level isn't zero");
            }

            if (! Playing)
            {
                _routineInfo?.Initialize();
                //var t = new ChainingCoroutine();
                //t._processor = _processor;
                //WaitCoroutine = CoroutineManager.StartCoroutine(t._processor.Execute());
                WaitCoroutine = CoroutineManager.StartCoroutine(_processor.Execute());
            }
            else
            {
                Debug.LogError("�̹� ���� ��");
            }

            return WaitCoroutine;
        }



        public void Stop()
        {
            if (!Playing)
            {
                return;
            }

            CoroutineManager.StopAllCoroutines();
        }
    }
}