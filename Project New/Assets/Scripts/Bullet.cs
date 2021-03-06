﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float damage = 100;

    public float GetDamage()
    {
        return damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground" || collision.tag == "Player" || collision.tag == "BulletDestroyer")
        Destroy(gameObject);
    }
}