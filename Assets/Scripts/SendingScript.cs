using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendingScript : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.5f;

    private float _maxY = 10f;
    void Update()
    {
        Vector3 direction = new Vector3(0, speed, 0);
        transform.Translate(direction * Time.deltaTime);

        speed += 0.01f;

        if (transform.position.y > _maxY)
        {
            Destroy(gameObject);
        }
    }
}
