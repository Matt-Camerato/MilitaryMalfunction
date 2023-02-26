using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    [Header("Missile Settings")]
    public float moveSpeed;
    public float damage;
    public bool heatSeeking;

    [Header("Player")]
    public GameObject target;

    [Header("HUD Controller")]
    public HUDController HUD;

    [Header("SFX")]
    public AudioSource SFXSource;

    private ParticleSystem explosionPS;

    private float flyTime = 200;
    private bool exploded = false;

    void Start()
    {
        explosionPS = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!exploded)
        {
            //check if game is paused
            if (!HUD.settingsOn)
            {
                //updates fly time and destroys missile if flying for too long
                flyTime -= 100 * Time.deltaTime;
                if (flyTime <= 0)
                {
                    explosionPS.Play();
                }

                //tracks target if heat seeking missile
                if (heatSeeking && target != null)
                {
                    Quaternion lookAt = Quaternion.LookRotation(transform.position - target.transform.position, Vector3.forward);
                    lookAt.x = 0;
                    lookAt.y = 0;
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, 2 * Time.deltaTime);
                }

                //moves missile forward
                transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;

                //delete missile if not in map
                if (Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50)
                {
                    explosionPS.Play();
                }
            }

            if (explosionPS.particleCount > 0)
            {
                exploded = true;
                SFXSource.GetComponent<SFXController>().Damage();
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            if (explosionPS.particleCount == 0)
            {
                deleteMissile();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //damage player and destroy missile
            collision.GetComponent<PlayerController>().Damage(damage);
            explosionPS.Play();
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            //damage enemy and destroy missile
            collision.GetComponent<EnemyController>().Damage(damage);
            explosionPS.Play();
        }
        else if (collision.gameObject.tag == "Wall")
        {
            //destroy missile
            explosionPS.Play();
        }
    }

    //destroys missile at end of explosion
    public void deleteMissile()
    {
        Destroy(gameObject);
    }
}
