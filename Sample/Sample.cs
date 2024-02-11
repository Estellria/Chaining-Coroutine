using StellaFox;
using UnityEngine;

public class Sample : MonoBehaviour
{
    [Range(1, 7)]
    public int sampleNumber;


    [ContextMenu("Play sample")]
    public void PlaySample()
    {
        sampleNumber = Mathf.Clamp(sampleNumber, 1, 7);
        Invoke($"Sample{sampleNumber}", 0);
    }

    private void Sample1()
    {
        var cc = new ChainingCoroutine();

        cc.Bind(x => print($"1 : {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
          .Bind(x => print($"2 : {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
          .Bind(x => print($"3 : {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
          .Bind(x => print($"4 : {x.elapsedTime}초, 실행 횟수  {x.CallingCount}"))
          .Bind(x => print($"5 : {x.elapsedTime}초, 실행 횟수  {x.CallingCount}"));

        cc.OnNextRoutine += () => print("다음");

        var coroutine = cc.Play();
    }


    private void Sample2()
    {
        new ChainingCoroutine()
            .Bind(x => print($"1 : {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"2 : {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"3 : {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"4 : {x.elapsedTime}초, 실행 횟수  {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"5 : {x.elapsedTime}초, 실행 횟수  {x.CallingCount}"))
            .Play();
    }


    private void Sample3()
    {
        int index = 0;
        new ChainingCoroutine()
            .BeginLoop(x => index < 5)
                .Bind(x => print($"{++index}번째 호출"))
                .Wait(new WaitForSeconds(0.5f))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    private void Sample4()
    {
        int index = 0;
        new ChainingCoroutine()
            .BeginLoop(x => index < 5)
                .Bind(x => print($"{++index}번째 호출"))
                .WaitFor(c => Input.GetMouseButtonDown(0))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    private void Sample5()
    {
        int index1 = 0;
        int index2 = 0;
        int count = 0;
        string color1 = "<color=red>##</color>";
        string color2 = "<color=green>@</color>";

        new ChainingCoroutine()
            .BeginLoop(x => index1 < 3) //무한반복문됨 오류 고쳐야됨
                .Bind(x => print($"{color1} {index1 + 1}번째 외부 루프 반복"))
                .Bind(x => index1++)
                .BeginLoop(x => index2 < 3)
                    .Bind(x => print($"{color2} {++count}번째 내부 루프 반복"))
                    .Bind(x => index2++)
                .EndLoop()
                .Bind(x => index2 = 0)
                .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    private void Sample6()
    {
        var cc1 = new ChainingCoroutine();
        var cc2 = new ChainingCoroutine();

        var cc1Co = cc1.Bind(x => print("start cc1"))
                       .Wait(new WaitForSeconds(3f))
                       .Bind(x => print("finish cc1"))
                       .Play();

        cc2.Bind(x => print("start cc2"))
            .WaitFor(cc1Co) //cc1가 끝날때까지 기다림
            .Bind(x => print("finish cc2"))
            .Play();
    }


    private void Sample7()
    {
        bool condition = true;

        var cc = new ChainingCoroutine()
                    .Bind(x => print($"start. {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"))
                    .WaitIf(new WaitForSeconds(3f), x => condition)
                    .Bind(x => print($"end. {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"));

        cc.Play();
    }
}