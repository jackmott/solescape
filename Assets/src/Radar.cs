using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour {


    float spinRotation = 0;

    void Update()
    {
        transform.localPosition = new Vector3(0, 0, Camera.main.transform.position.x / 51.3f);
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back,
            Camera.main.transform.rotation * Vector3.up);
        transform.Rotate(90, 0, 0);

        spinRotation += Time.deltaTime * 100 % 360;

        transform.Rotate(0, spinRotation, 0);

    }
}
