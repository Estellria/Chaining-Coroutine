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

        cc.Bind(x => print($"1 : {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
          .Bind(x => print($"2 : {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
          .Bind(x => print($"3 : {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
          .Bind(x => print($"4 : {x.elapsedTime}��, ���� Ƚ��  {x.CallingCount}"))
          .Bind(x => print($"5 : {x.elapsedTime}��, ���� Ƚ��  {x.CallingCount}"));

        cc.OnNextRoutine += () => print("����");

        var coroutine = cc.Play();
    }


    private void Sample2()
    {
        new ChainingCoroutine()
            .Bind(x => print($"1 : {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"2 : {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"3 : {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"4 : {x.elapsedTime}��, ���� Ƚ��  {x.CallingCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"5 : {x.elapsedTime}��, ���� Ƚ��  {x.CallingCount}"))
            .Play();
    }


    private void Sample3()
    {
        int index = 0;
        new ChainingCoroutine()
            .BeginLoop(x => x.To(0, 5, 1, ref index))
                .Bind(x => print($"{index}��° ȣ��"))
                .Wait(new WaitForSeconds(0.5f))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    private void Sample4()
    {
        new ChainingCoroutine()
            .Bind(x => print("���콺�� ȭ���� Ŭ���ϼ���."))
            .BeginLoop(x => x.To(0, 5, 1))
                .WaitFor(c => Input.GetMouseButtonDown(0))
                .Bind(x => print($"���콺 Ŭ��"))
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
                .Bind(x => print($"{color1} {++count0}��° �ܺ� ���� �ݺ�"))
                .BeginLoop(x => x.To(0, 2, 1, true)) 
                    .Bind(x => print($"{color2} {++count1}��° �߰� ���� �ݺ�"))
                    .BeginLoop(x => x.To(0, 2, 1, true))
                        .Bind(x => print($"{color3} {++count2}��° ���� ���� �ݺ�"))
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
            .WaitFor(cc1Co) //cc1�� ���������� ��ٸ�
            .Bind(x => print("finish cc2"))
            .Play();
    }


    private void Sample7()
    {
        bool condition = true;

        var cc = new ChainingCoroutine()
                    .Bind(x => print($"start. {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"))
                    .WaitIf(new WaitForSeconds(3f), x => condition) //conditiond�� true �϶��� ����Ѵ�
                    .Bind(x => print($"end. {x.elapsedTime}��, ���� Ƚ�� {x.CallingCount}"));

        cc.Play();
    }
}