using StellaFox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public void Start()
    {
        Sample6();
    }
    

    private void Sample1()
    {
        var coroutine = new ChainingCoroutine();
        coroutine.Bind(x => print("1"))
                 .Wait(new WaitForSeconds(1))
                 .Bind(x => print("2"))
                 .Wait(new WaitForSeconds(1))
                 .Bind(x => print("3"))
                 .Wait(new WaitForSeconds(1))
                 .Bind(x => print("4"))
                 .Wait(new WaitForSeconds(1))
                 .Bind(x => print("5"));


        coroutine.Bind(x => print($"ȣ�� ����.\n �ɸ� �ð� : {x.elapsedTime}"))
                 .Play();
    }

    private void Sample2()
    {
        int i = 0;

        new ChainingCoroutine()
            .BeginLoop(l => i < 5)
            .Bind(x => print($"loop : {i + 1}"))
            .Wait(new WaitForSeconds(0.5f))
            .EndLoop(l => i++)
            .Play();
    }

    private void Sample3()
    {
        int i = 0;
        int j = 0;

        var coroutine = new ChainingCoroutine();
        coroutine.Bind(x => print("����"))
                 .BeginLoop(l => i < 5) //<- �� �κ� �� �ȵ�
                    .BeginLoop(l => j < 5)
                    .Bind(x => print($"{i}"))
                    .EndLoop(l => j++)
                 .EndLoop(l => { i++; j = 0; }) //�̷������� ���� �ݺ��� ������ �ʱ�ȭ ����ߵ�
                 .Bind(x => print($"��ü ȣ�� �� {x.CallingCount}"));

        coroutine.Play();
    }

    private void Sample4()
    {
        var coroutineX = new ChainingCoroutine();
        var co = coroutineX.Bind(x => print("x start"))
                  .Wait(new WaitForSeconds(10))
                  .Bind(x => print("x coroutine end"))
                  .Play();

        var coroutineY = new ChainingCoroutine();
        coroutineY.Bind(x => print("y start"))
                  .WaitFor(co)
                  .Bind(x => print("finish waiting"))
                  .Bind(x => print("y coroutine end"));

        coroutineY.Play();
    }

    private void Sample5()
    {
        var coroutine = new ChainingCoroutine();
        coroutine.Bind(x => print("start"))
                 .WaitFor(c => Input.GetKeyDown(KeyCode.Space))
                 .Bind(x => print("finish coroutine"))
                 .Play();
    }

    private void Sample6()
    {
        bool isCondition = true;

        var coroutine = new ChainingCoroutine();
        coroutine.Bind(x => print("start"))
                 .WaitIf(new WaitForSeconds(2f), c => isCondition)
                 .Bind(x => print("finish coroutine"))
                 .Bind(x => print($"seconds : {x.elapsedTime}"))
                 .Play();
    }
}