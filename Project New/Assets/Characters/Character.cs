using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject {

    public float moveSpeed;
    public float jumpHeight;
    public float reloadTime;
    public float projectileSpeed;
    public float shootingDamage;
    public float health;

}
