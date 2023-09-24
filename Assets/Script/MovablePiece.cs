using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece piece;
    private IEnumerator moveCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move(int newX, int newY, float time)
    {
        if(moveCoroutine != null){
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }
    private IEnumerator MoveCoroutine(int newX, int newY, float time){
        piece.X = newX;
        piece.Y = newY;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = piece.PiecesGridRef.GetWorldPosition(newX, newY);
        for(float t = 0; t <= time; t += Time.deltaTime){
            piece.transform.position = Vector3.Lerp(startPos, endPos, t/time);
            yield return null;
        }
        piece.transform.position = endPos;
    }
}
