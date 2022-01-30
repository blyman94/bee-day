using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Field Declarations
    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Graphics")]
    [Space]
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Components")]
    [Space]
    [SerializeField] public Health health;
    [SerializeField] protected Mover mover;
    [SerializeField] protected PlayerAnimatorHelper animatorHelper;

    [Header("Colliders")]
    [Space]
    [SerializeField] protected BoxCollider2D mainCollider;

    [Header("Shooting")]
    [SerializeField] private Weapon[] weapons;
    [SerializeField] public GameObject crosshair;
    [SerializeField] private Transform shotOrigin;

    [Header("Audio")]
    [SerializeField] private AudioSource damageAudio;
    [SerializeField] private AudioClip damageClip;

    // control fields
    private Vector2 input;
    private Vector3 crosshairPos;
    private bool isGameRunning = true;

    // weapon fields
    [HideInInspector] public Weapon currentWeapon;
    private int currentWeaponNum = 0;

    // collection fields
    private readonly bool[] objectives = new bool[3];
    private int currentObjective = 0;
    private FlowerColor currentFlowerColor = FlowerColor.NONE;

    // melee fields
    private bool canAttack = true;
    #endregion

    #region Delegate Declarations
    public delegate void Win();
    public Win win;
    public delegate void Lose();
    public Lose lose;
    public delegate void NewObjective(FlowerColor flowerColor);
    public NewObjective newObjective;
    public delegate void ObjectiveComplete();
    public ObjectiveComplete objectiveComplete;
    public delegate void SwitchWeapon(int weaponNum);
    public SwitchWeapon switchWeapon;
    #endregion

    #region Start Up/Shutdown
    private void Awake()
    {
        InitializeAnimatorParams();
        InitializeHealthParams();
        InitializeMoverParams();
    }
    private void OnEnable()
    {
        health.empty += Die;
        animatorHelper.strike += StingerAttack;
    }
    private void Start()
    {
        currentWeapon = weapons[currentWeaponNum];
    }
    private void OnDisable()
    {
        health.empty -= Die;
        animatorHelper.strike -= StingerAttack;
    }
    #endregion

    #region Update Callbacks
    private void Update()
    {
        if (isGameRunning && !GameManager.isPaused)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            mover.SetDirectionalInput(input);
            crosshairPos = crosshair.transform.position;

            if (currentWeaponNum == 1)
            {
                // Automatic fire if its the BeeK-47
                if (Input.GetMouseButton(0))
                {
                    currentWeapon.FireProjectile();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentWeapon.FireProjectile();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (canAttack)
                {
                    animator.SetTrigger("attack");
                }
            }

            SwitchWeaponInput();

            FlipSprite();
            AimShooter();
            UpdateAnimatorParams();
        }
    }

    
    #endregion

    #region Aim Methods
    private void AimShooter()
    {
        if (weapons[currentWeaponNum].useShotRotation)
        {
            Vector3 shootDir = crosshairPos - shotOrigin.position;
            float rotation = (Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg) - 90.0f;
            if (rotation > 0)
            {
                rotation = -270;
            }
            else if (rotation > -90)
            {
                rotation = -90;
            }
            weapons[currentWeaponNum].shotRotation = rotation;
        }
    }
    private void FlipSprite()
    {
        float shotXPos = Mathf.Abs(shotOrigin.localPosition.x);
        if (crosshairPos.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
            shotOrigin.localPosition = new Vector3(-shotXPos, shotOrigin.localPosition.y, 0);
        }
        else
        {
            spriteRenderer.flipX = false;
            shotOrigin.localPosition = new Vector3(shotXPos, shotOrigin.localPosition.y, 0);
        }
    }
    #endregion

    #region Animator Controller Methods
    private void UpdateAnimatorParams()
    {
        animator.SetFloat("horizontalInput", input.x);
        animator.SetFloat("verticalInput", input.y);
    }
    #endregion

    #region Initialization Methods
    private void InitializeAnimatorParams()
    {
        animator.SetFloat("attackAnimSpeed", playerStats.attackAnimSpeed);
        animator.SetFloat("deathAnimSpeed", playerStats.deathAnimSpeed);
        animator.SetFloat("hitAnimSpeed", playerStats.hitAnimSpeed);
        animator.SetFloat("moveAnimSpeed", playerStats.moveAnimSpeed);
        animator.SetFloat("readyAnimSpeed", playerStats.readyAnimSpeed);
    }
    private void InitializeHealthParams()
    {
        health.spriteRenderer = spriteRenderer;
        health.maxHealth = playerStats.maxHealth;
        health.damageAudio = damageAudio;
        health.damageClip = damageClip;
    }
    private void InitializeMoverParams()
    {
        mover.bc2d = mainCollider;
        mover.groundLayer = playerStats.groundLayer;
        mover.useGravity = false;
        mover.moveSpeed = playerStats.moveSpeed;
        mover.accelAir = playerStats.accelAir;
    }
    #endregion

    #region Collection Methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Flower"))
        {
            Flower thisFlower = other.GetComponent<Flower>();
            if (currentFlowerColor == FlowerColor.NONE)
            {
                currentFlowerColor = thisFlower.flowerColor;
                newObjective?.Invoke(thisFlower.flowerColor);
                thisFlower.TurnOffGlow();
            }
            else
            {
                if(currentFlowerColor == thisFlower.flowerColor)
                {
                    objectives[currentObjective] = true;
                    currentObjective += 1;
                    thisFlower.TurnOffGlow();
                    currentFlowerColor = FlowerColor.NONE;
                    objectiveComplete?.Invoke();
                }
                return;
            }
        }
        if (other.transform.CompareTag("Start"))
        {
            if (objectives[0] && objectives[1] && objectives[2])
            {
                win?.Invoke();
                mover.SetDirectionalInput(Vector2.zero);
                isGameRunning = false;
            }
        }
    }
    #endregion

    #region Melee Methods
    public void StingerAttack()
    {
        Vector2 attackDir = (crosshairPos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(shotOrigin.position, attackDir, playerStats.attackRange, playerStats.canAttackLayer);
        Debug.DrawRay(shotOrigin.position, attackDir * playerStats.attackRange, Color.red,2.0f);
        if (hit)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>().AddPoison(playerStats.posionDuration);
            }
        }
        StartCoroutine(AttackCooldownRoutine());
    }
    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(playerStats.meleeCooldown);
        canAttack = true;
    }
    #endregion

    #region Behavior Methods
    private void SwitchWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeaponNum = 0;
            currentWeapon = weapons[currentWeaponNum];
            switchWeapon?.Invoke(currentWeaponNum);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeaponNum = 1;
            currentWeapon = weapons[currentWeaponNum];
            switchWeapon?.Invoke(currentWeaponNum);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeaponNum = 2;
            currentWeapon = weapons[currentWeaponNum];
            switchWeapon?.Invoke(currentWeaponNum);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentWeaponNum + 1 > weapons.Length - 1)
            {
                currentWeaponNum = 0;
            }
            else
            {
                currentWeaponNum += 1;
            }
            currentWeapon = weapons[currentWeaponNum];
            switchWeapon.Invoke(currentWeaponNum);
        }
    }
    public virtual void Die()
    {
        animator.SetTrigger("die");
        gameObject.tag = "Dead";
        if (mover != null)
        {
            mover.useGravity = true;
        }
        lose?.Invoke();
        isGameRunning = false;
    }
    #endregion
}
