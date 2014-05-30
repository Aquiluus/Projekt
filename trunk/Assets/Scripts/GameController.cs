using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class GameController : MonoBehaviour {

    public ChessboardPlane lastPiece;
    public ChessboardPlane actualPiece;
    public ChessboardPlane to;
    public ChessboardPlane[] possibleDestinations;

    public Camera gameCamera;

    private ChessboardController ChessboardControllerScript;
    private GameController GameControllerScript;
    private Allegiance turnAlligiance = Allegiance.white;

    // TODO : camera animation
    private Vector3 cameraWhitePosition = new Vector3(215.0f, 535.0f, -327.5f);
    private Quaternion cameraWhiteRotation = Quaternion.Euler(new Vector3(45.0f, -90.0f, 0.0f));
    private Vector3 cameraBlackPosition = new Vector3(203.0f, 535.0f, -327.5f);
    private Quaternion cameraBlackRotation = Quaternion.Euler(new Vector3(45.0f, 90.0f, 0.0f));


    // Use this for initialization
    void Start()
    {
        ChessboardControllerScript = GameObject.Find("ChessboardController").GetComponent<ChessboardController>();
        GameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
        lastPiece = null;
        actualPiece = null;
        to = null;
        possibleDestinations = new ChessboardPlane[4];
    }

	
	// Update is called once per frame
	void Update () {

        PreTurnSetting();

        if (actualPiece != null && actualPiece.piece.Allegiance == turnAlligiance)
        {
            if (actualPiece.piece != null)
            {
                CalculateDestinatinos(actualPiece.piece.IsKing, AllegianceToInteger(actualPiece.piece.Allegiance));
            }
            ActivatePossibleDestinations();
            if (to != null && to.particleSystem.activeInHierarchy == true )
            {
                //TODO: movement Animation
                actualPiece.piece.transform.position = to.plane.transform.position;
                ClearPossibleDestinations();
                to = null;
                actualPiece = null;
                PostTurnSetting();
            }
            
        }


	}

    void ActivatePossibleDestinations()
    {
        foreach (ChessboardPlane possiblePlane in possibleDestinations)
        {
            if (possiblePlane != null && possiblePlane.piece == null)
            {
                possiblePlane.particleSystem.SetActive(true);
                possiblePlane.plane.GetComponent<TapGesture>().enabled = true;
            }
        }

    }


    public void ClearPossibleDestinations()
    {
        for (int i = 0; i < 4; i++)
        {
            if (possibleDestinations[i] != null)
            {
                possibleDestinations[i].particleSystem.SetActive(false);
                possibleDestinations[i].plane.GetComponent<TapGesture>().enabled = false;
                possibleDestinations[i] = null;
            }
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

    void PostTurnSetting()
    {
        if (turnAlligiance == Allegiance.white)
        {
            turnAlligiance = Allegiance.black;
            gameCamera.transform.rotation = cameraBlackRotation;
            gameCamera.transform.position = cameraBlackPosition;
        }
        else if (turnAlligiance == Allegiance.black)
        {
            turnAlligiance = Allegiance.white;
            gameCamera.transform.rotation = cameraWhiteRotation;
            gameCamera.transform.position = cameraWhitePosition;
        }

    }


    void CalculateDestinatinos(bool isKing, int colour)
    {
        int actualX = actualPiece.X - 1;
        int actualY = actualPiece.Y - 1;

        if ((actualX - 1) >= 0 && (actualY + colour) >= 0 && (actualY + colour) <= 7)
        {
            possibleDestinations[0] = ChessboardControllerScript.Board[actualX - 1, actualY + colour];
        }
        if ((actualX + 1) <= 7 && (actualY + colour) >= 0 && (actualY + colour) <= 7)
        {
            possibleDestinations[1] = ChessboardControllerScript.Board[actualX + 1, actualY + colour];
        }
        possibleDestinations[2] = null;
        possibleDestinations[3] = null;
        if(isKing)
        {
            if ((actualX - 1) >= 0 && (actualY - colour) >= 0 && (actualY - colour) <= 7)
            {
                possibleDestinations[2] = ChessboardControllerScript.Board[actualX - 1, actualY - colour];
            }
            if ((actualX + 1) <= 7 && (actualY - colour) >= 0 && (actualY - colour) <= 7)
            {
                possibleDestinations[3] = ChessboardControllerScript.Board[actualX + 1, actualY - colour];
            }
        }
        foreach (ChessboardPlane plane in possibleDestinations)
        {
            if (plane != null)
            {
                Debug.Log("Destination nr: " + plane.plane.ToString());
            }
            else
            {
                Debug.Log("Jestem nullem");
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
