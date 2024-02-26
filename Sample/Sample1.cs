using CCoroutine;
using UnityEngine;


//Basic Example of Use
public class Sample1 : MonoBehaviour
{
    public void SampleA()
    {
        var cc = new ChainingCoroutine();

        cc.Bind(x => print($"1 : {x.elapsedTime} seconds, call count : {x.CalledCount}"))
          .Bind(x => print($"2 : {x.elapsedTime} seconds, call count : {x.CalledCount}"))
          .Bind(x => print($"3 : {x.elapsedTime} seconds, call count : {x.CalledCount}"));

        cc.OnNextRoutine += () => print("completeion");

        var coroutine = cc.Play();
    }


    public void SampleB()
    {
        new ChainingCoroutine()
            .Bind(x => print($"1 : {x.elapsedTime} seconds, call count : {x.CalledCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"2 : {x.elapsedTime} seconds, call count : {x.CalledCount}"))
            .Wait(new WaitForSeconds(1f))
            .Bind(x => print($"3 : {x.elapsedTime} seconds, call count : {x.CalledCount}"))
            .Wait(new WaitForSeconds(1f))
            .Play();
    }
}