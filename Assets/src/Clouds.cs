using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour
{

    public int rotateSpeed = 10;



    private GameState state;




    // Use this for initialization
    void Start()
    {
        state = GameState.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, state.planet.rotateVector, Time.deltaTime * rotateSpeed);

        //turn clouds brown as pollution gets worse;
        float fractionalPollution = state.GetPollution() / state.pollutionDeathAmount;
        float r = 1f - .8f * fractionalPollution;
        float g = 1f - .9f * fractionalPollution;
        float b = 1f - .98f * fractionalPollution;
        float a = 1.0f * fractionalPollution;


        renderer.material.SetColor("_Color", new Color(r, g, b));
        renderer.material.SetFloat("_Alpha", a);

        

    }
}
