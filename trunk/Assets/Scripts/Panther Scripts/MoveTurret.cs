using UnityEngine;
using System.Collections;

public class MoveTurret : MonoBehaviour {

    public float speed = 30;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
    }
}
