using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int sharedLives = 3;
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
        if (players.Contains(player))
            return;

        players.Add(player);
    }

    public void PlayerDied(PlayerHealth player)
    {
        bool allDead = true;

        foreach (var p in players)
        {
            if (!p.isDead)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        sharedLives--;

        if (sharedLives <= 0)
        {
            Debug.Log("GAME OVER");
            return;
        }

        foreach (var p in players)
        {
            p.FullRevive();
        }
    }

    public void WaveCompleted()
    {
        foreach (var p in players)
        {
            if (p.isDead)
                p.ReviveWithOneHeart();
        }
    }
}
