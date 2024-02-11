using System;
using System.Collections;
using StellaFox;
using UnityEngine;

public interface IChainingCoroutine
{
    public IChainingCoroutine Bind(Action<RoutineInfo> action);

    public IChainingCoroutine Wait(YieldInstruction wait);

    public IChainingCoroutine WaitIf(YieldInstruction yield, Func<RoutineInfo, bool> condition);

    public IChainingCoroutine WaitFor(Func<RoutineInfo, bool> triggerCondition);
    public IChainingCoroutine WaitFor(params Coroutine[] coroutines);

    public IChainingCoroutine BeginLoop(Func<RoutineInfo, bool> exitCondition);
    public IChainingCoroutine EndLoop(Action<RoutineInfo> endAction = null);

    public Coroutine Play();
    public void Stop();
}

public interface IRoutine
{
    public eRoutineType GetRoutineType { get; }

    public IEnumerator Execute();
}