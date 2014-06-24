using UnityEngine;
using System.Collections;

public class MoveGun : MonoBehaviour {

    public float speed = 15;
    public float currentRotation = 0;
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (currentRotation > -8.0f)
            {
                transform.Rotate(new Vector3(speed * Time.deltaTime, 0, 0));
                currentRotation -= speed * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (currentRotation < 18.0f)
            {
                transform.Rotate(new Vector3(-speed * Time.deltaTime, 0, 0));
                currentRotation += speed * Time.deltaTime;
            }
        }
	}
}
