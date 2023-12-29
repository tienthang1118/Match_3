using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesGrid : MonoBehaviour
{
    public GameManager gameManager;
    public enum PieceType 
    {
        EMPTY,
        NORMAL,
        CHAIN,
        ROW_CLEAR,
        COLUMN_CLEAR,
        MIGHTY,
        COUNT,
    };

    [System.Serializable]
    public struct PiecePrefab 
    {
        public PieceType type;
        public GameObject prefab;
    };

    private int MaxTargetPiece = 4;

    public int xDim;
    public int yDim;
    public float fillTime;
    
    public PiecePrefab[] piecePrefabs; 
    public GameObject backgroundPrefab;
    private Dictionary<PieceType, GameObject> piecePrefabDict;
    
    private GamePiece[,] pieces;
    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    private bool isSetUp = true;

    private List<ElementPiece.ElementType> targetElementTypes = new List<ElementPiece.ElementType>();
    private List<ElementPiece.ElementType> randomTargetElementTypes = new List<ElementPiece.ElementType>();

    private AudioManager audioManager;
    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        gameManager.Difficult = 0;
        InitializeAvailableElementTypes();

        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        //Them cac piecePrefabs vao dictionary
        for(int i=0; i<piecePrefabs.Length; i++){
            if(!piecePrefabDict.ContainsKey(piecePrefabs[i].type)){
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
    
        //Tao cac o background
        for(int x = 0; x < xDim; x++){
            for(int y = 0; y < yDim; y++){
                GameObject backgroud = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                backgroud.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, yDim];
        //Tao cac o empty
        for(int x = 0; x < xDim; x++){
            for(int y = 0; y < yDim; y++){
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }   
        }
        //Tao ChainPiece
        // Destroy(pieces[4, 4]);
        // SpawnNewPiece(4, 4, PieceType.CHAIN);

        StartCoroutine(StartGameFill());
        isSetUp = false;
    }
    IEnumerator StartGameFill()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Fill());
    }
    public IEnumerator Fill()
    {
        bool needsRefill = true;

        while(needsRefill){
            if(!isSetUp){
                yield return new WaitForSeconds(fillTime);
            }
            while (FillStep())
            {
                if(!isSetUp){
                    inverse = !inverse;
                    yield return new WaitForSeconds(fillTime);
                }
            }
            needsRefill = ClearAllValidMatches();
        }
        yield return new WaitForSeconds(fillTime);
        gameManager.UpdateDifficult();
        RandomizeTargetElementTypes(MaxTargetPiece - gameManager.Difficult);
        gameManager.DisplayTargetElementType(randomTargetElementTypes);
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        //Xet cac hang cua ma tran tu duoi len ngoai tru hang cuoi cung
        for(int y = yDim-2; y >= 0; y--)
        {
            //Xet cac o cua ma tran trong hang
            for(int loopX = 0; loopX < xDim; loopX++)
            {
                //neu !inverse x = loopX, nguoc lai x o vi tri doi xung voi loopX
                int x = loopX;
                if(inverse){
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];

                //Xet cac o co the di chuyen duoc
                if(piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y+1];

                    if(pieceBelow.Type == PieceType.EMPTY)
                    {
                        //Xoa bo EMPTY ben duoi va di chuyen piece hien tai xuong vi tri ben duoi, tao EMPTY o vi tri hien tai
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y+1, fillTime);
                        pieces[x, y+1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else{
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if(diag != 0)
                            {
                                //vi tri x cua piece o vi tri cheo voi piece hien tai
                                int diagX = x + diag;
                                if(inverse)
                                {
                                    diagX = x - diag;
                                }
                                if(diagX >= 0 && diagX < xDim)
                                {
                                    //piece o vi tri cheo ben duoi voi piece hien tai
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if(diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        //xet xem co piece co the di chuyen duoc hay piece empty o phia tren diagonalPiece khong
                                        bool hasPieceAbove = true;
                                        for(int aboveY = y; aboveY >=0; aboveY--)
                                        {
                                            //piece o tren diagonalPiece
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if(pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if(!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if(!hasPieceAbove)
                                        {
                                            //Xoa bo diagonalPiece va di chuyen pice hien tai xuong vi tri diagonalPiece
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            //Tao piece EMPTY o vi tri hien tai
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        //Duyet qua hang tren cung cua ma tran, them piece neu co piece EMPTY
        for(int x = 0; x < xDim; x++)
        {
            GamePiece piece = pieces[x, 0];
            if(piece.Type == PieceType.EMPTY)
            {
                Destroy(piece.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ElementComponent.SetElement((ElementPiece.ElementType)Random.Range(0, pieces[x, 0].ElementComponent.NumElements));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    //Lay vi tri moi cua cac o
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim/2.0f + x + 0.5f,
            transform.position.y + yDim/2.0f - y - 0.5f);
    }

    //Tao piece moi
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        //Tao piece 
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        //Gan grid là parent cua piece moi
        newPiece.transform.parent = transform;

        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        //Them thong tin cho piece
        pieces[x, y].Init(x, y, this, type);
        return pieces[x, y];
    }

    //Xet xem 2 piece co nam sat nhau khong
    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return(piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1
            ||(piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1));
    }
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if(piece1.IsMovable() && piece2.IsMovable()){
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;
            
            if(GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                || piece1.Type == PieceType.MIGHTY || piece2.Type == PieceType.MIGHTY)
            {
                audioManager.PlaySwapSound();
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);
            
                if(piece1.Type == PieceType.MIGHTY && piece1.IsElement() && piece2.IsElement())
                {
                    ClearMightyPiece clearElement = piece1.GetComponent<ClearMightyPiece>();
                    if(clearElement)
                    {
                        clearElement.Element = piece2.ElementComponent.Element;
                    }
                    ClearPiece(piece1.X, piece1.Y);
                }
                if(piece2.Type == PieceType.MIGHTY && piece1.IsElement() && piece2.IsElement())
                {
                    ClearMightyPiece clearElement = piece2.GetComponent<ClearMightyPiece>();
                    if(clearElement)
                    {
                        clearElement.Element = piece1.ElementComponent.Element;
                    }
                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();
                //Optional clear row and column mechanic
                // if(piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                // {
                //     ClearPiece(piece1.X, piece1.Y);
                // }

                // if(piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                // {
                //     ClearPiece(piece2.X, piece2.Y);
                // }

                pressedPiece = null;
                enteredPiece = null;

                StartCoroutine(Fill());
            }
            else{
                audioManager.PlaySwapErrorSound();
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }
    public void PressPiece(GamePiece piece){
        pressedPiece = piece;
    }
    public void EnterPiece(GamePiece piece){
        enteredPiece = piece;
    }
    public void ReleasePiece(GamePiece piece)
    {
        if(IsAdjacent(pressedPiece, enteredPiece)){
            SwapPieces(pressedPiece, enteredPiece);
        }
    }
    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        //Xet cac fruit piece
        if(piece.IsElement()){
            ElementPiece.ElementType element = piece.ElementComponent.Element;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //Them piece di chuyen vao list horizontal
            horizontalPieces.Add(piece);
            //Xet cac piece ben trai lan luot, neu giong nhau thi them vao list horizontal, neu khac thi break xet tiep cac piece ben phai
            for(int dir = 0; dir <=1; dir++){
                for(int xOffset = 1; xOffset < xDim; xOffset++){
                    int x;
                    if(dir == 0){
                        x = newX - xOffset;
                    }
                    else{
                        x = newX + xOffset;
                    }
                    if(x < 0 || x >= xDim){
                        break;
                    }
                    if(pieces[x, newY].IsElement() && pieces[x, newY].ElementComponent.Element == element){
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else{
                        break;
                    }
                }
            }
            //Neu list tu 3 tro len thi them vao matchingPieces va tra ve
            if(horizontalPieces.Count >= 3){
                for(int i = 0; i < horizontalPieces.Count; i++){
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }
            if(horizontalPieces.Count >= 3){
                for(int i = 0; i < horizontalPieces.Count; i++){
                    for(int dir = 0; dir <= 1; dir++){
                        for(int yOffset = 1; yOffset < yDim; yOffset++){
                            int y;
                            if(dir == 0){
                                y = newY - yOffset;
                            }
                            else{
                                y = newY + yOffset;
                            }
                            if(y < 0 || y >= yDim)
                            {
                                break;
                            }
                            if(pieces[horizontalPieces[i].X, y].IsElement() && pieces[horizontalPieces[i].X,y].ElementComponent.Element == element){
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else{
                                break;
                            }
                        }
                    }
                    if(verticalPieces.Count < 2){
                        verticalPieces.Clear();
                    }
                    else{
                        for(int j = 0; j < verticalPieces.Count; j++){
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if(matchingPieces.Count >= 3){
                return matchingPieces;
            }

            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for(int dir = 0; dir <= 1; dir++){
                for(int yOffset = 1; yOffset < yDim; yOffset++){
                    int y;
                    if(dir == 0){
                        y = newY - yOffset;
                    }
                    else{
                        y = newY + yOffset;
                    }
                    if(y < 0 || y >= xDim){
                        break;
                    }
                    if(pieces[newX, y].IsElement() && pieces[newX, y].ElementComponent.Element == element){
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else{
                        break;
                    }
                }
            }
            if(verticalPieces.Count >= 3){
                for(int i = 0; i < verticalPieces.Count; i++){
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            if(verticalPieces.Count >= 3){
                for(int i = 0; i < verticalPieces.Count; i++){
                    for(int dir = 0; dir <= 1; dir++){
                        for(int xOffset = 1; xOffset < xDim; xOffset++){
                            int x;
                            if(dir == 0){
                                x = newX - xOffset;
                            }
                            else{
                                x = newX + xOffset;
                            }
                            if(x < 0 || x >= xDim)
                            {
                                break;
                            }
                            if(pieces[x, verticalPieces[i].Y].IsElement() && pieces[x, verticalPieces[i].Y].ElementComponent.Element == element){
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else{
                                break;
                            }
                        }
                    }
                    if(horizontalPieces.Count < 2){
                        horizontalPieces.Clear();
                    }
                    else{
                        for(int j = 0; j < horizontalPieces.Count; j++){
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if(matchingPieces.Count >= 3){
                return matchingPieces;
            }
        }
        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for(int y = 0; y < yDim; y++){
            for(int x = 0; x < xDim; x++){
                if(pieces[x, y].IsClearable()){
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if(match != null){
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;
                        
                        if(match.Count == 4){
                            //Optional mechanic
                            // if(pressedPiece == null || enteredPiece == null)
                            // {
                            //     specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
                            // }   
                            // else if(pressedPiece.Y == enteredPiece.Y)
                            // {
                            //     specialPieceType = PieceType.ROW_CLEAR;
                            // }
                            // else
                            // {
                            //     specialPieceType = PieceType.COLUMN_CLEAR;
                            // }
                            specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR + 1);
                        }
                        else if(match.Count >= 5)
                        {
                            specialPieceType = PieceType.MIGHTY;
                        }
                        //clear piece
                        for(int i = 0; i < match.Count; i++){
                            if(ClearPiece(match[i].X, match[i].Y)){
                                needsRefill = true;

                                if(match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }
                        if(specialPieceType != PieceType.COUNT && !isSetUp)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if((specialPieceType == PieceType.ROW_CLEAR) || specialPieceType == PieceType.COLUMN_CLEAR
                                && newPiece.IsElement() && match[0].IsElement())
                            {
                                newPiece.ElementComponent.SetElement(match[0].ElementComponent.Element);
                            }
                            else if(specialPieceType == PieceType.MIGHTY && newPiece.IsElement())
                            {
                                newPiece.ElementComponent.SetElement(ElementPiece.ElementType.ANY);
                            }
                        }
                    }
                }
            }
        }
        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if(pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            audioManager.PlayClearPieceSound();
            if (!isSetUp && randomTargetElementTypes.Contains(pieces[x, y].ElementComponent.Element))
            {
                gameManager.IncreaseScore();
            }

            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            ClearObstacles(x, y);
            return true;
        }
        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        for(int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++){
            if(adjacentX != x && adjacentX >= 0 && adjacentX < xDim){
                if(pieces[adjacentX, y].Type == PieceType.CHAIN && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }
        for(int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++){
            if(adjacentY != y && adjacentY >= 0 && adjacentY < yDim){
                if(pieces[x, adjacentY].Type == PieceType.CHAIN && pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }
    public void ClearRow(int row)
    {
        for(int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }
    public void ClearColumn(int column)
    {
        for(int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }
    public void ClearMighty(ElementPiece.ElementType element)
    {
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                if(pieces[x, y].IsElement() && (pieces[x, y].ElementComponent.Element == element
                    || element == ElementPiece.ElementType.ANY))
                    {
                        ClearPiece(x, y);
                    }
            }
        }
    }
    void InitializeAvailableElementTypes()
    {
        targetElementTypes.Clear();
        targetElementTypes.Add(ElementPiece.ElementType.FIRE);
        targetElementTypes.Add(ElementPiece.ElementType.ICE);
        targetElementTypes.Add(ElementPiece.ElementType.WIND);
        targetElementTypes.Add(ElementPiece.ElementType.LIGHT);
        targetElementTypes.Add(ElementPiece.ElementType.DARK);
    }
    void RandomizeTargetElementTypes(int number)
    {
        randomTargetElementTypes = new List<ElementPiece.ElementType>();
        for (int i = 0; i < number; i++)
        {
            ElementPiece.ElementType randomElementType;
            while(true)
            {
                int randomIndex = Random.Range(0, targetElementTypes.Count);
                randomElementType = targetElementTypes[randomIndex];
                if(!randomTargetElementTypes.Contains(randomElementType))
                {
                    break;
                }
            }
            randomTargetElementTypes.Add(randomElementType);
        }
    }
    /*public void SpawnChainPiecesD2()
    {
        for(int x = 0; x < xDim; x++)
        {
            pieces[x, (yDim-1)/2].ClearableComponent.Clear();
            SpawnNewPiece(x, (yDim-1)/2, PieceType.CHAIN);
        }
    }
    public void SpawnChainPiecesD3()
    {
        for(int x = 0; x < xDim; x++)
        {
            pieces[x, (yDim-1)/2].ClearableComponent.Clear();
            SpawnNewPiece(x, (yDim-1)/2, PieceType.CHAIN);
        }
        for(int y = 0; y < yDim; y++)
        {
            pieces[(xDim-1)/2, y].ClearableComponent.Clear();
            SpawnNewPiece((xDim-1)/2, y, PieceType.CHAIN);
        }
    }*/
}
