using UnityEngine;

// Sets up transformation matrices to scale&scroll water waves
// for the case where graphics card does not support vertex programs.


public class Water : MonoBehaviour
{
    bool expanding = false;

    void Start()
    {
        print("water");
    }
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * 2);

        if (transform.localScale.x > .98 && expanding)
        {
            expanding = false;
        }
        else if (transform.localScale.x < .98 && !expanding)
        {
            expanding = true;
        }

        Vector3 scale = transform.localScale;
        float speed = .01f;
        if (expanding)
        {
            Vector3 newScale = new Vector3(scale.x + speed * Time.deltaTime, scale.y + speed * Time.deltaTime, scale.z + speed * Time.deltaTime);
            transform.localScale = newScale;
        }
        else
        {
            Vector3 newScale = new Vector3(scale.x - speed * Time.deltaTime, scale.y - speed * Time.deltaTime, scale.z - speed * Time.deltaTime);
            transform.localScale = newScale;
        }



    }
    
}
