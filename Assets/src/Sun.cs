using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour
{
    GameState state;
    Light l;
    LensFlare flare;
    
    // Use this for initialization
    void Start()
    {
        state = GameState.Instance;
        l = GetComponent<Light>();
        flare = GetComponent<LensFlare>();

    }

    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * 2);
    }

    public void UpdateSun()
    {
        //make light red
        float lightFactor = (float)state.yearsLeft / (float)state.gameLength;
        float intensity = 3f + 5f * (1-lightFactor);
        Color c = l.color;
        c.g = lightFactor;
        c.b = lightFactor;
        l.color = c;
        l.intensity = intensity;

        //increase lens flare size
        flare.color = c;
        flare.brightness = 2 * (1 - lightFactor) + 1;
    }
    

    // Update is called once per frame
    void Update()
    {
     
    }
}
