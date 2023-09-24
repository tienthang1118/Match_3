using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearMightyPiece : ClearablePiece
{
    private ElementPiece.ElementType element;

    public ElementPiece.ElementType Element
    {
        get{return element;}
        set{element = value;}
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Clear()
    {
        base.Clear();
        piece.PiecesGridRef.ClearMighty(element);
    }
}
