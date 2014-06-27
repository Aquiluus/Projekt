using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

    public ParticleSystem tempParticleObject;

    void Awake()
    {
        tempParticleObject = gameObject.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (tempParticleObject.isStopped)
        {
            Destroy(gameObject);
        }
	}
}
