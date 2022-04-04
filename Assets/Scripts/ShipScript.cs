using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    [SerializeField]
    private float health = 100f;

    private float _maxHealth;

    [SerializeField] private List<Sprite> destroySprites;

    [SerializeField] private GameObject sendingObject;

    private Transform _healthBar;

    // Start is called before the first frame update
    void Start()
    {
        _healthBar = transform.Find("HealthBar").Find("Health");
        _maxHealth = health;
    }


    public void AttackOneMe(float damage)
    {
        health -= damage;
        Vector3 scale = _healthBar.localScale;
        _healthBar.localScale = new Vector3(health > 0 ? health / _maxHealth : 0, scale.y, scale.z);
        if (health <= 0)
        {
            GlobalVariables.GameOver = true;
        }
    }

    public void Send()
    {
        Instantiate(sendingObject, new Vector3(0, 1.5f, 0), Quaternion.identity);
    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health > _maxHealth)
        {
            health = _maxHealth;
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
