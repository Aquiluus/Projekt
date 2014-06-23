using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class GameController : MonoBehaviour {

    public ChessboardPlane lastPiece;
	public GameObject lastPiece1;
    public ChessboardPlane actualPiece;
	public GameObject actualPiece1;
    public ChessboardPlane to;
	public GameObject toPlane1;
    public ChessboardPlane[] possibleDestinations;

    public Camera gameCamera;

	public GameObject whiteWins;
	public GameObject blackWins;

	private int whiteCount = 12;
	private int blackCount = 12;

	private Animator cameraAnimator;
    private ChessboardController ChessboardControllerScript;
    private GameController GameControllerScript;

	public bool forceAttack;
    public Allegiance turnAlligiance = Allegiance.white;

    // TODO : camera animation
    private Vector3 cameraWhitePosition = new Vector3(215.0f, 535.0f, -327.5f);
    private Quaternion cameraWhiteRotation = Quaternion.Euler(new Vector3(45.0f, -90.0f, 0.0f));
    private Vector3 cameraBlackPosition = new Vector3(203.0f, 535.0f, -327.5f);
    private Quaternion cameraBlackRotation = Quaternion.Euler(new Vector3(45.0f, 90.0f, 0.0f));


	void Awake()
	{
		forceAttack= false;
		ChessboardControllerScript = GameObject.Find("ChessboardController").GetComponent<ChessboardController>();
		GameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
		lastPiece = null;
		actualPiece = null;
		to = null;
		possibleDestinations = new ChessboardPlane[4];
		cameraAnimator = gameCamera.GetComponent<Animator> ();
		possibleDestinations = new ChessboardPlane[4];
	}


    // Use this for initialization
    void Start()
	{
		PreTurnSetting();
       
    }
	IEnumerator WaitFor1Sec(){

		 yield return new WaitForSeconds(1);
	}


	
	// Update is called once per frame
	void FixedUpdate () 
	{
		bool attacked= false;

        //PreTurnSetting();

        if (actualPiece != null )
        {
			if (actualPiece.piece != null&& actualPiece.piece.Allegiance == turnAlligiance)
			{
				bool active = CalculateDestinations(actualPiece.piece.IsKing, AllegianceToInteger(actualPiece.piece.Allegiance));
			}
			ActivatePossibleDestinations();

			if (to != null && to.particleSystem.activeInHierarchy == true && actualPiece.piece.Allegiance == turnAlligiance)
			{

				int checkX =actualPiece.X-to.X;
				if( Mathf.Abs(checkX) > 1)
				{
					PieceController tempPiece;
					Debug.Log("bicie!");
					attacked = true;
					if ( DestroyPiece())		// TODO: Destroy Animation
					actualPiece.piece.transform.position = to.plane.transform.position; // TODO: movement animation

					tempPiece = actualPiece.piece;
					WaitFor1Sec();

					actualPiece = to;
					actualPiece.piece = tempPiece;

					WaitFor1Sec();

					if (actualPiece.piece != null && actualPiece.piece.Allegiance == turnAlligiance)
					{
						Debug.Log("po biciu");
						ClearPossibleDestinations();
						bool activeDestinations = CalculateDestinations(actualPiece.piece.IsKing, AllegianceToInteger(actualPiece.piece.Allegiance));
						if (activeDestinations)
						{ Debug.Log ("mamy jakieś possible destinations");
							foreach(ChessboardPlane destination in possibleDestinations)
							{
								if(destination!=null)
									if( Mathf.Abs(actualPiece.X-destination.X) > 1)
									{
										forceAttack = true;
										Debug.Log ("bedzie force attack zapale lampkę na "+ destination.plane.gameObject.name);
										foreach (ChessboardPlane colorPiece in ChessboardControllerScript.Board)
										{
											if (colorPiece.piece != null && colorPiece.piece.Allegiance == turnAlligiance)
											{
												colorPiece.piece.GetComponent<TapGesture>().enabled = false;
											}
										}
										actualPiece.piece.GetComponent<TapGesture>().enabled = true;
										destination.particleSystem.SetActive(true);
										
									}else
									{
										Debug.Log("nie bedzie forceattack na "+ destination.plane.gameObject.name);
										forceAttack=false;
										to= null;
										actualPiece = null;
									}

							}

						}else{
							forceAttack=false;
							to= null;
							actualPiece = null;
						}
					}

				
			}
				if (actualPiece != null && to!=null && attacked ==false)
				{
					Debug.Log ("ruch bez bicia");
					actualPiece.piece.transform.position = to.plane.transform.position; // TODO: movement animation
					forceAttack = false;
					to=null;
				}
				
				if(forceAttack != true)
				{
					Debug.Log ("koniec tury");
					attacked = false;
					MovementDone();
				}

        }

		CheckIfWin ();

		}

	}

	void Update()
	{//debug 
		if (lastPiece!=null&&lastPiece.piece!=null&&lastPiece.piece.currentGameObject!=null)lastPiece1 = lastPiece.piece.currentGameObject;
		if (actualPiece!=null&&actualPiece.piece!=null&&actualPiece.piece.currentGameObject!=null)actualPiece1 = actualPiece.piece.currentGameObject;
		if ( to!=null&&to.plane!=null&& to.plane.gameObject!=null)toPlane1 = to.plane.gameObject;
	}

	void UpdateNumberOfPieces(PieceController figure)
	{
		if (figure.Allegiance == Allegiance.white)
						whiteCount--;
				else
						blackCount--;
	}

	bool DestroyPiece ()
	{
		int deltaX =actualPiece.X-to.X;
		int deltaY =actualPiece.Y-to.Y;
		ChessboardPlane toDestroy;
		
		if(to.X>actualPiece.X && to.Y>actualPiece.Y)
		{	
			deltaX = actualPiece.X + 1;
			deltaY = actualPiece.Y + 1;
		}
		
		if(to.X<actualPiece.X && to.Y>actualPiece.Y)
		{	
			deltaX = actualPiece.X - 1;
			deltaY = actualPiece.Y + 1;
		}
		
		if(to.X>actualPiece.X && to.Y<actualPiece.Y)
		{	
			deltaX = actualPiece.X + 1;
			deltaY = actualPiece.Y - 1;
		}
		
		if(to.X<actualPiece.X && to.Y<actualPiece.Y)
		{	
			deltaX = actualPiece.X - 1;
			deltaY = actualPiece.Y - 1;
		}
		
		toDestroy =ChessboardControllerScript.Board[deltaX-1,deltaY-1];

	//	Debug.Log ("do zabicia x:"+deltaX + "y:"+deltaY);
	//	Debug.Log (" name:" + toDestroy.piece.currentGameObject.name);
		if(toDestroy != null && toDestroy.piece.Allegiance != turnAlligiance)
		{
			GameObject.Destroy(toDestroy.piece.currentGameObject);
			return true;
		}else 
			return false;
	
	}

	void CheckIfWin()
	{
		if (whiteCount == 0) 
		{
			whiteWins.SetActive(true);
		} 
		else if (blackCount == 0) 
		{
			blackWins.SetActive(true);
		}

	}

	void MovementDone()
	{
		ClearPossibleDestinations();
		to = null;
		actualPiece = null;
		PostTurnSetting();
		PreTurnSetting();

	}

	
	void PostTurnSetting()
	{
		if (turnAlligiance == Allegiance.white)
		{
			cameraAnimator.SetTrigger("WhiteToBlack");
			turnAlligiance = Allegiance.black;
			//gameCamera.transform.rotation = cameraBlackRotation;
			//gameCamera.transform.position = cameraBlackPosition;
		}
		else if (turnAlligiance == Allegiance.black)
		{
			cameraAnimator.SetTrigger("BlackToWhite");
			turnAlligiance = Allegiance.white;
			//gameCamera.transform.rotation = cameraWhiteRotation;
			//gameCamera.transform.position = cameraWhitePosition;
		}
		
	}
	
	void PreTurnSetting()
	{
		foreach (ChessboardPlane colorPiece in ChessboardControllerScript.Board)
		{
			if (colorPiece.piece != null)
			{
				if (colorPiece.piece.Allegiance == turnAlligiance)
				{
					colorPiece.piece.GetComponent<TapGesture>().enabled = true;
				}
				else
				{
					colorPiece.piece.GetComponent<TapGesture>().enabled = false;
				}
			}
		}
		
	}


    void ActivatePossibleDestinations()
    {
		bool ret=false;
        foreach (ChessboardPlane possiblePlane in possibleDestinations)
        {
            if (possiblePlane != null && possiblePlane.piece == null)
            {
                possiblePlane.particleSystem.SetActive(true);
                possiblePlane.plane.GetComponent<TapGesture>().enabled = true;

            }
			else if(possiblePlane!=null && possiblePlane.piece!=null)
			{
				//if(GameControllerScript.turnAlligiance == Allegiance.white);
				//possiblePlane.piece.currentGameObject.GetComponent<TapGesture>().enabled = true;
				

			}

        }
	

    }

	bool CalculateDestinations(bool isKing, int colour)
	{
		bool needToAttack=false;
		int actualX = actualPiece.X - 1;
		int actualY = actualPiece.Y - 1;
		
		if ((actualX - 1) >= 0 && (actualX - 1) <= 7 && (actualY + colour) >= 0 && (actualY + colour) <= 7)
		{
			if(ChessboardControllerScript.Board[actualX - 1, actualY + colour].piece== null)
			{
			possibleDestinations[0] = ChessboardControllerScript.Board[actualX - 1, actualY + colour];

			}
			else
			{ 	if ((actualX - 2) >= 0 && (actualX - 2) <= 7 && (actualY + colour*2) >= 0 && (actualY + colour*2) <= 7 )
				if(ChessboardControllerScript.Board[actualX - 2, actualY + colour*2].piece== null && ChessboardControllerScript.Board[actualX - 1, actualY + colour].piece.Allegiance != GameControllerScript.turnAlligiance)
				{
				possibleDestinations[0] = ChessboardControllerScript.Board[actualX - 2, actualY + colour*2];
				needToAttack=true;
				}
			}
			
		}

		if ((actualX + 1) >=0 && (actualX + 1) <= 7 && (actualY + colour) >= 0 && (actualY + colour) <= 7)
		{
			if(ChessboardControllerScript.Board[actualX + 1, actualY + colour].piece== null)
			{
				possibleDestinations[1] = ChessboardControllerScript.Board[actualX + 1, actualY + colour];

			}
			else
			{ 	if ((actualX + 2) >=0 && (actualX + 2) <= 7 && (actualY + colour*2) >= 0 && (actualY + colour*2) <= 7)
				if(ChessboardControllerScript.Board[actualX + 2, actualY + colour*2].piece== null&& ChessboardControllerScript.Board[actualX + 1, actualY + colour].piece.Allegiance != GameControllerScript.turnAlligiance)
				{
					possibleDestinations[1] = ChessboardControllerScript.Board[actualX + 2, actualY + colour*2];
					needToAttack=true;
				}
			}
		}
		possibleDestinations[2] = null;
		possibleDestinations[3] = null;
		if(isKing)
		{
			if ((actualX - 1) >= 0 &&  (actualX - 1) <= 7 && (actualY - colour) >= 0 && (actualY - colour) <= 7)
			{
				if(ChessboardControllerScript.Board[actualX - 1, actualY - colour].piece== null)
				{
					possibleDestinations[2] = ChessboardControllerScript.Board[actualX - 1, actualY - colour];

				}
				else
				{   if ((actualX - 2) >= 0 &&  (actualX - 2) <= 7 && (actualY - colour*2) >= 0 && (actualY - colour*2) <= 7)
					if(ChessboardControllerScript.Board[actualX - 2, actualY - colour*2].piece== null && ChessboardControllerScript.Board[actualX - 1, actualY - colour].piece.Allegiance != GameControllerScript.turnAlligiance)
					{
						possibleDestinations[2] = ChessboardControllerScript.Board[actualX - 2, actualY - colour*2];
						needToAttack=true;
					}
				}
			}
			if ((actualX + 1) >= 0 && (actualX + 1) <= 7 && (actualY - colour) >= 0 && (actualY - colour) <= 7)
			{
				if(ChessboardControllerScript.Board[actualX + 1, actualY - colour].piece== null)
				{
					possibleDestinations[3] = ChessboardControllerScript.Board[actualX + 1, actualY - colour];

				}
				else
				{   if ((actualX + 2) >= 0 && (actualX + 2) <= 7 && (actualY - colour*2) >= 0 && (actualY - colour*2) <= 7)
					if(ChessboardControllerScript.Board[actualX + 2, actualY - colour*2].piece== null && ChessboardControllerScript.Board[actualX + 1, actualY - colour].piece.Allegiance != GameControllerScript.turnAlligiance)
					{
						possibleDestinations[3] = ChessboardControllerScript.Board[actualX + 2, actualY - colour*2];
						needToAttack=true;
					}
				}
			}
		}
		return needToAttack;
	}

    public void ClearPossibleDestinations()
    {
       
		for (int i=0; i<4; i++) 
		{
			if(possibleDestinations[i] !=null)
			{
		 		possibleDestinations[i].particleSystem.SetActive(false);
				possibleDestinations[i].plane.GetComponent<TapGesture>().enabled = false;
				possibleDestinations[i] = null;
			}
		}

    }





    int AllegianceToInteger(Allegiance allegiance)
    {
        if (allegiance == Allegiance.black)
        {
            return 1;
        }
        else if (allegiance == Allegiance.white)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
