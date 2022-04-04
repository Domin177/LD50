using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class FenceScript : MonoBehaviour
{
    private bool _buildingState = true;
    private Rigidbody2D rigidbody;

    private Camera _camera;
    private bool _rotating;

    private List<EnemyScript> _enemies = new List<EnemyScript>();

    [SerializeField] private float health = 20;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.simulated = false;
        
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        
        if (_buildingState)
        {
            SetPosition();
            Rotate();
        }
    }

    private void SetPosition()
    {
        Vector3 position = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.z * -1));
        position.z = 0;
        transform.position = position;
    }
    
    public void AddAttacker(EnemyScript enemyScript)
    {
        _enemies.Add(enemyScript);
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _rotating = true;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            _rotating = false;
        }

        if (_rotating)
        {
            transform.Rotate(new Vector3(0, 0, 1), 0.6f);
        }
    }

    public void Put()
    {
        _buildingState = false;
        rigidbody.simulated = true;
        PathScanner.UpdateFences(1,false);
    }
    
    public void AttackOneMe(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            PathScanner.UpdateFences(-1,false);
            foreach (EnemyScript enemy in _enemies)
            {
                if (enemy != null)
                {
                    enemy.StopFenceAttack();
                }
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}
