using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerClass
{
    protected float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }

    protected float jumpHeight;
    public float JumpHeight { get { return jumpHeight; } }

    protected float reloadTime;
    public float ReloadTime { get { return reloadTime; } }

    protected float projectileSpeed;
    public float ProjectileSpeed { get { return projectileSpeed; } }

    protected float shootingDamage;
    public float ShootingDamage { get { return shootingDamage; } }

    protected float health;
    public float Health { get { return health; } }
}

public class Character_1Class : PlayerClass
{
    public Character_1Class()
    {
        moveSpeed = 300;
        jumpHeight = 625;
        reloadTime = 1;
        projectileSpeed = 15;
        shootingDamage = 60;
        health = 500;
    }
}