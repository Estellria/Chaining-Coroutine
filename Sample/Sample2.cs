using CCoroutine;
using UnityEngine;


//Example of using repeaters
public class Sample2 : MonoBehaviour
{
    public void SampleA()
    {
        int index = 0;
        new ChainingCoroutine()
            .BeginLoop(x => x.To(0, 5, 1, ref index))
                .Bind(x => print($"{index} call"))
                .Wait(new WaitForSeconds(0.5f))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    public void SampleB()
    {
        new ChainingCoroutine()
            .Bind(x => print("Click the screen with mouse"))
            .BeginLoop(x => x.To(0, 5, 1))
                .WaitFor(c => Input.GetMouseButtonDown(0))
                .Bind(x => print($"click complete"))
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }


    public void SampleC()
    {
        int count0 = 0;
        int count1 = 0;
        int count2 = 0;
        string color1 = "<color=red>##</color>";
        string color2 = "<color=green>@@</color>";
        string color3 = "<color=yellow>&&</color>";

        new ChainingCoroutine()
            .BeginLoop(x => x.To(0, 2, 1))
                .Bind(x => print($"{color1} {++count0} loop 1"))
                .BeginLoop(x => x.To(0, 2, 1, true))
                    .Bind(x => print($"{color2} {++count1} loop 2"))
                    .BeginLoop(x => x.To(0, 2, 1, true))
                        .Bind(x => print($"{color3} {++count2} loop 3"))
                    .EndLoop()
                .EndLoop()
            .EndLoop()
            .Bind(x => print(x.elapsedTime))
            .Play();
    }
}