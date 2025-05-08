using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridScript : MonoBehaviour
{
    static public int activeFunctions = 0;
    static public float score = 0;
    private int scoreCheck = 1000;
    static public int ROWS = 11;
    static public int COLUMNS = 6;
    [SerializeField] private float ANIMATION_DURATION = 0.001f; // Changed from extremely small value
    private bool loaded = false;
    static public int difficulty = 3;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject ScreenToLoad;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    [SerializeField] private Transform camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    static private byte[,] grid = new byte[COLUMNS, ROWS];
    static bool update;
    [SerializeField] private Sprite[] sprites = new Sprite[10];
    [SerializeField] private AudioClip[] audioC = new AudioClip[10];
    [SerializeField] private RuntimeAnimatorController[] anims = new RuntimeAnimatorController[10];
    [SerializeField] private Tile tile;
    [SerializeField] private AudioSource effectsSource; // Added centralized audio source
    
    private Dictionary<Vector2Int, Tile> tiles;
    private List<Match> matchList = new List<Match>(24); // Changed to List for better performance
    
    class Match {
        public int locX, locY, length;
        public bool verticle;
        public byte type;
        public Match(int X, int Y, int len, bool vert, byte iconType) {
            locX = X;
            locY = Y;
            length = len;
            verticle = vert;
            type = iconType;
        }
    }

    private void Awake()
    {
        activeFunctions = 0;
        score = 0;

        grid = new byte[COLUMNS, ROWS];
    }
    void Start()
    {
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        DOTween.SetTweensCapacity(200, 125);
        
        
        generateTiles();
        StartCoroutine(CheckTiles());
    }


    IEnumerator checkActive(){
        yield return new WaitForSeconds(0.1f);
        if(activeFunctions == 0){
            firstCheck();
            loaded = true;
        }
    }
    
    void Update()
    {
        if(!loaded)
            StartCoroutine(checkActive());
    }

    public Tile GetTileAtPos(Vector2Int pos) {
        if(tiles.TryGetValue(pos, out Tile tile)) 
            return tile;
        return null;
    }
    
    void generateTiles() {
        float startTime = Time.time;
        if(difficulty > 9)
            difficulty = 9;
        tiles = new Dictionary<Vector2Int, Tile>();
        
        Vector3 cameraPos = new Vector3(((float)ROWS / 2 - 0.5f)-2.5f, -((float)COLUMNS / 2 + 0.5f)-2.8f, -10);
        
        for (int iY = 0; iY < ROWS; iY++){
            for (int iX = 0; iX < COLUMNS; iX++){
                int typeNum;
                if(difficulty == 2){
                    typeNum = 1 + (iX + iY) % 2;
                } 
                else{
                    typeNum = Random.Range(1, difficulty+1);
                }
                Vector2Int tilePos = new (iX, iY);
                Vector3 spawnPos = new Vector3(iX, -iY, 0);
                var spawnTile = Instantiate(tile, spawnPos, Quaternion.identity);
                spawnTile.name = $"Tile {iX} {iY}";
                spawnTile.iconNum = typeNum;
                grid[iX, iY] = (byte)spawnTile.iconNum;
                
                var renderer = spawnTile.GetComponent<SpriteRenderer>();
                renderer.sprite = sprites[spawnTile.iconNum];
                
                var animator = spawnTile.GetComponent<Animator>();
                animator.runtimeAnimatorController = anims[spawnTile.iconNum];
                
                tiles[tilePos] = spawnTile;
            }
        }
        float endTime = Time.time;
        Debug.Log($"generateTiles {(endTime-startTime)} seconds elapsed");
        camera.transform.position = cameraPos;
    }

    IEnumerator CheckTilesX(){
        activeFunctions += 1;
        float startTime = Time.time;
        // Process matches in batches to reduce yield returns
        for (int iY = 0; iY < ROWS; iY++) {
            int processedInBatch = 0;
            
            for (int iX = 0; iX < COLUMNS-2; iX++) { 
                if(grid[iX,iY] == grid[iX+1,iY] && grid[iX,iY] == grid[iX+2,iY]){
                    byte target = grid[iX,iY];
                    int count = 3;
                    
                    for(int i = iX+3; i < COLUMNS; i++) {
                        if(grid[i,iY] == target){ count++; }
                        else { break; }
                    }
                    
                    matchList.Add(new Match(iX, iY, count, false, target));
                    processedInBatch++;
                    
                    if(iX+count > COLUMNS-2) break;
                    else iX += count - 1; // -1 because the loop will increment iX
                }
            }
            
            // Only yield once per row or after processing multiple matches
            if(processedInBatch > 0 || iY % 3 == 2)
                yield return null;
        }
        float endTime = Time.time;
        Debug.Log($"checkTilesY {(endTime-startTime)} seconds elapsed");
        activeFunctions -= 1;
    }

    public void swapTiles(Vector2 orig, Vector2 dest){
        int oX, oY, dX, dY;
        oX = (int)orig.x; oY = (int)orig.y; dX = (int)dest.x; dY = (int)dest.y;
        var temp = grid[oX, oY];
        grid[oX, oY] = grid[dX, dY];
        grid[dX, dY] = temp;

        if (checkSwap())
            refreshTiles();
        else{
            temp = grid[dX, dY];
            grid[dX, dY] = grid[oX, oY];
            grid[oX, oY] = temp;
            refreshTiles();
        }
    }

    bool checkSwap(){
        int tempMatCount = 0;
        
        // Check vertical matches
        for (int iX = 0; iX < COLUMNS; iX++) { 
            for (int iY = 0; iY < ROWS-2; iY++) {
                if(grid[iX,iY] == grid[iX,iY+1] && grid[iX,iY] == grid[iX,iY+2]){
                    int target = grid[iX,iY];
                    int count = 3;
                    for(int i = iY+3; i < ROWS; i++) {
                        if(grid[iX,i] == target){ count++; }
                        else { break; }
                    }
                    tempMatCount++;
                    if(iY+count > ROWS-2)
                        break;
                    else iY += count - 1;
                }
            }
        }
        
        // Check horizontal matches
        for (int iY = 0; iY < ROWS; iY++) {
            for (int iX = 0; iX < COLUMNS-2; iX++) { 
                if(grid[iX,iY] == grid[iX+1,iY] && grid[iX,iY] == grid[iX+2,iY]){
                    int target = grid[iX,iY];
                    int count = 3;
                    for(int i = iX+3; i < COLUMNS; i++) {
                        if(grid[i,iY] == target){ count++; }
                        else { break; }
                    }
                    tempMatCount++;
                    if(iX+count > COLUMNS-2)
                        break;
                    else iX += count - 1;
                }
            }
        }
        
        return tempMatCount > 0;
    }
    
    IEnumerator CheckTilesY(){
        activeFunctions += 1;
        float startTime = Time.time;
        // Process matches in batches to reduce yield returns
        for (int iX = 0; iX < COLUMNS; iX++) { 
            int processedInBatch = 0;
            
            for (int iY = 0; iY < ROWS-2; iY++) {
                if(grid[iX,iY] == grid[iX,iY+1] && grid[iX,iY] == grid[iX,iY+2]){
                    byte target = grid[iX,iY];
                    int count = 3;
                    
                    for(int i = iY+3; i < ROWS; i++) {
                        if(grid[iX,i] == target){ count++; }
                        else { break; }
                    }
                    
                    matchList.Add(new Match(iX, iY, count, true, target));
                    processedInBatch++;
                    
                    if(iY+count > ROWS-2) break;
                    else iY += count - 1; // -1 because the loop will increment iY
                }
            }
            
            // Only yield once per column or after processing multiple matches
            if(processedInBatch > 0 || iX % 2 == 1)
                yield return null;
        }
        float endTime = Time.time;
        Debug.Log($"CheckTilesY {(endTime-startTime)} seconds elapsed");
        Debug.Log($"Number of Matches = {matchList.Count}");
        activeFunctions -= 1;
    }

    void refreshTiles(){
        float startTime = Time.time;
        // Use a StringBuilder to avoid string concatenation GC overhead
        for (int iY = 0; iY < ROWS; iY++) {
            for (int iX = 0; iX < COLUMNS; iX++) { 
                Vector2Int tilePos = new Vector2Int(iX, iY);
                var tile = GetTileAtPos(tilePos);
                
                // Update position once instead of separately
                tile.transform.position = new Vector3(iX, -iY, 0);
                
                var tempTile = tiles[tilePos];
                tempTile.iconNum = grid[iX,iY];
                
                var renderer = tempTile.GetComponent<SpriteRenderer>();
                renderer.sprite = sprites[tempTile.iconNum];
                
                var animator = tempTile.GetComponent<Animator>();
                animator.runtimeAnimatorController = anims[tempTile.iconNum];
                
                // Set light intensity directly instead of tweening
                tile.GetComponentInChildren<Light>().intensity = 1.2f;
                animator.Play("Idle");
            }
        }
        float endTime = Time.time;
        Debug.Log($"refreshTiles {(endTime-startTime)} seconds elapsed");
        StartCoroutine(CheckTiles());
    }

    // Optimized to use StringBuilder for debugging
    void debugGrid(){
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        System.Text.StringBuilder debugGrid = new System.Text.StringBuilder();
        for (int iY = 0; iY < ROWS; iY++) {
            for (int iX = 0; iX < COLUMNS; iX++) { 
                debugGrid.Append(grid[iX,iY]);
                debugGrid.Append(' ');
            }
            debugGrid.AppendLine();
        }
        Debug.Log(debugGrid.ToString());
        #endif
    }

    IEnumerator showScore(Tile tile, int score) {

        TMP_Text txt = tile.GetComponentInChildren<TMP_Text>();

        txt.text = $"+{score}";
        txt.enabled = true;
        yield return new WaitForSeconds(ANIMATION_DURATION+0.4f);
        txt.enabled = false;
    }
    
    IEnumerator lightUp(Tile tile){

        Light point = tile.GetComponentInChildren<Light>();
        
        // Cache target values to reduce allocations
        float startIntensity = point.intensity;
        float maxIntensity = startIntensity + 2f;
        float halfDuration = ANIMATION_DURATION/2;
        
        //point.DOKill(); // Kill any existing tweens
        point.DOIntensity(maxIntensity, halfDuration);
        yield return new WaitForSeconds(halfDuration);
        //point.DOKill(); // Kill any existing tweens
        point.DOIntensity(startIntensity, halfDuration);
    }

    // Optimized to handle matches in batches
     IEnumerator processMatches(){
        activeFunctions += 1;
        float startTime = Time.time;
        int matches = matchList.Count;
        for(int i = 0; i < matches; i++) {
            //Debug.Log($"DEBUG Match number: {i+1} Location(x, y): ({matchList[i].locX}, {matchList[i].locY}) Size: {matchList[i].length} Direction: {(matchList[i].verticle?"Verticle":"Horizontal")}\n");
            int calScore = (matchList[i].type+difficulty)*matchList[i].length;
            score += calScore;
            if(score > scoreCheck && loaded){
                difficulty++;
                if(difficulty > 9)
                    difficulty = 9;
                scoreCheck += 1000;
            }

            //Debug.Log ($"Score = {score}");
            if(matchList[i].verticle){
                for(int p = matchList[i].locY; p < (matchList[i].length+matchList[i].locY); p++) {
                    int type = grid[matchList[i].locX, p];
                    var pos = new Vector2Int(matchList[i].locX, p);
                    var matTile = GetTileAtPos(pos);
                    yield return null; 
                    matTile.GetComponent<Animator>().SetTrigger("Pop");
                    if(p == (matchList[i].length+matchList[i].locY)/2)
                        StartCoroutine(showScore(matTile, calScore));
                    StartCoroutine(lightUp(matTile));
                    if(loaded)
                        AudioSource.PlayClipAtPoint(audioC[type], new Vector3(1, 1, 1));
                    grid[matchList[i].locX,p] = 0;
                }
            }else{
                for(int p = matchList[i].locX; p < (matchList[i].length+matchList[i].locX); p++) {
                    int type = grid[matchList[i].locX, p];
                    var pos = new Vector2Int(p, matchList[i].locY);
                    var matTile = GetTileAtPos(pos);
                    yield return null;
                    GetTileAtPos(pos).GetComponent<Animator>().SetTrigger("Pop");
                    matTile.GetComponent<Animator>().SetTrigger("Pop");
                    StartCoroutine(lightUp(matTile));
                    if(p == (matchList[i].length+matchList[i].locX)/2)
                        StartCoroutine(showScore(matTile, calScore));
                    if(loaded)
                        AudioSource.PlayClipAtPoint(audioC[type], new Vector3(1, 1, 1));
                    grid[p, matchList[i].locY] = 0;
                }
            }
        }


        yield return new WaitForSeconds(ANIMATION_DURATION);
        addGravity();
        activeFunctions -= 1;
        float endTime = Time.time;
        Debug.Log($"processMatches {(endTime-startTime)} seconds elapsed");
    }
    
    // Optimized to process columns in batches
    IEnumerator MoveTilesDown() {
        activeFunctions += 1;
        float startTime = Time.time;
        List<Transform> tilesToMove = new List<Transform>();
        List<Vector3> targetPositions = new List<Vector3>();
        
        for(int iX = 0; iX < COLUMNS; iX++) {
            // Skip columns with no changes needed
            bool columnHasChanges = false;
            for(int iY = 0; iY < ROWS; iY++) {
                if(grid[iX, iY] == 0) {
                    columnHasChanges = true;
                    break;
                }
            }
            
            if(!columnHasChanges) continue;
            
            // Process each column in a batch
            for(int iY = 0; iY < ROWS; iY++) {
                Vector2Int pos = new Vector2Int(iX, iY);
                var tileTran = GetTileAtPos(pos)?.transform;
                
                if(tileTran != null) {
                    Vector3 targetPos = new Vector3(iX, -iY, 0);
                    if((tileTran.position - targetPos).sqrMagnitude > 0.01f) {
                        tilesToMove.Add(tileTran);
                        targetPositions.Add(targetPos);
                    }
                }
            }
            
            yield return null; // Yield once per column to keep responsiveness
        }
        
        // Move all tiles in batches
        if(tilesToMove.Count > 0) {
            // Create a single sequence for all movements
            Sequence seq = DOTween.Sequence();
            for(int i = 0; i < tilesToMove.Count; i++) {
                seq.Join(tilesToMove[i].DOMove(targetPositions[i], ANIMATION_DURATION));
            }
            
            // Wait for all movements to complete
            yield return seq.WaitForCompletion();
        }
        refreshTiles();
        float endTime = Time.time;
        Debug.Log($"MoveTilesDown {(endTime-startTime)} seconds elapsed");
        activeFunctions -= 1;
    }

    void addGravity() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugGrid();
        #endif
        
        // First pass: Move existing tiles down
        for(int iX = 0; iX < COLUMNS; iX++) {
            int writePos = ROWS - 1; // Start from bottom
            
            // Move all non-empty cells to the bottom
            for(int iY = ROWS - 1; iY >= 0; iY--) {
                if(grid[iX, iY] != 0) {
                    // If we found a non-empty cell, move it to the lowest available position
                    if(writePos != iY) {
                        grid[iX, writePos] = grid[iX, iY];
                        grid[iX, iY] = 0;
                    }
                    writePos--; // Move write position up
                }
            }
            
            // Second pass: Fill empty cells at the top with random values
            for(int iY = writePos; iY >= 0; iY--) {
                grid[iX, iY] = (byte)Random.Range(1, difficulty+1);
            }
        }
        
        // Start a single coroutine to handle all tile movements
        StartCoroutine(MoveTilesDown());
        
    }
    
    void firstCheck(){
        ANIMATION_DURATION = 0.3f;
        score = 0;
        loading.GetComponent<UnityEngine.UI.Image>().enabled = false;
        ScreenToLoad.SetActive(true);
    }
    
    IEnumerator CheckTiles() {
        activeFunctions += 1;
        float startTime = Time.time;
        
        // Clear previous matches
        matchList.Clear();
        
        // Run checks concurrently
        var checkY = StartCoroutine(CheckTilesY()); 
        var checkX = StartCoroutine(CheckTilesX());
        
        yield return checkY;
        yield return checkX;
        
        float endTime = Time.time;
        Debug.Log($"checkTiles {(endTime-startTime)} seconds elapsed");
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        string matchDetails = "";
        for(int i = 0; i < matchList.Count; i++) {
            Match match = matchList[i];
            matchDetails += $"DEBUG Match number: {i+1} Location(x, y): ({match.locX}, {match.locY}) Size: {match.length} Direction: {(match.verticle?"Verticle":"Horizontal")}\n";
        }
        if(matchList.Count > 0) 
            Debug.Log(matchDetails);
        #endif
        
        if(matchList.Count > 0) 
            StartCoroutine(processMatches());
            
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugGrid();
        #endif
        
        activeFunctions -= 1;
    }
}