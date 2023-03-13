using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using TMPro;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float range;
    [SerializeField] private float fireRate;
    [SerializeField] private float maxHealth;
    
    [Header("References")]
    [SerializeField] private Transform missileStart;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private ParticleSystem explosionPS;
    [SerializeField] private Canvas enemyCanvas;
    [SerializeField] private Image enemyHealthBar;

    private Transform playerTransform;
    private SpriteRenderer sr;
    private CircleCollider2D cc;

    private float health;
    private float playerDistance;
    private float fireCooldown = 0;
    private float turnCooldown = 0;
    private int celebrationType = -1;
    private bool exploded = false;

    private void Start()
    {
        health = maxHealth;

        //initialize references
        playerTransform = PlayerController.Instance.transform;
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        //do celebration if player is dead
        if(PlayerController.Instance.isDead)
        {
            if(celebrationType == -1) celebrationType = Random.Range(0, 3);
            DoCelebration();
            return;
        }

        //check if exploded
        if(!exploded && explosionPS.particleCount > 0)
        {
            //when enemy dies, disable sprite renderer, collider, and HUD canvas
            sr.enabled = false;
            cc.enabled = false;
            enemyCanvas.enabled = false;
            exploded = true;
            return;
        }

        //get rid of enemy after explosion
        if(exploded)
        {
            if (explosionPS.particleCount == 0) Destroy(gameObject);
            return;
        }

        //check if paused
        if(HUDManager.Instance.isPaused) return;

        //updates fire and turn cooldowns
        UpdateCooldowns();

        //updates distance from player
        playerDistance = Vector2.Distance(transform.position, playerTransform.position);

        //rotates enemy to look at player (uses slerp to make turn time slower)
        Quaternion lookAt = Quaternion.LookRotation(transform.position - playerTransform.position, Vector3.forward);
        lookAt.x = 0;
        lookAt.y = 0;
        if (turnCooldown <= 0) transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, turnSpeed * Time.deltaTime);

        //if tank is close enough to player, stop moving and shoot
        if (playerDistance <= range)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * range, range, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default"));
            if (hit.collider != null && hit.transform.gameObject.tag == "Player")
            {
                Debug.DrawRay(transform.position, transform.up * range, Color.green);
                if (fireCooldown <= 0)
                {
                    GameObject missileObj = Instantiate(missilePrefab, transform.GetChild(0).position, transform.rotation);
                    missileObj.GetComponent<MissileController>().target = hit.collider.gameObject;
                    fireCooldown = fireRate;
                    turnCooldown = 0.3f;
                    AudioSystem.Instance.FireSFX(); //play missile firing SFX
                }
            }
            else
            {
                //else move towards player and around wall obstacle
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
            }
        }
        else transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime; //otherwise move towards player until within firing range
    }

    public void Damage(float damage)
    {
        enemyCanvas.enabled = true;
        health -= damage;

        //update health bar
        enemyHealthBar.fillAmount = health / maxHealth;

        //death check
        if (health <= 0)
        {
            //increment enemies killed and decrement enemies remaining
            EnemyManager.Instance.EnemiesKilled++;
            EnemyManager.Instance.EnemiesRemaining--;

            //explode enemy
            explosionPS.Play();         
        }
    }

    private void UpdateCooldowns()
    {
        //update firing cooldown if not able to fire
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        //update turn cooldown if unable to turn
        if (turnCooldown > 0) turnCooldown -= Time.deltaTime;
    }

    private void DoCelebration()
    {
        switch(celebrationType)
        {
            case 0:
                //this celebration makes the enemy tank forever move towards the player's death location
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                break;
            case 1:
                //this celebration makes the enemy spin on the z-axis forever
                transform.Rotate(new Vector3(0, 0, 1));
                break;
            case 2:
                //this celebration makes the enemy do donuts forever
                transform.Rotate(new Vector3(0, 0, 1));
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
                break;
        }
    }
}
