using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private float health;

    public Image healthBar;
    [SerializeField] private Image powerBar;
    public Text reloadText;

    public Transform GroundCheck;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    [SerializeField] private KeyCode left;
    [SerializeField] private KeyCode right;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode shoot;
    [SerializeField] private KeyCode createShield;
    [SerializeField] private KeyCode ignorePlatform;

    [SerializeField] private string horizontal;
    [SerializeField] private string vertical;
    [SerializeField] private KeyCode joystickJump;
    [SerializeField] private KeyCode joystickShoot;
    [SerializeField] private KeyCode joystickCreateShield;
    [SerializeField] private KeyCode joystickIgnorePlatform;

    private Rigidbody2D rb;

    public GameObject projectile;
    public GameObject shieldPrefab;

    private bool isReloading;

    public LevelManager levelManager;

    private enum HorizontalMovementStates { STATIC, LEFT, RIGHT };
    private HorizontalMovementStates currentHorizontalMovementState;

    private PlayerClass currentClass;

    private bool isJumping = false;
    private float jumpTimeCounter;

    private bool shieldEquipped;
    private bool canEquipShield = true;
    private bool shieldReloading;

    private float shieldDeployTime;
    private float shieldReloadTime;

    private GameObject shield;

    private void Awake()
    {
        currentClass = new Character_2Class();

        rb = GetComponent<Rigidbody2D>();

        health = currentClass.Health;

        shieldDeployTime = currentClass.ShieldDeployTime;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapBox(GroundCheck.position, new Vector2(1.2f, 0f), 0, groundLayer);

        rb.velocity = Vector2.up * rb.velocity.y;

        HandleInput();
        MovementUpdate();

        if (Input.GetKey(ignorePlatform) || Input.GetKey(joystickIgnorePlatform))
        {
            RaycastHit2D hitL = Physics2D.Raycast(transform.position + new Vector3(-.58f, 0f), Vector2.down, 1.1f, groundLayer);
            RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(.54f, 0f), Vector2.down, 1.1f, groundLayer);
            if (hitL.collider != null && hitL.collider.tag == "Platform")
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitL.collider, true);
                StartCoroutine(DontIgnorePlatform(hitL.collider));
            }
            if (hitR.collider != null && hitR.collider.tag == "Platform")
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitR.collider, true);
                StartCoroutine(DontIgnorePlatform(hitR.collider));
            }
        }
    }

    private void Update()
    {
        if(GetComponent<Animator>() != null)
            GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(rb.velocity.x)); 
        healthBar.fillAmount = health / currentClass.Health;

        if (health <= 0)
        {
            levelManager.playerDied = true;
            Destroy(gameObject);
        }

        if ((Input.GetKeyDown(jump) || Input.GetKeyDown(joystickJump)) && isGrounded)
        {
            isJumping = true;

            jumpTimeCounter = currentClass.MaxJumpTime;

            Jump();
        }

        if ((Input.GetKey(jump) || Input.GetKey(joystickJump)) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                Jump();
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(jump) || Input.GetKeyUp(joystickJump))
        {
            isJumping = false;
        }

        if ((Input.GetKey(shoot) || Input.GetKey(joystickShoot)) && !isReloading && !shieldEquipped)
        {
            Fire();
            StartCoroutine(Reload());
        }

        if ((Input.GetKey(createShield) || Input.GetKey(joystickCreateShield)) && canEquipShield)
        {
            CreateShield();
            canEquipShield = false;
        }

        if (shieldEquipped)
        {
            ShieldDeployTime();
        }
        if (shieldReloading)
        {
            ShieldReloadTime();
        }
    }

    private void HandleInput()
    {
        currentHorizontalMovementState = HorizontalMovementStates.STATIC;

        if (Input.GetKey(left) || Input.GetAxis(horizontal) <= -.8)
        {
            currentHorizontalMovementState = HorizontalMovementStates.LEFT;
        }
        if(Input.GetKey(right) || Input.GetAxis(horizontal) >= .8)
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
            transform.rotation = Quaternion.identity;

            rb.velocity = new Vector2(-currentClass.MoveSpeed, rb.velocity.y);
        } else if(currentHorizontalMovementState == HorizontalMovementStates.RIGHT)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);

            rb.velocity = new Vector2(currentClass.MoveSpeed, rb.velocity.y);
        }
        else
        {
            Debug.LogError("Unrecognised state " + currentHorizontalMovementState);
        }
    }

    public void Jump()
    {
        rb.velocity = Vector2.up * currentClass.JumpHeight;
    }

    void Fire()
    {
        // The direction in which the player will shoot
        int direction;
        if (transform.rotation.y == 0)
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
        bullet.GetComponent<Rigidbody2D>().velocity += (bulletDirection);
    }

    void CreateShield()
    {
        int direction;
        if (transform.rotation.y == 0)
        {
            direction = -1;
        }
        else direction = 1;

        Vector3 offset = new Vector3(direction, 0, 0);

        shield = Instantiate(shieldPrefab, transform.position + offset, transform.rotation) as GameObject;
        shield.transform.parent = transform;
        shieldEquipped = true;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        reloadText.text = "Reloading...";
        yield return new WaitForSeconds(currentClass.ReloadTime);
        isReloading = false;
        reloadText.text = "";
    }

    IEnumerator DontIgnorePlatform(Collider2D currentIgnoredPlatform)
    {
        yield return new WaitForSeconds(.4f);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), currentIgnoredPlatform, false);
    }

    void ShieldDeployTime()
    {
        canEquipShield = false;
        shieldDeployTime -= currentClass.ShieldDeployTime * Time.deltaTime;
        powerBar.fillAmount -= currentClass.ShieldDeployTime * Time.deltaTime;

        if(shieldDeployTime <= 0)
        {
            Destroy(shield);
            shieldEquipped = false;
            shieldReloading = true;
            shieldDeployTime = currentClass.ShieldDeployTime;
        }
    }

    void ShieldReloadTime()
    {
        shieldReloadTime += Time.deltaTime;
        powerBar.fillAmount = shieldReloadTime / currentClass.ShieldReloadTime;

        if(shieldReloadTime >= currentClass.ShieldReloadTime)
        {
            shieldReloadTime = 0;
            shieldReloading = false;
            canEquipShield = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Healthpack(Clone)")
        {
            Destroy(collision.gameObject);
            health += currentClass.Health / 3;
            if (health >= currentClass.Health)
            {
                health = currentClass.Health;
            }
        }

        if (collision.gameObject.name == "Bullet(Clone)")
        {
            if (!shieldEquipped)
            {
                health -= collision.gameObject.GetComponent<Bullet>().damage;
            }
            else if ((collision.GetComponent<Rigidbody2D>().velocity.x > 0 && transform.eulerAngles.y == 0) || (collision.GetComponent<Rigidbody2D>().velocity.x < 0 && transform.eulerAngles.y == 180))
            {
                if(health != currentClass.Health)
                {
                    health += 60f;
                }
                return;
            } else
            {
                health -= collision.gameObject.GetComponent<Bullet>().damage;
            }
        }
    }
}