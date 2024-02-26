using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCoroutine 
{
    public class ReferenceAction
    {
        public Action onCallbackNext;

        public void Invoke()
        {
            if (onCallbackNext == null)
            {
                return;
            }

            onCallbackNext();
        }
    }
}