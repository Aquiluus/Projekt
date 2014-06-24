using UnityEngine;
using System.Collections;

public class MoveTrack : MonoBehaviour {

    public int currentTTIdx = 0;
    public Texture[] trackTextures;
    public float speed = 40;
    public float moveTick = 0.1f;
    public int gearStatus = 0;

	// Update is called once per frame
	void Update () 
    {
        switch (gearStatus)
        {
            case 0:
                break;

            case 1:
                if (speed < 1.0f)
                {
                    speed = 1;
                }
                if (Time.time > moveTick)
                {
                    currentTTIdx++;
                    if (currentTTIdx >= trackTextures.Length)
                    {
                        currentTTIdx = 0;
                    }
                    renderer.material.mainTexture = trackTextures[currentTTIdx];
                    moveTick = Time.time + 4 / (speed * 1000 / (60 * 60) * 100);
                }
                break;

            case 2:
                if (speed < 1)
                {
                    speed = 1;
                }
                if (Time.time > moveTick)
                {
                    currentTTIdx--;
                    if (currentTTIdx < 0)
                    {
                        currentTTIdx = trackTextures.Length - 1;
                    }
                    renderer.material.mainTexture = trackTextures[currentTTIdx];
                    moveTick = Time.time + 4 / (speed * 1000 / (60 * 60) * 100);
                }
                break;
        }
	}
}
