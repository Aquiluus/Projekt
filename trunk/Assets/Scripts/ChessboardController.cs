using UnityEngine;
using System.Collections;

public enum Allegiance { none, white, black };

public class ChessboardController : MonoBehaviour {

    public ChessboardPlane[,] Board;


    void Awake()
    {
        Board = new ChessboardPlane[8, 8];
        initializeBoard();
    } 
    // Use this for initialization
    void Start()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < 8; i++)
        //{
        //    for (int j = 0; j < 8; j++)
        //    {
        //        if(Board[i,j].piece != null)
        //        Debug.Log(" on Board[" + i + "," + j + "]: " + Board[i,j].piece.Occupant.name);
        //    }
        //}



	}

    void initializeBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject temp = GameObject.Find("Plane_" + (i + 1) + (j + 1));
                //Debug.Log(temp.name);
                ChessboardPlane hooker = new ChessboardPlane(i + 1, j + 1, null, temp);
               // Debug.Log(" on Board[" + i + "," + j + "]: " + temp.name);
                GameObject particleSystem = (GameObject)Instantiate(Resources.Load("PlaneParticle"), hooker.plane.transform.position, hooker.plane.transform.rotation);
                particleSystem.transform.parent = hooker.plane.transform;
                particleSystem.SetActive(false);
                hooker.particleSystem = particleSystem;
                Board[i, j] = hooker;
            }
        }


    }
}

public class ChessboardPlane
{
    public int X;
    public int Y;
    public PieceController piece;
    public GameObject plane;
    public GameObject particleSystem;

    public ChessboardPlane()
    {
        this.X = 0;
        this.Y = 0;
        this.piece = null;
        this.plane = null;
        this.particleSystem = null;
    }

    public ChessboardPlane(int x, int y, PieceController piece, GameObject plane)
    {
        this.X = x;
        this.Y = y;
        this.piece = piece;
        this.plane = plane;
    }

    public ChessboardPlane(ChessboardPlane temp)
    {
        this.X = temp.X;
        this.Y = temp.Y;
        this.piece = temp.piece;
        this.plane = temp.plane;

    }

}