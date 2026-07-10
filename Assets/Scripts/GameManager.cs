using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Tilemaps;


[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int sharedLives = 3;
    public WaveManager waveManager;
    public BoxCollider2D confiner;
    //public string targetTag = "Player";

    public int coinCount = 0;

    public List<PlayerHealth> players = new List<PlayerHealth>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPlayer(PlayerHealth player)
    {


        players.Add(player);

        //RefreshList();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        confiner = GameObject.FindGameObjectWithTag("Confiner").GetComponent<BoxCollider2D>();
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
            waveManager.ShowNewCoinUI(coinCount);
    }

    /*public void RefreshList()
    {
        players.Clear();

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        var filtered = allObjects.Where(o => o.CompareTag(targetTag));
        players.AddRange(filtered);
    }*/

    public void PlayerDied(PlayerHealth player)
    {
        player.gameObject.SetActive(false);
        
        bool allDead = true;

        /*foreach (var p in players)
        {
            if (!p.isDead)
            {
                allDead = false;
                break;
            }
        }*/

        if (allDead)
        {
            LoseLife();
            if (sharedLives > 0)
                LevelManager.instance.ReloadLevel();
        }
    }

    private void LoseLife()
    {
        sharedLives--;

        if (sharedLives <= 0)
        {
            Debug.Log("GAME OVER");
            SceneManager.LoadScene(0);
            return;
        }

        foreach (var p in players)
        {
            p.FullRevive();
        }

        //RefreshList();
    }

    public void WaveCompleted(WaveData.WaveType type)
    {
        switch (type)
        {
            case WaveData.WaveType.Boss:
                coinCount += 100;
                break;

            case WaveData.WaveType.Special:
                coinCount += 20;
                break;
        }

        waveManager.ShowNewCoinUI(coinCount);
    }


    public void AddCoinToCount()
    {
        coinCount++;
        waveManager.ShowNewCoinUI(coinCount);
    }
}
