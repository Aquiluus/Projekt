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
		public GameObject currentGameObject;
      
        private ChessboardController ChessboardControllerScript;
        private GameController GameControllerScript;
       

     void Awake()
     {
		 currentGameObject= this.gameObject;
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
				Debug.Log ("klik ocupied plane zajęty");
	                int pirsza = OcupiedPlane.name[6] - '0'; // kurwa jak ? 
	                int drugo = OcupiedPlane.name[7] - '0'; // nie wieże 
	                GameControllerScript.lastPiece = GameControllerScript.actualPiece;
	                if (GameControllerScript.lastPiece != null && GameControllerScript.lastPiece.piece != null)
	                {
//					if(this.Allegiance != GameControllerScript.turnAlligiance)
//					{
//						Debug.Log("klik w obcy pionek");
//						GameControllerScript.to = ChessboardControllerScript.Board[pirsza - 1, drugo - 1];
//					}

					Debug.Log("klik zmiana pionka");
	                    GameControllerScript.ClearPossibleDestinations();
	                    GameControllerScript.lastPiece.piece.particleSystem.SetActive(false);
	                }
	                GameControllerScript.actualPiece = ChessboardControllerScript.Board[pirsza - 1, drugo - 1];
	                GameControllerScript.actualPiece.piece.particleSystem.SetActive(true);
            }



        }
    }

	void CheckIfKing(ChessboardPlane figure)
	{
		if (figure.piece.Allegiance == Allegiance.white && figure.Y == 1) 
		{
			KingSetting(figure);
		}
		if (figure.piece.Allegiance == Allegiance.black && figure.Y == 8) 
		{
			KingSetting(figure);
		}
	}

	void KingSetting(ChessboardPlane figure)
	{
		figure.piece.IsKing = true;
		figure.piece.currentGameObject.transform.localScale += new Vector3(0,0.3f,0);
		figure.piece.currentGameObject.transform.position += new Vector3(0,0.3f,0);

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

		CheckIfKing (ChessboardControllerScript.Board[pirsza-1, drugo-1]);
    }

    void OnTriggerExit(Collider other)
    {
        int pirsza = other.name[6] - '0'; // kurwa jak ? 
        int drugo = other.name[7] - '0'; // nie wieże 
        ChessboardControllerScript.Board[pirsza - 1, drugo - 1].piece.particleSystem.SetActive(false);
        ChessboardControllerScript.Board[pirsza - 1, drugo - 1].piece = null;
    }


}
