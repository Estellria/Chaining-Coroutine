using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

namespace StellaFox
{
    public class CoroutineHelper : MonoBehaviour
    {
        private static MonoBehaviour monoInstance = null;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            monoInstance = new GameObject("Coroutine Manager").AddComponent<CoroutineHelper>();
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
