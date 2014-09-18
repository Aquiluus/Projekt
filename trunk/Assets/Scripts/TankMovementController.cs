using UnityEngine;
using System.Collections;

public class TankMovementController : MonoBehaviour {

	public bool doMovement= false;
	public GameObject tank;
	public ChessboardPlane to;

	Vector3 temp;


	void moveTank ()
	{

		tank.transform.position = Vector3.Lerp (tank.transform.position, temp, 0.05f);
	
	}


	public void go(){
		temp = new Vector3(to.plane.transform.position.x, to.plane.transform.position.y + 1.1f, to.plane.transform.position.z);

		doMovement = true;

		}

	public void stop(){
		doMovement = false;
		to=null;
		tank=null;
	}

	
	// Update is called once per frame
	void Update () {

		if (to!=null && tank!=null && doMovement)
						moveTank ();

		if ( to!=null && tank!=null &&Vector3.Distance(to.plane.transform.position, tank.transform.position) < 0.05f)
						stop ();
	
	}
}
