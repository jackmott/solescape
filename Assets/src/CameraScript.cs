using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{

    public float m_RotateSpeed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        //rotate camera around planet
        float rotateDirection = Input.GetAxis("Horizontal");
        transform.Rotate(0, rotateDirection * m_RotateSpeed * Time.deltaTime, 0);

        rotateDirection = Input.GetAxis("Vertical");
        transform.Rotate(0, 0, rotateDirection * m_RotateSpeed * Time.deltaTime);

       

    }

    void FixedUpdate()
    {
        //transform.RotateAround (Vector3.zero, GameState.Instance.planet.rotateVector, Time.deltaTime * GameState.Instance.planet.rotateSpeed);
    }
}
