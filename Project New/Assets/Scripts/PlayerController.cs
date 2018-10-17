using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    private float health;

    public Image healthBar;
    public Text reloadText;

    public Transform GroundCheck;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    [SerializeField]
    private KeyCode left;
    [SerializeField]
    private KeyCode right;
    [SerializeField]
    private KeyCode jump;
    [SerializeField]
    private KeyCode shoot;

    private Rigidbody2D rb;

    public GameObject projectile;

    private bool isReloading;

    public LevelManager levelManager;

    private enum HorizontalMovementStates { STATIC, LEFT, RIGHT };
    private HorizontalMovementStates currentHorizontalMovementState;

    private PlayerClass currentClass;

    private void Awake()
    {
        print(groundLayer.value);
        currentClass = new Character_1Class();
        rb = GetComponent<Rigidbody2D>();
        health = currentClass.Health;
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
        healthBar.fillAmount = health / currentClass.Health;
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
            rb.AddForce(Vector2.left * currentClass.MoveSpeed);
        } else if(currentHorizontalMovementState == HorizontalMovementStates.RIGHT)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            rb.AddForce(Vector2.right * currentClass.MoveSpeed);
        }
        else
        {
            Debug.LogError("Unrecognised state " + currentHorizontalMovementState);
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * currentClass.JumpHeight);
    }

    void Fire()
    {
        // The direction in which the player will shoot
        int direction;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            direction = -1;
        }
        else direction = 1;

        Vector3 offset = new Vector3(direction, 0, 0);
        health -= currentClass.ShootingDamage;
        GameObject bullet = Instantiate(projectile, transform.position + offset, Quaternion.identity) as GameObject;
        Vector2 bulletDirection;

        if(direction == 1)
        {
            bulletDirection = Vector2.right * currentClass.ProjectileSpeed;
        }
        else
        {
            bulletDirection = Vector2.left * currentClass.ProjectileSpeed;
        }
        bullet.GetComponent<Rigidbody2D>().velocity += bulletDirection;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        reloadText.text = "Reloading...";
        yield return new WaitForSeconds(currentClass.ReloadTime);
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