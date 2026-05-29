using System;
using System.Collections;
using UnityEngine;

namespace RacingGoMap.Race
{
    public class CountdownController : MonoBehaviour
    {
        public event Action<int> OnTick;
        public event Action      OnFinished;

        public void StartCountdown(int seconds = 3) => StartCoroutine(Run(seconds));

        IEnumerator Run(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                OnTick?.Invoke(i);
                yield return new WaitForSeconds(1f);
            }
            OnFinished?.Invoke();
        }
    }
}
