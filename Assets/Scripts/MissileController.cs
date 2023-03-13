using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class MissileController : MonoBehaviour
{
    public GameObject target = null;

    [Header("Missile Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;
    [SerializeField] private bool heatSeeking;

    [Header("References")]
    [SerializeField] private ParticleSystem explosionPS;

    private float flyTime = 200;
    private bool exploded = false;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //check if exploded
        if(!exploded && explosionPS.particleCount > 0)
        {
            sr.enabled = false;
            bc.enabled = false;
            AudioSystem.Instance.DamageSFX();
            exploded = true;
        }

        //get rid of missile after explosion
        if(exploded)
        {
            if (explosionPS.particleCount == 0) Destroy(gameObject);
            return;
        }

        //check if paused
        if(HUDManager.Instance.isPaused) return;

        //updates fly time and destroys missile if flying for too long
        flyTime -= 100 * Time.deltaTime;
        if (flyTime <= 0) explosionPS.Play();

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
        if (Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50) explosionPS.Play();
    }

    //handles missile collision
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
        else if (collision.gameObject.tag == "Wall") explosionPS.Play(); //just destroy missile
    }

    //destroys missile at end of explosion
    private void DeleteMissile() => Destroy(gameObject);
}
