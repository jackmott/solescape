using System;
using UnityEngine;
using System.Collections;

public class TickManager : MonoBehaviour
{

    public event Action tick;
    public event Action lateTick;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FixedTick());
    }

    IEnumerator FixedTick()
    {
        for (; ; )
        {
            if (tick != null)
                tick();
            if (lateTick != null)
                lateTick();
            yield return new WaitForFixedUpdate();
        }
    }
}