using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int maxRooms;

    [HideInInspector] public bool playerTurn = true;

    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    // Start is called before the first frame update
    void Awake()
    {
        maxRooms = level * 2;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level, Vector2.zero);
    }

    public void IsGameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemy(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].timeToMove);
        }
        playerTurn = true;
        enemiesMoving = false;
    }
}
