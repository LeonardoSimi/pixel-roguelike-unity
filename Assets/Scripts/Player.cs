using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MovingMob
{
    public float restartLevelDelay = 1f;

    private Animator animator;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }

    private void OnDisable()
    {
        //TODO save score
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playerTurn) return;

        int h = 0;
        int v = 0;

        h = (int)Input.GetAxisRaw("Horizontal");
        v = (int)Input.GetAxisRaw("Vertical");

        if (h != 0)
            v = 0;

        if (h != 0 || v != 0)
            AttemptToMove<Wall>(h, v);
    }

    protected override void AttemptToMove<T>(int xDir, int yDir)
    {
        base.AttemptToMove<T>(xDir, yDir);
        RaycastHit2D hit;
        CheckGameOver();

        GameManager.instance.playerTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }

        if (other.tag == "Door")
        {
            Debug.Log("porta presa");
        }    

        //TODO PICKUPS
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
    }

    public void LoseLife()
    {
        //TODO
    }

    private void Restart()
    { 
        //Application.LoadLevel(Application.loadedLevel); DEPRECATED
        SceneManager.LoadScene("LevelScene");
        
    }

    private void CheckGameOver()
    {
        //TODO if life 0 gamemanager.instance.gameover
    }
}
