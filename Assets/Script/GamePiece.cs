using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int x;
    private int y;

    public int X
    {
        get{return x;}
        set{
            if(IsMovable()){
                x = value;
            }
        }
    }
    public int Y
    {
        get{return y;}
        set{
            if(IsMovable()){
                y = value;
            }
        }
    }

    private PiecesGrid.PieceType type;
    public PiecesGrid.PieceType Type
    {
        get{return type;}
    }

    private PiecesGrid piecesGrid;
    public PiecesGrid PiecesGridRef
    {
        get{return piecesGrid;}
    }
    
    private MovablePiece movableComponent;
    public MovablePiece MovableComponent
    {
        get{return movableComponent;}
    }

    private ElementPiece elementComponent;
    public ElementPiece ElementComponent
    {
        get{return elementComponent;}
    }

    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent{
        get{return clearableComponent;}
    }

    void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        elementComponent = GetComponent<ElementPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init(int _x, int _y, PiecesGrid _piecesGrid, PiecesGrid.PieceType _type)
    {
        x = _x;
        y = _y;
        piecesGrid = _piecesGrid;
        type = _type;
    }
    void OnMouseEnter()
    {
        piecesGrid.EnterPiece(this);
    }
    void OnMouseDown()
    {
        piecesGrid.PressPiece(this);
    }
    void OnMouseUp()
    {
        piecesGrid.ReleasePiece(this);
    }
    public bool IsMovable()
    {
        return movableComponent != null;
    }
    public bool IsElement()
    {
        return elementComponent != null;
    }
    public bool IsClearable()
    {
        return clearableComponent != null;
    }
}
