using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions 
{
    public static void DelayedCall(this MonoBehaviour monoBehaviour, float waitForSeconds, Action callback)
    {
        IEnumerator DelayCallCoroutine()
        {
            yield return new WaitForSeconds(waitForSeconds);
            
            callback?.Invoke();
        }

        monoBehaviour.StartCoroutine(DelayCallCoroutine());
    }
}
