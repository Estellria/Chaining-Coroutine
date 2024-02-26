using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

namespace CCoroutine
{
    public class CoroutineManager : MonoBehaviour
    {
        private static MonoBehaviour monoInstance = null;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            monoInstance = new GameObject("Coroutine Manager").AddComponent<CoroutineManager>();
            DontDestroyOnLoad(monoInstance.gameObject);
        }

        public new static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return monoInstance.StartCoroutine(coroutine);
        }

        public new static void StopCoroutine(Coroutine coroutine)
        {
            monoInstance.StopCoroutine(coroutine);
        }

        public new static void StopAllCoroutines()
        {
            monoInstance.StopAllCoroutines();
        }
    }
}
