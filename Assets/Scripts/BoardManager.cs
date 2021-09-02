using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 10;
    public int rows = 9;
    public Count wallCount = new Count(5, 9); //min and max walls
    public Count pickupCount = new Count(1, 3);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] pickupTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject door;

    private Transform boardHolder;
    private Transform room;
    private List<Vector3> gridPositions = new List<Vector3>();
    private GameObject[] totWalls;
    [SerializeField]
    private List<Vector2> doors;
    [SerializeField]
    private Vector2 roomPos;
    private float roomsDelay = 0.4f;


    void InitializeList()
    {
        gridPositions.Clear();
        for (roomPos.x = 1; roomPos.x < columns - 1; roomPos.x++)
        {
            for (roomPos.y = 1; roomPos.y < rows - 1; roomPos.y++)
            {
                gridPositions.Add(new Vector3(roomPos.x, roomPos.y, 0f));
            }
        }
    }

    private void Awake()
    {
        if (doors.Count == 0)
        {
            Debug.Log(doors.Count);
            StartCoroutine(GetDoors(0));
        }
    }

    void BoardSetup(Vector2 roomPos)
    {
        //boardHolder = new GameObject("Board").transform;
        room = new GameObject("Room").transform;
        room.tag = "Room";

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y + roomPos.y, 0), Quaternion.identity) as GameObject;
                instance.transform.SetParent(room);
            }
        }
        Vector2 pos = new Vector2(DoorPosition().x, DoorPosition().y);
        Instantiate(door, pos, Quaternion.identity, room);
        //DungeonGen();
    }


    public Vector2 DoorPosition()
    {
        totWalls = GameObject.FindGameObjectsWithTag("OuterWall");
        GameObject chosenWall = totWalls[Random.Range(0, totWalls.Length - 1)];
        Vector2 DoorPosition = new Vector2(chosenWall.transform.position.x, chosenWall.transform.position.y);
        return DoorPosition;
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            Instantiate(tileChoice, randomPosition, Quaternion.identity, room);
        }
    }

    public void SetupScene(int level, Vector2 roomPos)
    {
        BoardSetup(Vector2.zero);
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(pickupTiles, pickupCount.minimum, pickupCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity, room);

        StartCoroutine(DungeonGen());
    }



    IEnumerator DungeonGen()
    {
        if (GameObject.Find("Room") != null)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(roomsDelay);
                BoardSetup(roomPos = new Vector2(roomPos.x, doors[i].y + 10));
                StartCoroutine(GetDoors(i+1));
                //Debug.Log(roomPos);
            }
        }
        else
            StartCoroutine(DungeonGen());
    }

    IEnumerator GetDoors(int i)
    {
        yield return new WaitForSeconds(roomsDelay - .1f);
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            doors.Add(door.transform.position);
            doors = doors.Distinct().ToList(); //remove duplicates
            Debug.Log(doors);
        }
    }

}
