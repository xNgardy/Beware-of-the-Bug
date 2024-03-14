using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
   public int width = 16, height = 16;
   public int mineCounts = 32;

   private Board board;
   [SerializeField] private AudioClip clickSound;
   [SerializeField] private AudioClip explosionSound;
   private AudioSource audioSource;
   private Cell[,] state;
   private bool gameOver;

   private void Awake(){
    board = GetComponentInChildren<Board>();
    audioSource = GetComponent<AudioSource>();
   } 

   private void Start(){
    NewGame();
   }

   private void NewGame(){
    state = new Cell[width, height];
    gameOver = false;
    GenerateCells();
    GenerateMines();
    GenerateNumbers();
    Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
    board.Draw(state);
   }

   private void GenerateCells(){
    for(int i = 0; i < width; i++){
        for(int j = 0; j < height; j++){
            Cell cell = new Cell();
            cell.position = new Vector3Int(i, j, 0);
            cell.type = Cell.Type.Empty;
            state[i,j] = cell;
        }
    }
   }

   private void GenerateMines(){
    for (int i = 0; i < mineCounts; i++){

        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(0, height);

        while(state[x, y].type == Cell.Type.Mine){
            x++;

            if(x >= width){
                x = 0;
                y++;

                if(y >= height){
                    y = 0;
                }
            }
        }
        state[x, y].type = Cell.Type.Mine;
    }
   }

   private void GenerateNumbers(){
    for(int i = 0; i < width; i++){
        for(int j = 0; j < height; j++){
            Cell cell = state[i, j];

            if(cell.type == Cell.Type.Mine){
                continue;
            }

            cell.number = CountMines(i, j);

            if(cell.number > 0){
                cell.type = Cell.Type.Number;
            }

            state[i, j] = cell;
        }
    }
   }

   private int CountMines(int cellX, int cellY){
    int count = 0;

    for(int adjacentX = -1; adjacentX <= 1; adjacentX++){
        for(int adjacentY = -1; adjacentY <= 1; adjacentY++){

            if(adjacentX == 0 && adjacentY == 0){
                continue;
            }

            int x = cellX + adjacentX;
            int y = cellY + adjacentY;

            if(GetCell(x, y).type == Cell.Type.Mine){
                count++;
            }
        }
    }

    return count;
   }

   private void Update(){
    if(Input.GetKeyDown(KeyCode.R)){
        NewGame();
    }
    else if(!gameOver){
        if(Input.GetMouseButtonDown(1)){
            audioSource.clip = clickSound;
            audioSource.Play();
            Flag();
        }
        else if(Input.GetMouseButtonDown(0)){
            Reveal();
        }
    }
   }

   private void Flag(){
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
    Cell cell = GetCell(cellPosition.x, cellPosition.y);

    if(cell.type == Cell.Type.Invalid || cell.revealed){
        return;
    }

    cell.flagged = !cell.flagged;
    state[cellPosition.x, cellPosition.y] = cell;
    board.Draw(state);

   }

   private void Reveal(){
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
    Cell cell = GetCell(cellPosition.x, cellPosition.y);

    if(cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged){
        return;
    }

    switch(cell.type){
        case Cell.Type.Mine:
            Explode(cell);
            audioSource.clip = explosionSound;
            audioSource.Play();
            break;
        case Cell.Type.Empty:
            Flood(cell);
            audioSource.clip = clickSound;
            audioSource.Play();
            CheckWinCondition();
            break;
        default:
            cell.revealed = true;
            audioSource.clip = clickSound;
            audioSource.Play();
            state[cellPosition.x, cellPosition.y] = cell;
            CheckWinCondition();
            break;
    }

    board.Draw(state);

   }

   private void Flood(Cell cell){
    if(cell.revealed)
        return;
    if(cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid)
        return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if(cell.type == Cell.Type.Empty){
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
   }

   private void Explode(Cell cell){
    Debug.Log("Game Over!!!!!\n");
    gameOver = true;
    cell.revealed = true;
    cell.exploded = true;
    state[cell.position.x, cell.position.y] = cell;

    for(int i = 0; i < width; i++){
        for(int j = 0; j < height; j++){
            cell = state[i, j];

            if(cell.type == Cell.Type.Mine){
                cell.revealed = true;
                state[i, j] = cell;
            }
        }
    }
    Invoke("GameOver", 3f);
   }

   private void CheckWinCondition(){
    for(int i = 0; i < width; i++){
        for(int j = 0; j < height; j++){
            Cell cell = state[i, j];
            if(cell.type != Cell.Type.Mine && !cell.revealed){
                return;
            }
        }
    }

    Debug.Log("You are a Winner!!!!!\n");
    gameOver = true;

    for(int i = 0; i < width; i++){
        for(int j = 0; j < height; j++){
            Cell cell = state[i, j];

            if(cell.type == Cell.Type.Mine){
                cell.flagged = true;
                state[i, j] = cell;
            }
        }
    }

    Invoke("YouWin", 3f);

   }

   private Cell GetCell(int x, int y){
    if(IsValid(x, y)){
        return state[x, y];
    }
    else{
        return new Cell();
    }
   }

   private bool IsValid(int x, int y){
    return x >= 0 && x < width && y >= 0 && y < height;
   }

   private void GameOver(){
    SceneManager.LoadScene(2);
   }

   private void YouWin(){
    SceneManager.LoadScene(3);
   }
}
