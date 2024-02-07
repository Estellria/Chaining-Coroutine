using StellaFox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTester : MonoBehaviour
{
    public void Start()
    {
        Test1();
    }

    private void Test1()
    {
        int x = 0;
        ChainingRoutine chaining = new ChainingRoutine();
        chaining.OnNextRoutine += () => {
            print($"on next {++x}");
        };

        chaining.Bind(x => print("1"))
                .Bind(x => print("2"))
                .Bind(x => print("3"));

        //chaining.Play();
    }

    private void Test3()
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
                        .Wait(new WaitForSeconds(0.01f), c => x + y < 10)
                        .EndLoop() //end loop 2
                    .Bind(x => print("s loop 7"))
                    .Wait(new WaitForSeconds(3))
                    .EndLoop()
                .Bind(x => print($"{x.Count} end")); //end loop 1

        chaining.OnNextRoutine += () => {
            print($"on next {x}, {y}");
        };

        chaining.Play();
    }
}