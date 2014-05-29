using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Events;
using System;


public class PlaneController : MonoBehaviour {


    private ChessboardController ChessboardControllerScript;
    private GameController GameControllerScript;

	// Use this for initialization
	void Start () {
        ChessboardControllerScript = GameObject.Find("ChessboardController").GetComponent<ChessboardController>();
        GameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        GetComponent<TapGesture>().StateChanged += tapStateChangedHandler;


    }

    void OnDisable()
    {
        GetComponent<TapGesture>().StateChanged -= tapStateChangedHandler;

    }

    void tapStateChangedHandler(object sender, GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            int pirsza = this.name[6] - '0'; // kurwa jak ? 
            int drugo = this.name[7] - '0'; // nie wieże 
            GameControllerScript.to = ChessboardControllerScript.Board[pirsza - 1, drugo - 1];


        }
    }
}
