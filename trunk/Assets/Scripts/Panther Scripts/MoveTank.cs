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
    public float angle = 0.0f;
    public float maxAngle = 130.0f;

    public int turn = 0;
    public int drive = 0;
 
    private MoveTrack leftTrack;
    private MoveTrack rightTrack;
    private float nextFire;
    private GameController gameController;
  


    void Start()
    {
        leftTrack = GameObject.Find(gameObject.name + "/Lefttrack").GetComponent<MoveTrack>();
        rightTrack = GameObject.Find(gameObject.name + "/Righttrack").GetComponent<MoveTrack>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {

        //switch (turn)
        //{
        //    case 0:
        //        break;
        //    case 1:
        //        if (angle < maxAngle)
        //        {
        //            TurnLeft();
        //            angle += 1.0f;
        //        }
        //        else
        //        {
        //            angle = 0.0f;
        //            turn = 0;
        //            gameController.turned = true;
        //        }
        //        break;
        //    case 2:
        //        if (angle < maxAngle)
        //        {
        //            TurnRight();
        //            angle += 1.0f;
        //        }
        //        else
        //        {
        //            angle = 0.0f;
        //            turn = 0;
        //            gameController.turned = true;
        //        }
        //        break;
        //}
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

        if (currentVelocity > 0)
        {
            SetTrackAnimation(currentVelocity, 1, 1);
        }
        else if (currentVelocity < 0)
        {
            SetTrackAnimation(-currentVelocity, 2, 2);
        }
        else
        {
            SetTrackAnimation(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                TurnRight();
            }
            else
            {
                TurnLeft();
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                TurnLeft();
            }
            else
            {
                TurnRight();
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Time.time > nextFire)
        {
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

    public void TurnLeft()
    {
            transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
            SetTrackAnimation(rotationSpeed / 2, 2, 1);
    }

    public void TurnRight()
    {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
            SetTrackAnimation(rotationSpeed / 2, 1, 2);
    }

    public void DriveForward()
    {
        currentVelocity += acceleration * Time.deltaTime;
        transform.Translate(new Vector3(0, 0, currentVelocity * Time.deltaTime));
        SetTrackAnimation(currentVelocity, 1, 1);
    }
}

