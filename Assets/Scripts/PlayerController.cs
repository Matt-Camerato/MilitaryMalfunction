using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController Instance;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    public float maxHealth;
    public float maxArmor;
    public float fireRate;
    public float regenRate;

    [Header("Player Info")]
    private float currentHealth;
    private float currentArmor;
    private float fireCooldown = 0;
    private float turnCooldown = 0;

    public bool isDead = false;
    private bool exploded = false;

    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite;   
    [SerializeField] private Transform missileStart;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private ParticleSystem explosionPS;
    [SerializeField] private GameObject deathScreenPanel;

    private void Awake() => Instance = this;

    private void Start()
    {
        currentArmor = maxArmor;
        currentHealth = maxHealth;
        UpdateHUD();
    }

    private void Update()
    {
        //check if intro sequence is over
        if(!IntroManager.Instance.doneIntro) return;

        //check if dead
        if(isDead) return;
        
        //check if exploded
        if(exploded) 
        {
            if(explosionPS.particleCount == 0 )
            {
                deathScreenPanel.SetActive(true);
                isDead = true;
            }
            return;
        }

        //check if paused
        if(HUDManager.Instance.isPaused) return;

        //updates fire and turn cooldowns;
        UpdateCooldowns();
        UpdateHUD();

        //always make player move forward at given speed
        transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;

        //rotate player with a and d keys
        float lookRotation = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        if (turnCooldown <= 0) transform.Rotate(new Vector3(0, 0, -lookRotation));

        //regenerate player health
        if(currentHealth < maxHealth) currentHealth += Time.deltaTime * regenRate;

        //fire missiles with the space button
        if (Input.GetKeyDown(KeyCode.Space) && fireCooldown <= 0)
        {
            GameObject newMissile = Instantiate(missilePrefab, missileStart.position, transform.rotation);
            fireCooldown = fireRate;
            turnCooldown = 0.3f;
            AudioSystem.Instance?.FireSFX(); //play missile firing SFX
        }
    }

    public void Damage(float damage)
    {
        //deal damage to armor, then health
        float healthDamage = currentArmor - damage;
        if(healthDamage >= 0)
        {
            //if only armor is damaged
            currentArmor = healthDamage;
            return;
        }
        else
        {
            //if health is damaged through armor
            currentArmor = 0;
            currentHealth += healthDamage;
        }

        //death check
        if (currentHealth <= 0)
        {
            HandleDeath();
            exploded = true;
        }
    }

    private void UpdateCooldowns()
    {
        //update firing cooldown if unable to fire
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        //update turn cooldown if unable to turn
        if (turnCooldown > 0) turnCooldown -= Time.deltaTime;
    }

    private void UpdateHUD()
    {
        //get normalized armor, health, and reload values
        float armor = currentArmor / maxArmor;
        float health = currentHealth / maxHealth;
        float reload = fireCooldown / fireRate;

        //update HUD bars accordingly
        HUDManager.Instance.UpdateHUDBars(health, armor, reload);
    }

    private void HandleDeath()
    {
        //make player's tank blow up and stop tank noise
        playerSprite.enabled = false;
        explosionPS.Play();
        AudioSystem.Instance?.ToggleTankNoise();
    }
}
