using UnityEngine;
 using System;
 using System.Collections;
 
 public static class CoroutineUtils {
 
     /**
      * Usage: StartCoroutine(CoroutineUtils.Chain(...))
      * For example:
      *     StartCoroutine(CoroutineUtils.Chain(
      *         CoroutineUtils.Do(() => Debug.Log("A")),
      *         CoroutineUtils.WaitForSeconds(2),
      *         CoroutineUtils.Do(() => Debug.Log("B"))));
      */
     public static IEnumerator Chain(MonoBehaviour monoBehaviour, params IEnumerator[] actions) {
         foreach (IEnumerator action in actions) {
             yield return monoBehaviour.StartCoroutine(action);
         }
     }
 
     /**
      * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
      * For example:
      *     StartCoroutine(CoroutineUtils.DelaySeconds(
      *         () => DebugUtils.Log("2 seconds past"),
      *         2);
      */
     public static IEnumerator DelaySeconds(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
     }

     public static IEnumerator WaitForFrames(int frameCount) {
        if (frameCount <= 0)
        {
            throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
        }

        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
     }
 
 
     public static IEnumerator WaitForSeconds(float time) {
         yield return new WaitForSeconds(time);
     }
 
     public static IEnumerator Do(Action action) {
         action();
         yield return 0;
     }
 }