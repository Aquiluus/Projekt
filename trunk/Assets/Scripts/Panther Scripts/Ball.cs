using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    public float speed = 200;
    public float range = 400;
    public GameObject ExplosionParticle;


    private float dist;
	private GameController gameController;

	void Awake()
	{
	 	gameController = GameObject.Find("GameController").GetComponent<GameController>();

	}

	// Update is called once per frame
	void Update () 
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        dist += Time.deltaTime * speed;

        if (dist >= range)
        {
            Instantiate(ExplosionParticle, transform.position, transform.rotation);
            Destroy(gameObject);
        }

	}

    void OnTriggerEnter(Collider other)
    {
        Instantiate(ExplosionParticle, transform.position, transform.rotation);
        Destroy(gameObject);
		gameController.doneExternalDestroy = true;
		Destroy (other.gameObject);
    }
}
