using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Events;
using System;

public class PieceController : MonoBehaviour {

        public Allegiance Allegiance;
        public bool IsKing;
        public Collider OcupiedPlane;
        public GameObject particleSystem;
      
        private ChessboardController ChessboardControllerScript;
        private GameController GameControllerScript;
       

     void Awake()
     {
 
         OcupiedPlane = null;
         AddAligiance();
         AddParticle();
         this.IsKing = false;
      }

    

	// Use this for initialization
	void Start () {

        ChessboardControllerScript = GameObject.Find("ChessboardController").GetComponent<ChessboardController>();
        GameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();

	}

    void AddAligiance()
    {
        bool allegianceTest = this.gameObject.name.StartsWith("White");
        if (allegianceTest)
        {
            Allegiance = Allegiance.white;
        }
        else
        {
            Allegiance = Allegiance.black;             //How racist is that? 
        }
    }

    void AddParticle()
    {
        particleSystem = (GameObject)Instantiate(Resources.Load("PieceParticle"), this.transform.position, this.transform.rotation);
        particleSystem.transform.parent = this.transform;
        particleSystem.SetActive(false);
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

            if (OcupiedPlane != null)
            {
                int pirsza = OcupiedPlane.name[6] - '0'; // kurwa jak ? 
                int drugo = OcupiedPlane.name[7] - '0'; // nie wieże 
                GameControllerScript.lastPiece = GameControllerScript.actualPiece;
                if (GameControllerScript.lastPiece != null && GameControllerScript.lastPiece.piece != null)
                {
                    GameControllerScript.lastPiece.piece.particleSystem.SetActive(false);
                }
                GameControllerScript.actualPiece = ChessboardControllerScript.Board[pirsza - 1, drugo - 1];
                GameControllerScript.actualPiece.piece.particleSystem.SetActive(true);
            }
        }
    }


	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        OcupiedPlane = other;

        int pirsza = other.name[6] - '0'; // kurwa jak ? 
        int drugo = other.name[7] - '0'; // nie wieże 
        ChessboardControllerScript.Board[pirsza-1, drugo-1].piece = this;
    }

    void OnTriggerExit(Collider other)
    {
        int pirsza = other.name[6] - '0'; // kurwa jak ? 
        int drugo = other.name[7] - '0'; // nie wieże 
        ChessboardControllerScript.Board[pirsza - 1, drugo - 1].piece.particleSystem.SetActive(false);
        ChessboardControllerScript.Board[pirsza - 1, drugo - 1].piece = null;
    }


}
