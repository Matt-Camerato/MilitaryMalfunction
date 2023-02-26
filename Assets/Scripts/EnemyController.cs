using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using TMPro;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Type Number")]
    public float enemyNum;

    [Header("Enemy Stats")]
    public float lookSensitivity;
    public float moveSpeed;
    public float shotRange;
    public float fireCooldownAmount;
    public float maxHealth;

    [Header("Player")]
    public GameObject player;

    [Header("Missile")]
    public GameObject missile;

    [Header("HUD Controller")]
    public HUDController HUD;

    [Header("For 4 Barriers")]
    public GameObject Ground;
    private BoxCollider2D northBarrier;
    private BoxCollider2D eastBarrier;
    private BoxCollider2D southBarrier;
    private BoxCollider2D westBarrier;

    private ParticleSystem explosionPS;
    private GameObject enemyHUD;

    [Header("SFX")]
    public AudioSource SFXSource;

    private float playerDistance;
    private float fireCooldown = 0;
    private float turnCooldown = 0;
    private float health;
    private float deathChoice = -1;
    private bool isDead = false;

    private void Start()
    {
        health = maxHealth;
        explosionPS = transform.GetChild(1).GetComponent<ParticleSystem>();
        enemyHUD = transform.GetChild(2).gameObject;

        northBarrier = Ground.transform.GetChild(0).GetComponent<BoxCollider2D>();
        eastBarrier = Ground.transform.GetChild(1).GetComponent<BoxCollider2D>();
        southBarrier = Ground.transform.GetChild(2).GetComponent<BoxCollider2D>();
        westBarrier = Ground.transform.GetChild(3).GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), northBarrier);
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), eastBarrier);
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), southBarrier);
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), westBarrier);
    }

    void Update()
    {
        if (player != null)
        {
            //check death explosion
            if (isDead == false)
            {
                //check if game is paused
                if (!HUD.settingsOn)
                {
                    //check if enemy is in map
                    if (Mathf.Abs(transform.position.x) < 36 && Mathf.Abs(transform.position.y) < 37)
                    {
                        //rotates enemy to look at player
                        //uses slerp to make turn time slower
                        Quaternion lookAt = Quaternion.LookRotation(transform.position - player.transform.position, Vector3.forward);
                        lookAt.x = 0;
                        lookAt.y = 0;
                        if (turnCooldown == 0)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, lookSensitivity * Time.deltaTime);
                            //use this if you want healthbar to stay at bottom of tank and not rotate
                            //transform.GetChild(2).position = transform.position - new Vector3(0, 1.2f, 0);
                            //transform.GetChild(2).rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.z));
                        }

                        //if tank is close enough to player, stop and shoot, else move towards player
                        if (playerDistance <= shotRange)
                        {
                            //stop and shoot if player is in line of sight
                            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * shotRange, shotRange, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default"));
                            if (hit.collider != null && hit.transform.gameObject.tag == "Player")
                            {
                                Debug.DrawRay(transform.position, transform.up * shotRange, Color.green);
                                if (fireCooldown == 0)
                                {
                                    GameObject newMissile = Instantiate(missile, transform.GetChild(0).position, transform.rotation);
                                    newMissile.GetComponent<MissileController>().target = hit.collider.gameObject;
                                    newMissile.GetComponent<MissileController>().HUD = HUD;
                                    newMissile.GetComponent<MissileController>().SFXSource = SFXSource;
                                    SFXSource.GetComponent<SFXController>().Shoot();
                                    fireCooldown = fireCooldownAmount * 100;
                                    turnCooldown = 30;
                                }
                            }
                            else
                            {
                                //else move towards player and around wall obstacle
                                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                            }
                        }
                        else
                        {
                            //else move towards player
                            transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                        }
                    }
                    else if(transform.position.y > 36)
                    {
                        //if player spawned at north spawn, move south into map
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;

                    }
                    else if(transform.position.x > 36)
                    {
                        //if player spawned at east spawn, move west into map
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                        transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                    }
                    else if (transform.position.y < -36)
                    {
                        //if player spawned at south spawn, move north into map
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                    }
                    else if (transform.position.x < -36)
                    {
                        //if player spawned at west spawn, move east into map
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                        transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                    }
                }

                if (explosionPS.particleCount > 0)
                {
                    isDead = true;
                }
            }
            else
            {
                //when enemy dies, disable sprite renderer, collider, and HUD.
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                enemyHUD.GetComponent<Canvas>().enabled = false;
                if (explosionPS.particleCount == 0)
                {
                    deleteEnemy();
                }
            }

            //updates fire and turn cooldowns;
            updateCooldowns();

            //updates distance from player
            playerDistance = Vector2.Distance(transform.position, player.transform.position);
        }
        else
        {
            if(deathChoice == -1)
            {
                deathChoice = Random.Range(0, 3);
            }
            else if(deathChoice == 0)
            {
                //This death choice makes the enemy tank forever move towards the player's death location
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
            }
            else if (deathChoice == 1)
            {
                //This death choice makes the enemy spin on the z-axis forever
                transform.Rotate(new Vector3(0, 0, 1));
            }
            else
            {
                //this death choice makes the enemy do donuts forever
                transform.Rotate(new Vector3(0, 0, 1));
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
            }
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        enemyHUD.GetComponent<Canvas>().enabled = true;

        if (health <= 0)
        {
            //add enemytype num to list of killed enemies
            WaveController.killedEnemies.Add((int)enemyNum);

            //checks if killing this enemy will cause end of round
            if(WaveController.enemiesRemaining - 1 == 0)
            {
                //if so, reset wave cooldown, and then update HUD display info
                WaveController.waveCooldown = 1500;
                HUD.transform.GetChild(3).GetComponent<TMP_Text>().text = "Next Wave Will Begin In " + (WaveController.waveCooldown / 100).ToString();
            }
            //then lower enemies remaining
            WaveController.enemiesRemaining--;

            explosionPS.Play();
        }

        //update healthBar
        enemyHUD.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = health.Remap(0, maxHealth, 0, 1);
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
    public void deleteEnemy()
    {
        Destroy(gameObject);
    }
}
