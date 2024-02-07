using StellaFox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTester : MonoBehaviour
{
    public void Start()
    {
        Test2();
    }

    private void Test1()
    {
        int x = 0;
        ChainingRoutine chaining = new ChainingRoutine();
        chaining.OnNextRoutine += () =>
        {
            print($"on next {++x}");
        };

        chaining.Bind(x => print("일"))
                .Wait(new WaitForSeconds(3)) //조건이 만족되지 않으면 OnNextRoutine도 실행되지 않음.
                .Bind(x => print("삼"))
                .Wait(new WaitForSeconds(3), c => x > 10000 )
                .Bind(x => print("오"));

        chaining.Play();
    }

    private void Test2()
    {
        int x = 0;
        ChainingRoutine chaining = new ChainingRoutine();

        chaining.Bind(x => print("1"))
                .BeginLoop(l => x++ < 3)
                    .Bind(x => print("2"))
                    .Bind(x => print("3"))
                    .Wait(new WaitForSeconds(1f))
                .EndLoop()
                .Bind(x => print($"{x.Count}"));

        chaining.Play();
    }

    private void Test3()
    {
        int x = 0, y = 0;
        ChainingRoutine chaining = new ChainingRoutine();

        chaining.Bind(x => print("1"))
                .BeginLoop(l => x++ < 2)
                    .Bind(x => print("2"))
                    .BeginLoop(l => y++ < 2)
                        .Bind(x => print("3"))
                        .Wait(new WaitForSeconds(1f))
                    .EndLoop()
                .EndLoop()
                .Bind(x => print("end"));

        chaining.Play();
    }

    private void Test4()
    {
        int x = 0;
        int y = 0;

        ChainingRoutine chaining = new ChainingRoutine();
        chaining.Bind(x => print("1"))
                .Bind(x => print("Hello world 2"))
                    .BeginLoop(c => x++ < 3) //loop 1
                    .Bind(x => print("s loop 3"))
                        .BeginLoop(c => y++ < 3) //loop 2
                        .Bind(x => print("d loop 6"))
                        .Wait(new WaitForSeconds(0.01f))
                        .EndLoop() //end loop 2
                    .Bind(x => print("s loop 7"))
                    .Wait(new WaitForSeconds(3))
                    .EndLoop()
                .Bind(x => print($"{x.Count} end")); //end loop 1

        //x.Count 실행 순서에 문제 있음

        chaining.OnNextRoutine += () =>
        {
            print($"on next {x}, {y}");
        };

        chaining.Play();
    }
}