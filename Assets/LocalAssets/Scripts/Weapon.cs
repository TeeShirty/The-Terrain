using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
    AudioSource gunshotAudioSource;
    public AudioClip gunshotSFX;

    public Rigidbody projectile;            // Used to Create projectile
    //public int ammo;                      // Used to keep track of how much ammo there is
    public Transform projectileSpawnPoint;  // Used to position the bullet once spawned
    public float projectileForce;

    public ParticleSystem muzzleFlash;
        
    // Used to apply force to the bullet being fired

    // Use this for initialization
    void Start () 
    {
        //if (ammo <= 0)
        //{
            // Set the ammo count to 20
            //ammo = 20;
        //}

        if (projectileForce <= 0)
        {
            // Set the bullet force
            projectileForce = 20.0f;
        }
	}

    private void Update()
    {
        //swap between weapons
    }


    public int Shoot()
    {
        // Check if there is enough ammo
        if (projectile && GameManager.Instance.ammo > 0)
        {
            muzzleFlash.Play();

            // Create the bullet if there is enough ammo
            Rigidbody temp = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation) as Rigidbody;
            
            // Add the force to fire the bullet
            temp.AddForce(transform.forward * projectileForce, ForceMode.Impulse);

            gunshotAudioSource = gameObject.AddComponent<AudioSource>();
            gunshotAudioSource.clip = gunshotSFX;
            gunshotAudioSource.loop = false;
            gunshotAudioSource.Play();

            // Remove one ammo count
            //ammo--;

            GameManager.Instance.ammo--;

            if(GameManager.Instance.ammo <= 0)
            {
                GameManager.Instance.ammo = 0;
                temp = null;
            }

            Destroy(temp.gameObject, 2.0f);
        }
        // Do something if there isnt enough ammo
        else
        {
            // Play audio for reload

            // Print message
            Debug.Log("Reload");
        }

        //return ammo;
        return GameManager.Instance.ammo;
    }

    
}
