using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;
    private float health = 500;
    public Image healthBar;

    public bool isGrounded = false;
    public Transform GroundCheck;
    public LayerMask groundLayer;

    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode shoot;

    Rigidbody2D rb;
    public GameObject projectile;
    private float projectileSpeed;
    private int direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, 0.35f, groundLayer);

        rb.velocity = Vector2.up * rb.velocity.y;

        if (Input.GetKeyDown(jump) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKey(right))
        {
            Move(false);
        }

        if (Input.GetKey(left))
        {
            Move(true);
        }
    }

    private void Update()
    {
        healthBar.fillAmount = health / 500;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        health -= 12.75f * Time.deltaTime;

        if (Input.GetKeyDown(shoot))
        {
            Fire();
        }
    }

    public void Move(bool left)
    {
        if (!left)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            direction = 1;
            projectileSpeed = 700 * Time.deltaTime;
            rb.AddForce(Vector2.right * moveSpeed);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            direction = -1;
            projectileSpeed = -700 * Time.deltaTime;
            rb.AddForce(Vector2.left * moveSpeed);
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpHeight);
    }

    void Fire()
    {
        Vector3 offset = new Vector3(direction, 0, 0);
        health -= 20f;

        GameObject bullet = Instantiate(projectile, transform.position + offset, Quaternion.identity) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity += new Vector2(projectileSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Bullet(Clone)")
        {
            health -= collision.GetComponent<Bullet>().damage;
        }
    }
}