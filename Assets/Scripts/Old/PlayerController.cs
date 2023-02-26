using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    public float lookSensitivity;
    public float moveSpeed;
    public float fireCooldownAmount;
    public float maxHealth;
    public float maxArmor;
    public float regenRate;

    [Header("Player Info")]
    public float totalHealth;
    public float fireCooldown = 0;
    public bool canMove = true;

    [Header("Missile")]
    public GameObject missile;

    [Header("Explosion")]
    public ParticleSystem explosionPS;

    [Header("HUD")]
    public GameObject HUD;

    [Header("SFX")]
    public AudioSource SFXSource;

    private float turnCooldown = 0;
    private bool isDead = false;

    void Start()
    {
        totalHealth = maxHealth + maxArmor;
    }

    void Update()
    {
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
    }

    public void Damage(float damage)
    {
        totalHealth -= damage;

        //Death check
        if (totalHealth <= 0)
        {
            explosionPS.Play();
        }

        //update health + armor HUD bars
        updateHUDBars();
    }

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

    private void updateCooldowns()
    {
        //update firing cooldown if not able to fire
        if (fireCooldown != 0)
        {
            fireCooldown -= 100 * Time.deltaTime;
            if (fireCooldown < 0)
            {
                fireCooldown = 0;
            }
        }

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
