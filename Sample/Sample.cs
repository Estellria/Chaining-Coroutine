using StellaFox;
using UnityEngine;

public class Sample : MonoBehaviour
{
    [Range(1, 7)]
    public int sampleNumber;


    private void Start()
    {

    }


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
            .BeginLoop(x => x.To(0, 5, 1, ref index))
                .Bind(x => print($"{index}번째 호출"))
                .Wait(new WaitForSeconds(0.5f))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    private void Sample4()
    {
        new ChainingCoroutine()
            .Bind(x => print("마우스로 화면을 클릭하세요."))
            .BeginLoop(x => x.To(0, 5, 1))
                .WaitFor(c => Input.GetMouseButtonDown(0))
                .Bind(x => print($"마우스 클릭"))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    private void Sample5()
    {
        int count0 = 0;
        int count1 = 0;
        int count2 = 0;
        string color1 = "<color=red>##</color>";
        string color2 = "<color=green>@@</color>";
        string color3 = "<color=yellow>&&</color>";

        new ChainingCoroutine()
            .BeginLoop(x => x.To(0, 2, 1)) 
                .Bind(x => print($"{color1} {++count0}번째 외부 루프 반복"))
                .BeginLoop(x => x.To(0, 2, 1, true)) 
                    .Bind(x => print($"{color2} {++count1}번째 중간 루프 반복"))
                    .BeginLoop(x => x.To(0, 2, 1, true))
                        .Bind(x => print($"{color3} {++count2}번째 내부 루프 반복"))
                .EndLoop()
                .EndLoop()
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
                    .WaitIf(new WaitForSeconds(3f), x => condition) //conditiond이 true 일때만 대기한다
                    .Bind(x => print($"end. {x.elapsedTime}초, 실행 횟수 {x.CallingCount}"));

        cc.Play();
    }
}