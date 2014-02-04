using UnityEngine;
using System.Collections;

public class Tornado : MonoBehaviour {

    float startTime = 0;
    float lifeTime = 2; //how long to exist

    void Start()
    {
        this.particleSystem.enableEmission = false; 
    }

    public void Show()
    {
        startTime = Time.time;
        this.particleSystem.enableEmission = true;
    }

    void Update()
    {
       
        float duration = Time.time - startTime;

        if (duration > lifeTime)
        {
            this.particleSystem.enableEmission = false;
        }


    }
}
