using UnityEngine;
using System.Collections;

public class ChildCameraScript : MonoBehaviour
{
    public int m_ZoomSpeed = 1;

    // Use this for initialization
    void Start()
    {
        print("started");

    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * m_ZoomSpeed);

    }
}
