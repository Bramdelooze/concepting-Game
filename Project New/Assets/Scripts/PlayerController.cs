using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public Character character;

    //[SerializeField]
    //private float moveSpeed;
    //[SerializeField]
    //private float jumpHeight;
    //[SerializeField]
    //private float reloadTime;
    //[SerializeField]
    //private float projectileSpeed;
    //[SerializeField]
    //private float shootingDamage;
    //[SerializeField]
    //private float health;
    private float health;

    private int direction;
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

    public LevelManager levelManager;

    private enum HorizontalMovementStates { STATIC, LEFT, RIGHT };
    private HorizontalMovementStates currentHorizontalMovementState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = character.health;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapBox(GroundCheck.position, new Vector2(.98f, 0f), 0, groundLayer);

        rb.velocity = Vector2.up * rb.velocity.y;

        if (isGrounded && Input.GetKey(jump))
        {
            Jump();
        }

        HandleInput();
        MovementUpdate();
    }

    private void Update()
    {
        healthBar.fillAmount = health / character.health;
        if (health <= 0)
        {
            levelManager.playerDied = true;
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

    private void HandleInput()
    {
        currentHorizontalMovementState = HorizontalMovementStates.STATIC;

        if (Input.GetKey(left))
        {
            currentHorizontalMovementState = HorizontalMovementStates.LEFT;
        }
        if(Input.GetKey(right))
        {
            currentHorizontalMovementState = HorizontalMovementStates.RIGHT;
        }
    }

    private void MovementUpdate()
    {
        if(currentHorizontalMovementState == HorizontalMovementStates.STATIC)
        {
            return;
        } else if(currentHorizontalMovementState == HorizontalMovementStates.LEFT)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            direction = -1;
            rb.AddForce(Vector2.left * character.moveSpeed);
        } else if(currentHorizontalMovementState == HorizontalMovementStates.RIGHT)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            direction = 1;
            rb.AddForce(Vector2.right * character.moveSpeed);
        }
        else
        {
            Debug.LogError("Unrecognised state " + currentHorizontalMovementState);
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * character.jumpHeight);
    }

    void Fire()
    {
        Vector3 offset = new Vector3(direction, 0, 0);
        health -= character.shootingDamage;
        GameObject bullet = Instantiate(projectile, transform.position + offset, Quaternion.identity) as GameObject;
        Vector2 bulletDirection;

        if(direction == 1)
        {
            bulletDirection = Vector2.right * character.projectileSpeed;
        }
        else
        {
            bulletDirection = Vector2.left * character.projectileSpeed;
        }
        bullet.GetComponent<Rigidbody2D>().velocity += bulletDirection;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        reloadText.text = "Reloading...";
        yield return new WaitForSeconds(character.reloadTime);
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