using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject player;
    private GameObject currentPlayer;
    private CameraManager cam;
    private Vector3 checkpoint;

    public static int levelCount;
    public static int currentLevel;

    void Start()
    {
        cam = GetComponent<CameraManager>();

        if (GameObject.FindGameObjectWithTag("Spawn"))
        {
            checkpoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        }
        SpawnPlayer(checkpoint);
        Debug.Log(currentPlayer);
    }

    private void SpawnPlayer(Vector3 spawnPos)
    {
        currentPlayer = Instantiate(player, spawnPos, Quaternion.identity);
        cam.SetTarget(currentPlayer.transform);
    }

    void Update()
    {
        //Debug.Log(currentPlayer);
        if(currentPlayer is null)
        {
            if(Input.GetButtonDown("Respawn"))
            {
                SpawnPlayer(checkpoint);
            }
        }
    }

    public void SetCheckPoint(Vector3 cp)
    {
        checkpoint = cp;
    }

    public void EndLevel()
    {
        if(currentLevel < levelCount)
        {
            currentLevel ++;
            SceneManager.LoadScene("Level " +currentLevel);
        }
        else
        {
            Debug.Log("Game Over");
        }
    }
}
