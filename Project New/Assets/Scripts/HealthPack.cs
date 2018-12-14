using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour {

    private bool collided;

    private void FixedUpdate()
    {
        if (!collided)
        {
            HealthPackFalling();
        }
    }

    void HealthPackFalling()
    {
        transform.position += Vector3.down * .05f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            collided = true;
        }
    }
}
