using UnityEngine;
using System.Collections;

public class MoveTank : MonoBehaviour {

    public float acceleration = 5;
    public float currentVelocity = 0;
    public float maxForwardSpeed = 25;
    public float maxReverseSpeed = -10;
    public float rotationSpeed = 30;
    public Transform spawnPoint;
    public GameObject bulletObject;
    public GameObject fireEffect;
    public float fireRate;
 
    private MoveTrack leftTrack;
    private MoveTrack rightTrack;
    private float nextFire;
  


    void Start()
    {
        leftTrack = GameObject.Find(gameObject.name + "/Lefttrack").GetComponent<MoveTrack>();
        rightTrack = GameObject.Find(gameObject.name + "/Righttrack").GetComponent<MoveTrack>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (currentVelocity <= maxForwardSpeed)
            {
                currentVelocity += acceleration * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (currentVelocity >= maxReverseSpeed)
            {
                currentVelocity -= acceleration * Time.deltaTime;
            }
        }
        else
        {
            if (currentVelocity > 0)
            {
                currentVelocity -= acceleration * Time.deltaTime;
            }
            else if (currentVelocity < 0)
            {
                currentVelocity += acceleration * Time.deltaTime;
            }
        }

        if (Mathf.Abs(currentVelocity) <= 0.05f)
        {
            currentVelocity = 0;
        }

        transform.Translate(new Vector3(0, 0, currentVelocity * Time.deltaTime));

        if(currentVelocity > 0)
        {
            SetTrackAnimation(currentVelocity, 1, 1);
            //leftTrack.speed = currentVelocity;
            //leftTrack.gearStatus = 1;
            //rightTrack.speed = currentVelocity;
            //rightTrack.gearStatus = 1;
        }
        else if (currentVelocity < 0)
        {
            SetTrackAnimation(-currentVelocity, 2, 2);
            //leftTrack.speed = -currentVelocity;
            //leftTrack.gearStatus = 2;
            //rightTrack.speed = -currentVelocity;
            //rightTrack.gearStatus = 2;
        }
        else
        {
            SetTrackAnimation(0, 0, 0);
            //leftTrack.gearStatus = 0;
            //rightTrack.gearStatus = 0;
        }

        if(Input.GetKey (KeyCode.LeftArrow))
        {
            if(Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
                SetTrackAnimation(rotationSpeed / 2, 1, 2);
                //leftTrack.speed = rotationSpeed/2;
                //leftTrack.gearStatus = 1;
                //rightTrack.speed = rotationSpeed/2;
                //rightTrack.gearStatus = 2;
            }
            else
            {
                transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
                SetTrackAnimation(rotationSpeed / 2, 2, 1);
                //leftTrack.speed = rotationSpeed/2;
                //leftTrack.gearStatus = 2;
                //leftTrack.speed = rotationSpeed/2;
                //rightTrack.gearStatus = 1;
            }
        }

        if(Input.GetKey(KeyCode.RightArrow))
        {
            if(Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
                SetTrackAnimation(rotationSpeed / 2, 2, 1);
                //leftTrack.speed = rotationSpeed/2;
                //leftTrack.gearStatus = 2;
                //rightTrack.speed = rotationSpeed/2;
                //rightTrack.gearStatus = 1;
            }
            else
            {
                transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
                SetTrackAnimation(rotationSpeed / 2, 1, 2);
                //leftTrack.speed = rotationSpeed/2;
                //leftTrack.gearStatus = 1;
                //rightTrack.speed = rotationSpeed/2;
                //rightTrack.gearStatus = 2;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
            Instantiate(fireEffect, spawnPoint.position, spawnPoint.rotation);
            Instantiate(bulletObject, spawnPoint.position, spawnPoint.rotation);
        }
    }

    void SetTrackAnimation(float speed, int leftTrackStatus, int rightTrackStatus)
    {
        leftTrack.speed = speed;
        rightTrack.speed = speed;
        leftTrack.gearStatus = leftTrackStatus;
        rightTrack.gearStatus = rightTrackStatus;
    }
}
