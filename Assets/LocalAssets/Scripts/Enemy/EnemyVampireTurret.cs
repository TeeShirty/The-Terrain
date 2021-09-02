using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVampireTurret : MonoBehaviour
{

    public Transform target;
    Rigidbody rigidbody;
    Animator anim;
    public bool targetLocked;
    private bool shotReady;

    public Transform projectileSpawnPoint;
    public Rigidbody projectilePrefab;

    public float projectileForce;
    public float aggroDistance;

    public float fireTimer = 3f;
    private float _timeSinceLastFire = 0.0f;

    private Vector3 playerTarget;

    public GameObject[] spawnPowerUps;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        //Garbage data
        if (projectileForce <= 0)
        {
            projectileForce = 7.0f;
        }

        if (aggroDistance <= 0)
        {
            aggroDistance = 10f;
        }

        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        shotReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (targetLocked)
        //{
            if (shotReady)
            {
                if (Vector3.Distance(target.transform.position, transform.position) <= aggroDistance)
                {
                    //Debug.Log(target.transform.position);
                    transform.LookAt(target);
                    Shoot();
                    

                    //Rigidbody temp = Instantiate(projectilePrefab, projectileSpawnPoint.position,
                    //   Quaternion.identity);

                    //// Shoot projectile
                    //temp.AddForce(projectileSpawnPoint.forward * projectileForce, ForceMode.Impulse);

                    //// Destroy projectile after 2.0 seconds
                    //Destroy(temp.gameObject, 2.0f);


                    //Debug.Log("Player in range");
                    ////Vector3 myPos = new Vector3(projectileSpawnPoint.position.x, projectileSpawnPoint.position.y);
                    //Debug.Log("Got myPos");
                    ////StartCoroutine(TurretShooting(1.0f));
                    //Debug.Log("Instantiated");
                    ////Vector3 direction = myPos - (Vector3)target.transform.position;
                    //Debug.Log("Locked on location");
                    ////projectile.GetComponent<Rigidbody>().AddRelativeForce(new Vector3 (0, 4, 0));
                    //Debug.Log("Shot towards enemy");
                }
                else
                {
                    anim.SetBool("targetInRange", false);
                }
            }
        //}
    }

    void Shoot()
    {
        Rigidbody temp = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        anim.SetBool("targetInRange", true);
        temp.AddForce(projectileSpawnPoint.forward * projectileForce, ForceMode.Impulse);
        shotReady = false;
        StartCoroutine(fireRate());
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            StartCoroutine(Wait(1));
            StartCoroutine(PowerUpSpawner(1f));
        }
    }

        IEnumerator fireRate()
    {
        yield return new WaitForSeconds(fireTimer);
        shotReady = true;
        //Shoot projectile
        //temp.AddForce(projectileSpawnPoint.forward * projectileForce, ForceMode.Impulse);
    }

    public IEnumerator PowerUpSpawner(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(spawnPowerUps[Random.Range(0, spawnPowerUps.Length)], spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
