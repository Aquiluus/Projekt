using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Events;
using System;

public class PieceController : MonoBehaviour {

        public GameObject Piece;
        public Allegiance Allegiance;

        private GameObject ChessboardControllerGO;
        private ChessboardController ChessboardControllerScript;

     void Awake()
     {
         Piece = this.gameObject;
      }

	// Use this for initialization
	void Start () {

        ChessboardControllerGO = GameObject.Find("ChessboardController");
        ChessboardControllerScript = ChessboardControllerGO.GetComponent<ChessboardController>();
	}

    void OnEnable()
    {
        GetComponent<TapGesture>().StateChanged += tapStateChangedHandler;


    }

    void OnDisable()
    {
        GetComponent<TapGesture>().StateChanged -= tapStateChangedHandler;

    }

    void tapStateChangedHandler( object sender, GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            /*
             * TODO
             * Napisać to kurwa
             */



        }
    }


	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {

        int pirsza = other.name[6] - '0'; // kurwa jak ? 
        int drugo = other.name[7] - '0'; // nie wieże 
        ChessboardControllerScript.Board[pirsza-1, drugo-1].piece = this;
        Debug.Log("kurwa colizja");
    }


}
