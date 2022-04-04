using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float damage = 0f;

    [SerializeField][Tooltip("Life time of bullet until dissapear")]
    private float lifeTime = 10f;

    [SerializeField][Tooltip("Speed of bullet fire")]
    private float speed = 5.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        
        transform.Translate(Vector3.up * (speed * Time.deltaTime));

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
