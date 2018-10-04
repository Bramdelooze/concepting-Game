using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;
    public float reloadTime;
    public float projectileSpeed;
    public float shootingDamage;
    public float health;
    private float startHealth;

    public int direction;
    public Image healthBar;
    public Text reloadText;

    public Transform GroundCheck;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode shoot;

    private Rigidbody2D rb;

    public GameObject projectile;

    private bool isReloading;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startHealth = health;
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
        healthBar.fillAmount = health / startHealth;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        //health -= 12.75f * Time.deltaTime;

        if (Input.GetKeyDown(shoot) && !isReloading)
        {
            Fire();
            StartCoroutine(Reload());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Move(bool left)
    {
        if (!left)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            direction = 1;
            rb.AddForce(Vector2.right * moveSpeed);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            direction = -1;
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
        health -= shootingDamage;
        GameObject bullet = Instantiate(projectile, transform.position + offset, Quaternion.identity) as GameObject;
        Vector2 bulletDirection;

        if(direction == 1)
        {
            bulletDirection = Vector2.right * projectileSpeed;
        }
        else
        {
            bulletDirection = Vector2.left * projectileSpeed;
        }

        bullet.GetComponent<Rigidbody2D>().velocity += bulletDirection;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        reloadText.text = "Reloading...";
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        reloadText.text = "";
        StopCoroutine(Reload());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Bullet(Clone)")
        {
            health -= collision.GetComponent<Bullet>().damage;
        }
    }
}