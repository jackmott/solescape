using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour
{
    
   
    
    // Use this for initialization
    void Start()
    {
        
   
        
    }

    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * 2);
    }

   
    

    // Update is called once per frame
    void Update()
    {
     
    }
}
