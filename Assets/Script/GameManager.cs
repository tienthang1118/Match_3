using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public GameObject timeValueBar;
    public GameObject scoreValueBar;
    public GameObject resultWindow;
    public TMP_Text txtResult;
    public PiecesGrid piecesGrid;
    public List<GameObject> targets = new List<GameObject>();
    [SerializeField] private float timeLimit;
    [SerializeField] private float scoreLimit;
    private float timePassed;
    private float score;
    private bool isSpawnChainsD2;
    private bool isSpawnChainsD3;
    private int difficult;
    public int Difficult
    {
        set{difficult = value;}
        get{return difficult;}
    }
    void Start()
    {
        Time.timeScale = 1f;
    }
    void Update()
    {
        timePassed += Time.deltaTime;
        UpdateTimeBar();
    }

    void UpdateTimeBar()
    {
        float scaleX = Mathf.Max(0, (timeLimit - timePassed) / timeLimit);
        timeValueBar.transform.localScale = new Vector3(scaleX, timeValueBar.transform.localScale.y, timeValueBar.transform.localScale.z);
        if(timePassed > timeLimit)
        {
            Lose();
        }
    }
    void UpdateScoreBar()
    {
        float scaleX = Mathf.Min(1, score / scoreLimit);
        scoreValueBar.transform.localScale = new Vector3(scaleX, scoreValueBar.transform.localScale.y, scoreValueBar.transform.localScale.z);
        if(score >= scoreLimit)
        {
            StartCoroutine(Win());
        }
    }
    void Lose()
    {
        Time.timeScale = 0f;
        resultWindow.SetActive(true);
        txtResult.text = "YOU LOSE";
    }
    IEnumerator  Win()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
        resultWindow.SetActive(true);
        txtResult.text = "YOU WIN";
    }

    public void UpdateDifficult()
    {
        if((float)(score/scoreLimit) >= (float)2/3)
        {
            if(!isSpawnChainsD3)
            {
                difficult = 3;
                piecesGrid.SpawnChainPiecesD3();
                isSpawnChainsD3 = true;
            }
        }
        else if((float)(score/scoreLimit) >= (float)1/3)
        {
            if(!isSpawnChainsD2)
            {
                difficult = 2;
                piecesGrid.SpawnChainPiecesD2();
                isSpawnChainsD2 = true;
            }
        }
    }
    public void IncreaseScore()
    {
        score++;
        UpdateScoreBar();
        IncreaseTime();
    }
    public void IncreaseTime()
    {
        timePassed = Mathf.Max(0, timePassed - 0.25f);
    }
    public void DisplayTargetElementType(List<ElementPiece.ElementType> randomTargetElementTypes)
    {
        foreach(GameObject target in targets)
        {
            target.SetActive(false);
            if(randomTargetElementTypes.Contains((ElementPiece.ElementType)System.Enum.Parse( typeof(ElementPiece.ElementType), target.tag )))
            {
                target.SetActive(true);
            }
        }
    }
}
