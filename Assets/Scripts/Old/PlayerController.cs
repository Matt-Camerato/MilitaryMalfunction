using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController Instance;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    public float maxHealth;
    public float maxArmor;
    public float fireRate;
    public float regenRate;

    [Header("Player Info")]
    public float totalHealth;
    private float fireCooldown = 0;
    private float turnCooldown = 0;

    public bool isDead = false;

    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite;   
    [SerializeField] private Transform missileStart;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private ParticleSystem explosionPS;

    private void Awake() => Instance = this;

    private void Start()
    {
        totalHealth = maxHealth + maxArmor;
        UpdateHUD();
    }

    private void Update()
    {
        //check if paused
        if(HUDManager.Instance.isPaused) return;

        //check if dead
        if(isDead) return;

        //updates fire and turn cooldowns;
        UpdateCooldowns();
        UpdateHUD();

        //always make player move forward at given speed
        transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;

        //rotate player with a and d keys
        float lookRotation = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        if (turnCooldown <= 0) transform.Rotate(new Vector3(0, 0, -lookRotation));

        //fire missiles with the space button
        if (Input.GetKeyDown(KeyCode.Space) && fireCooldown == 0)
        {
            GameObject newMissile = Instantiate(missilePrefab, missileStart.position, transform.rotation);
            //newMissile.GetComponent<MissileController>().HUD = HUD.GetComponent<HUDController>();
            //newMissile.GetComponent<MissileController>().SFXSource = SFXSource;
            //SFXSource.GetComponent<SFXController>().Shoot();
            fireCooldown = fireRate;
            turnCooldown = 30;
        }
    }

    public void Damage(float damage)
    {
        totalHealth -= damage;

        //death check
        if (totalHealth <= 0)
        {
            HandleDeath();
            isDead = true;
        }
    }

    private void UpdateCooldowns()
    {
        //update firing cooldown if unable to fire
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        //update turn cooldown if unable to turn
        if (turnCooldown != 0)
        {
            turnCooldown -= 100 * Time.deltaTime;
            if (turnCooldown < 0)
            {
                turnCooldown = 0;
            }
        }
    }

    private void UpdateHUD()
    {
        //get normalized armor value
        float currentArmor = totalHealth - maxHealth;
        float armorNormalized = currentArmor / maxArmor;

        //get normalized health value
        float currentHealth = totalHealth - currentArmor;
        float healthNormalized = currentHealth / maxHealth;

        //get normalized reload value
        float reloadNormalized = fireCooldown / fireRate;

        //update HUD bars accordingly
        HUDManager.Instance.UpdateHUDBars(healthNormalized, armorNormalized, reloadNormalized);
    }

    private void HandleDeath()
    {
        playerSprite.enabled = false;
        explosionPS.Play();
    }
}
