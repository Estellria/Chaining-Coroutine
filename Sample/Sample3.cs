using CCoroutine;
using UnityEngine;


//Example of using function
public class Sample3 : MonoBehaviour
{
    public void SampleA()
    {
        var cc1 = new ChainingCoroutine();
        var cc2 = new ChainingCoroutine();

        var cc1Co = cc1.Bind(x => print("start cc1"))
                       .Wait(new WaitForSeconds(3f))
                       .Bind(x => print("finish cc1"))
                       .Play();

        cc2.Bind(x => print("start cc2"))
            .WaitFor(cc1Co)
            .Bind(x => print("finish cc2"))
            .Play();
    }


    public void SampleB()
    {
        bool condition = true;

        var cc = new ChainingCoroutine()
                    .Bind(x => print($"start. {x.elapsedTime} seconds, call count : {x.CalledCount}"))
                    .WaitIf(new WaitForSeconds(3f), x => condition)
                    .Bind(x => print($"end. {x.elapsedTime} seconds, call count : {x.CalledCount}"));

        cc.Play();
    }
}