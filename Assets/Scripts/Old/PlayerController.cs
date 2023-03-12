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
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform missileStart;
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

        /*
        //check death explosion
        if (isDead == false)
        {
            //check if game is paused
            if (!HUD.GetComponent<HUDController>().settingsOn)
            {
                //updates fire and turn cooldowns;
                updateCooldowns();

                //always make player move forward at given speed
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;

                //rotate player with a and d keys
                float lookRotation = Input.GetAxis("Horizontal") * lookSensitivity * Time.deltaTime;
                if (turnCooldown == 0)
                {
                    transform.Rotate(new Vector3(0, 0, -lookRotation));
                    transform.GetChild(0).Rotate(new Vector3(0, 0, lookRotation));
                }

                if (Input.GetKeyDown(KeyCode.Space) && fireCooldown == 0)
                {
                    GameObject newMissile = Instantiate(missile, transform.GetChild(1).position, transform.rotation);
                    newMissile.GetComponent<MissileController>().HUD = HUD.GetComponent<HUDController>();
                    newMissile.GetComponent<MissileController>().SFXSource = SFXSource;
                    SFXSource.GetComponent<SFXController>().Shoot();
                    fireCooldown = fireCooldownAmount * 100;
                    turnCooldown = 30;
                }
            }

            
            if (Input.GetKeyDown(KeyCode.Escape) && !HUD.GetComponent<HUDController>().fadeOut)
            {
                HUD.GetComponent<HUDController>().Settings();
                SFXSource.GetComponent<SFXController>().ButtonPress();
            }

            if (explosionPS.particleCount > 0)
            {
                isDead = true;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            HUD.transform.GetChild(1).GetComponent<Button>().interactable = false;
            if(explosionPS.particleCount == 0)
            {
                deletePlayer();
            }
        }
        */
    }

    public void Damage(float damage)
    {
        totalHealth -= damage;

        //death check
        if (totalHealth <= 0)
        {
            isDead = true;
            HandleDeath();
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
        explosionPS.Play();
    }

    /*
    public void updateHUDBars()
    {
        //armor
        float currentArmor = totalHealth - maxHealth;
        if (currentArmor > 0)
        {
            HUD.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = currentArmor.Remap(0, maxArmor, 0, 1);
        }
        else
        {
            currentArmor = 0;
            HUD.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = currentArmor;
        }

        //health
        float currentHealth = totalHealth - currentArmor;
        HUD.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = currentHealth.Remap(0, maxHealth, 0, 1);
    }
    */

    //call this function at end of death particle system
    public void deletePlayer()
    {
        //deletes all children but camera... 
        for (int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //...then detaches camera...
        transform.DetachChildren();
        //...and lastly deletes player
        Destroy(gameObject);
    }
}
