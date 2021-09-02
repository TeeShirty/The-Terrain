using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaw : MonoBehaviour
{

    public Transform target;
    public float aggroDistance = 15f;
    public float moveSpeed = 17f;

    Rigidbody rigidbody;
    Animator anim;

    public Character character;
    Enemy enemy;

    public GameObject[] spawnPowerUps;
    public Transform spawnPoint;

    AudioSource explosionAudioSource;
    public AudioClip explosionSFX;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target && Vector3.Distance(target.transform.position, gameObject.transform.position) <= aggroDistance)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            rigidbody.MovePosition(target.position * Time.deltaTime * 2);
            //transform.LookAt(target.transform);
            anim.SetBool("isRunning", true);
            //Debug.Log("Running Maw");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            Boom();
            Debug.Log("Big Boom");
            StartCoroutine(Wait(1));
            StartCoroutine(PowerUpSpawner(1f));
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Boom();
            StartCoroutine(Wait(0.5f));
        }
    }

    void Boom()
    {
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particleSystems)
        {
            particle.Play();
            Debug.Log("Played particles");
        }
        explosionAudioSource = gameObject.AddComponent<AudioSource>();
        explosionAudioSource.clip = explosionSFX;
        explosionAudioSource.loop = true;
        explosionAudioSource.Play();
    }

    IEnumerator Wait(float time)
    {
        character.healthbar.OnTakeDamage(10);
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public IEnumerator PowerUpSpawner(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(spawnPowerUps[Random.Range(0, spawnPowerUps.Length)], spawnPoint.transform.position, spawnPoint.transform.rotation);
    }
}
