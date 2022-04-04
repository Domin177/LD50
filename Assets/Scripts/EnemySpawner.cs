using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField][Tooltip("What type of enemies can spawn")]
    private List<GameObject> enemies;

    [SerializeField]
    private PlayerScript player;

    [SerializeField][Tooltip("Size of game area on X axis")]
    private float gameAreaX = 12f;

    [SerializeField][Tooltip("Size of game area on Y axis")]
    private float gameAreaY = 6f;

    private float _nextSpawn;
    
    private float spawnRate = 12f;
    private float _spawnTimeDecay;
    private float _decayModifier = 1f;

    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;

        _spawnTimeDecay = _decayModifier - (player.GetLevel() / 15f);

        if (_spawnTimeDecay <= 0.09)
        {
            _spawnTimeDecay = 0.09f;
        }
        
        if (Time.time > _nextSpawn)
        {
            _nextSpawn = Time.time + (spawnRate * _spawnTimeDecay);
            Instantiate(GetRandomEnemy(), GetRandomPosition(), Quaternion.identity);
        }
    }

    private GameObject GetRandomEnemy()
    {
        int to = player.GetLevel() - 1;
        if (enemies.Count < to)
        {
            to = enemies.Count;
        }
        return enemies[Random.Range(0, to)];
    }

    private Vector3 GetRandomPosition()
    {
        float side = Random.Range(0f, 1f);
        float subSide = Random.Range(0f, 1f);

        float posX;
        float posY;
        
        if (side > 0.5) //top/bottom
        {
            posY = -gameAreaY;
            posX = Random.Range(-gameAreaX, gameAreaX);
        }
        else
        {
            if (subSide > 0.5)
            {
                posX = gameAreaX;
            }
            else
            {
                posX = -gameAreaX;
            }
            posY = Random.Range(-gameAreaY, gameAreaY + 2);
        }

        return new Vector3(posX, posY, player.transform.position.z);
    }
}
