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
    public float pieceHeightMargin = 0.2f;
    public float currAngle = 0.0f;
    public bool turned = false;
    public bool turnDone;
	public float maxSpeed;

    public Camera gameCamera;

	public GameObject whiteWins;
	public GameObject blackWins;

	public ChessboardPlane toDestroy;
	public GameObject gameObjectToDestroy;

	private int whiteCount = 12;
	private int blackCount = 12;

	private Animator cameraAnimator;
    private ChessboardController ChessboardControllerScript;
    private GameController GameControllerScript;

	private PieceController tempPiece;



	public bool forceAttack;
	public bool doneDestroy;
	public bool doneExternalDestroy;

    public Allegiance turnAlligiance = Allegiance.white;

    // TODO : camera animation
   
    private Quaternion cameraWhiteRotation = Quaternion.Euler(new Vector3(45.0f, -90.0f, 0.0f));
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
        turnDone = false;
		doneDestroy = false;
		doneExternalDestroy = false;
	}


    // Use this for initialization
    void Start()
	{
		PreTurnSetting();
       
    }
	IEnumerator WaitFor1Sec(){

		 yield return new WaitForSeconds(1.0F);
	}

    IEnumerator WaitFor3Sec()
    {
        yield return new WaitForSeconds(3.0f);
    }

	
	IEnumerator TurnAnimation(int side)
	{
		
		if( side == -1)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnLeft45");
		else if (side == 1)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnRight45");
		if (side == -2)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnLeft135");
		else if (side == 2)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnRight135");
		yield return new WaitForSeconds(1);
		
	}

	IEnumerator IdleAnimation()
	{

		actualPiece.piece.GetComponent<Animator>().SetTrigger("turnLeft135");
		yield return new WaitForSeconds(1);
	}
	
	IEnumerator TurnBackAnimation(int side)
	{
		Vector3 temp = new Vector3(to.plane.transform.position.x, to.plane.transform.position.y + pieceHeightMargin, to.plane.transform.position.z);
		actualPiece.piece.transform.position = temp;
		if (side == -1)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnStraightFromLeft45");
		else if (side == 1)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnStraightFromRight45");
		if (side == -2)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnStraightFromLeft135");
		else if (side == 2)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("turnStraightFromRight135");
		yield return new WaitForSeconds(1);
		
	}

	IEnumerator AttackAnimation (int side)
	{
		if( side == -1)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("attackLeft");
		else if (side == 1)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("attackRight");
		if (side == -2)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("attackLeft135");
		else if (side == 2)
			actualPiece.piece.GetComponent<Animator>().SetTrigger("attackRight135");
		yield return new WaitForSeconds(2);

	}

	
	// Update is called once per frame
	void Update () 
	{
		bool attacked= false;
        bool moveDone = false;
       

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


					attacked = true;
					toDestroy = DestroyPiece(doneDestroy);


					if (toDestroy != null && toDestroy.piece.Allegiance != turnAlligiance && doneDestroy == false)
						{
							Debug.Log("bicie!");
							gameObjectToDestroy = toDestroy.piece.currentGameObject;
							AttackPiece();
							//GameObject.Destroy(toDestroy.piece.currentGameObject);
							//to = toDestroy;
							doneDestroy= true;
						}

					// TUTAJ MA SIE TEN IF WYKONAĆ 
					// NIE WCHODZI DO NIEGO
					if (gameObjectToDestroy == null && doneExternalDestroy ==true && doneDestroy == true)
					{
						Debug.Log("mowe");
						Vector3 temp = new Vector3(to.plane.transform.position.x, to.plane.transform.position.y + pieceHeightMargin, to.plane.transform.position.z);
						actualPiece.piece.transform.position = temp; // TODO: movement animation
						
						tempPiece = actualPiece.piece;
						actualPiece = to;
						actualPiece.piece = tempPiece;
						
						doneDestroy = false;
						doneExternalDestroy = false;
						
					}
						//MovePiece();


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
                                        BlockOtherPieces();
										destination.particleSystem.SetActive(true);
										
									}else
									{
										Debug.Log("nie bedzie forceattack na "+ destination.plane.gameObject.name);
										forceAttack=false;
										//to= null;
										actualPiece = null;
									}

							}

						}else{
							forceAttack=false;
							//to= null;
							//actualPiece = null;
						}
					}

				
			}
				if (actualPiece !=null && to!=null && attacked ==false)
				{
                    
					Debug.Log ("ruch bez bicia");
                    
                    currAngle = FindAngle(actualPiece.piece.ownTank.transform.position, to.plane.transform.position, actualPiece.piece.ownTank.transform.up);
                    Debug.Log("Kąt: " + currAngle);

                    
                    MovePiece();

                    //actualPiece.piece.transform.position = temp; // TODO: movement animation
					forceAttack = false;
					to=null;
				}
               
				if(forceAttack != true && moveDone == true)
				{
					Debug.Log ("koniec tury");
					attacked = false;
					MovementDone();
                    forceAttack = false;
                    actualPiece = null;
                    to = null;
                    moveDone = false;
				}

        }

		CheckIfWin ();

		}

	}





	
	ChessboardPlane DestroyPiece (bool doneDestroy)
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


		toDestroy = ChessboardControllerScript.Board [deltaX - 1, deltaY - 1];

		//	Debug.Log ("do zabicia x:"+deltaX + "y:"+deltaY);
		//	Debug.Log (" name:" + toDestroy.piece.currentGameObject.name);

		return toDestroy;
		
	}

	void AttackPiece()
	{
		if (to.plane.transform.position.z < actualPiece.plane.transform.position.z)
		{
				if (this.turnAlligiance == Allegiance.black)
					if (to.plane.transform.position.x > actualPiece.plane.transform.position.x)
				{
					//BlockOtherPlanes();
					StartCoroutine(AttackAnimation(1));
				}
				else
				{
					// BlockOtherPlanes();
					StartCoroutine(AttackAnimation(2));
				}
				else
					if (to.plane.transform.position.x < actualPiece.plane.transform.position.x)
				{
					// BlockOtherPlanes();
					StartCoroutine(AttackAnimation(-1));
				}
				else
				{
					// BlockOtherPlanes();
					StartCoroutine(AttackAnimation(-2));
				}

			}

	}



    void MovePiece()
    {
        if (to.plane.transform.position.z < actualPiece.plane.transform.position.z)
        {
            if (turnDone == false)
            {
                ClearPossibleDestinations();
                if (this.turnAlligiance == Allegiance.black)
                    if (to.plane.transform.position.x > actualPiece.plane.transform.position.x)
                    {
                        //BlockOtherPlanes();
                        StartCoroutine(TurnAnimation(1));
                    }
                    else
                    {
                       // BlockOtherPlanes();
                        StartCoroutine(TurnAnimation(2));
                    }
                else
                    if (to.plane.transform.position.x < actualPiece.plane.transform.position.x)
                    {
                       // BlockOtherPlanes();
                        StartCoroutine(TurnAnimation(-1));
                    }
                    else
                    {
                       // BlockOtherPlanes();
                        StartCoroutine(TurnAnimation(-2));
                    }


                turnDone = true;
                BlockOtherPieces();
            }
            else
            {
                forceAttack = false;
                if (this.turnAlligiance == Allegiance.black)
                    if (to.plane.transform.position.x > actualPiece.plane.transform.position.x)
                    {
                       // BlockOtherPlanes();
                        StartCoroutine(TurnBackAnimation(1));
                    }
                    else
                    {
                       // BlockOtherPlanes();
                        StartCoroutine(TurnBackAnimation(2));
                    }
                else
                    if (to.plane.transform.position.x < actualPiece.plane.transform.position.x)
                    {
                      //  BlockOtherPlanes();
                        StartCoroutine(TurnBackAnimation(-1));
                    }
                    else
                    {
                       // BlockOtherPlanes();
                        StartCoroutine(TurnBackAnimation(-2));
                    }
                turnDone = false;
                MovementDone();
            }

        }
        else if (to.plane.transform.position.z > actualPiece.plane.transform.position.z)
        {
            if (turnDone == false)
            {
                ClearPossibleDestinations();
                if (this.turnAlligiance == Allegiance.black)
                    if (to.plane.transform.position.x > actualPiece.plane.transform.position.x)
                        StartCoroutine(TurnAnimation(-1));
                    else
                        StartCoroutine(TurnAnimation(-2));
                else
                    if (to.plane.transform.position.x < actualPiece.plane.transform.position.x)
                        StartCoroutine(TurnAnimation(1));
                    else
                        StartCoroutine(TurnAnimation(2));


                turnDone = true;
                BlockOtherPieces();
            }
            else
            {
                forceAttack = false;
                if (this.turnAlligiance == Allegiance.black)
                    if (to.plane.transform.position.x > actualPiece.plane.transform.position.x)
                        StartCoroutine(TurnBackAnimation(-1));
                    else
                        StartCoroutine(TurnBackAnimation(-2));
                else
                    if (to.plane.transform.position.x < actualPiece.plane.transform.position.x)
                        StartCoroutine(TurnBackAnimation(1));
                    else
                        StartCoroutine(TurnBackAnimation(2));
                turnDone = false;
                MovementDone();
            }

        }


    }



    void BlockOtherPieces()
    {
        foreach (ChessboardPlane colorPiece in ChessboardControllerScript.Board)
        {
            if (colorPiece.piece != null && colorPiece.piece.Allegiance == turnAlligiance)
            {
                colorPiece.piece.GetComponent<TapGesture>().enabled = false;
            }
        }
        actualPiece.piece.GetComponent<TapGesture>().enabled = true;

    }

    void BlockOtherPlanes()
    {
        foreach (ChessboardPlane colorPiece in ChessboardControllerScript.Board)
        {
            if (colorPiece.plane != null)
            {
                colorPiece.plane.GetComponent<TapGesture>().enabled = false;
                colorPiece.particleSystem.SetActive(false);
            }
        }
        to.plane.GetComponent<TapGesture>().enabled = true;
        to.particleSystem.SetActive(true);

    }

	void FixedUpdate()
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

		}
		else if (turnAlligiance == Allegiance.black)
		{
			cameraAnimator.SetTrigger("BlackToWhite");
			turnAlligiance = Allegiance.white;
		
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

    float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if (toVector == Vector3.zero)
        {
            return 0.0f;
        }
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
        angle *= Mathf.Deg2Rad;

        return angle;
    }
}
