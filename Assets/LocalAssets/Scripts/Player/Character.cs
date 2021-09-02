using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Character : MonoBehaviour
{
    CharacterController controller;
    public HealthBar healthbar;
    MouseLook mouselook;

    // Attributes will help organize the Inspector
    [Header("Player Settings")]
    [Space(2)]
    [Tooltip("Speed value between 1 and 6.")]
    [Range(1.0f, 6.0f)]
    public float speed;
    public float jumpSpeed;
    public float rotationSpeed;
    public float gravity;
    float doubleSpeed;

    public int ammo; // Used to keep track of how much ammo there is
    public bool collectWeapon;


    bool isAlive = true;
    public bool isPaused = false;
    bool isRunning = false;

    Vector3 moveDirection;
    private Vector3 verticalSpeed;

    enum ControllerType { SimpleMove, Move };
    [SerializeField] ControllerType type;

    public float health;
    public GameObject player;

    [Header("Weapon Settings")]
    // Handle weapon shooting
    public float projectileForce;
    public Rigidbody projectilePrefab;
    public Transform projectileSpawnPoint;

   
    [Header("Raycast Settings")]
    public Transform thingToLookFrom;
    public float lookDistance;

    GameManager gameManager;
    CanvasManager canvasManager;

    Animator animator;

    AudioSource runningAudioSource;
    public AudioClip runningSFX;

    // Start is called before the first frame update
    void Start()
    {
        //Garbage values
        try
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            canvasManager = FindObjectOfType<CanvasManager>();

            //checkpoint respawn position set
            transform.position = gameManager.lastCheckpointPosition; 

            health = healthbar.health;

            controller.minMoveDistance = 0.0f;
            animator.applyRootMotion = false;

            animator.SetBool("IsAlive", true);
            isAlive = true;

            //This will name our objects in the scene
            name = "Player";

            if (speed <= 0)
            {
                speed = 4f;

                Debug.Log("Speed not set on " + name + " defaulting to " + speed);
            }

            doubleSpeed = speed * 2;

            if (jumpSpeed <= 0)
            {
                jumpSpeed = 6.0f;

                Debug.Log("JumpSpeed not set on " + name + " defaulting to " + jumpSpeed);
            }

            if (rotationSpeed <= 0)
            {
                rotationSpeed = 10.0f;

                Debug.Log("RotationSpeed not set on " + name + " defaulting to " + rotationSpeed);
            }

            if (gravity <= 0)
            {
                gravity = 9.81f;

                Debug.Log("Gravity not set on " + name + " defaulting to " + gravity);
            }

            moveDirection = Vector3.zero;

            if (projectileForce <= 0)
            {
                projectileForce = 50.0f;

                Debug.Log("ProjectileForce not set on " + name + " defaulting to " + projectileForce);
            }

            if (!projectilePrefab)
                Debug.LogWarning("Missing projectilePrefab on " + name);

            if (!projectileSpawnPoint)
                Debug.LogWarning("Missing projectileSpawnPoint on " + name);
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        catch (UnassignedReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            Debug.LogWarning("Always get called");
        }

        runningAudioSource = gameObject.AddComponent<AudioSource>();
        runningAudioSource.clip = runningSFX;
    }

    // Update is called once per frame
    void Update()
    {
        //if (isAlive)
        //{
        //    controller.enabled = true;
            
        //    GetComponent<MouseLook>().enabled = true;
            
        //}
        //else if (!isAlive)
        //{
        //    speed = 0.0f;
            
        //    GetComponent<MouseLook>().enabled = false;
        //}

        switch (type)
        {
            case ControllerType.SimpleMove:
                controller.SimpleMove(transform.forward * Input.GetAxis("Vertical") * speed);
                break;

            case ControllerType.Move:

                if (controller.isGrounded)
                {
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    moveDirection *= speed;
                    moveDirection = transform.TransformDirection(moveDirection);

                    if (Input.GetButtonDown("Jump"))
                    {
                        Debug.Log("Space Pressed");
                        moveDirection.y = controller.stepOffset / Time.deltaTime;
                        animator.Play("Jumping");
                        moveDirection.y = jumpSpeed;
                    }
                    //else
                        //moveDirection.y = -controller.stepOffset / Time.deltaTime;

                    if(Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        
                        isRunning = true;
                        speed = doubleSpeed;
                        animator.SetBool("IsRunning", true);
                        //set up run audio
                        if (!runningAudioSource.isPlaying)
                        {
                            runningAudioSource.loop = true;
                            runningAudioSource.Play();
                            Debug.Log("run sound played");
                        }
                    }
                    else if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        isRunning = false;
                        animator.SetBool("IsRunning", false);
                        speed = speed/2;
                        if (runningAudioSource.isPlaying)
                        {
                            runningAudioSource.loop = false;
                            runningAudioSource.Stop();
                        }
                    }
                }
                moveDirection.y -= gravity * Time.deltaTime;

                controller.Move(moveDirection * Time.deltaTime);
                break;
        }


        //RAYCAST GOES HERE
        RaycastHit hit;

        if (thingToLookFrom)
        {
            Debug.DrawRay(thingToLookFrom.transform.position, thingToLookFrom.transform.forward * lookDistance, Color.red);

            if (Physics.Raycast(thingToLookFrom.transform.position, thingToLookFrom.transform.forward, out hit, lookDistance))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * lookDistance, Color.blue);

            if (Physics.Raycast(transform.position, transform.forward, out hit, lookDistance))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);
            }
        }


        animator.SetBool("IsGrounded", controller.isGrounded);
        //will follow the character rotation
        animator.SetFloat("Speed", transform.InverseTransformDirection(controller.velocity).z);

        //Punch Mechanic
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1");
            Punch();
        }

        //Kick Mechanic
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E Pressed");
            Kick();
        }

        //Projectile Throw Mechanic
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2");
            animator.SetTrigger("ThrowProjectile");
        }

        //Death Mechanic - on Key Press
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q Pressed");
            animator.SetBool("IsAlive", false);
            isAlive = false;
            Death();
            StartCoroutine(Wait(3));
            
        }

        if (health == 0)
        {
            Debug.Log("Player health is 0");
            Death();
            StartCoroutine(Wait(3));
        }

        //Save on key press
        if(Input.GetKeyDown(KeyCode.O))
        {
            canvasManager.SaveGame();
        }

        //Load on key press
        if (Input.GetKeyDown(KeyCode.I))
        {
            canvasManager.LoadGame();
        }

    }

    //Functions

    public void SaveGamePrepare()
    {
        //get player data object
        LoadSaveManager.GameStateData.DataPlayer data = GameManager.StateManager.gameState.player;

        data.collectedAmmo = ammo;
        data.collectedWeapon = collectWeapon;
        data.health = (int)health;

        data.positionRotationScale.positionX = transform.position.x;
        data.positionRotationScale.positionY = transform.position.y;
        data.positionRotationScale.positionZ = transform.position.z;

        data.positionRotationScale.rotationX = transform.localEulerAngles.x;
        data.positionRotationScale.rotationY = transform.localEulerAngles.y;
        data.positionRotationScale.rotationZ = transform.localEulerAngles.z;

        data.positionRotationScale.scaleX = transform.localScale.x;
        data.positionRotationScale.scaleY = transform.localScale.y;
        data.positionRotationScale.scaleZ = transform.localScale.z;
    }

    public void LoadGameComplete()
    {
        LoadSaveManager.GameStateData.DataPlayer data = GameManager.StateManager.gameState.player;

        health = data.health;
        ammo = (int)data.collectedAmmo;

        if(data.collectedWeapon)
        {
            //if a specific weapon, needs more info
            GameObject weaponPowerUp = GameObject.Find("Crossfire");

            if(weaponPowerUp)
            {
                weaponPowerUp.SendMessage("OnTriggerEnter2D", GetComponent<Collider2D>(), SendMessageOptions.DontRequireReceiver);
            }
        }

        transform.position = new Vector3(data.positionRotationScale.positionX, data.positionRotationScale.positionY, data.positionRotationScale.positionZ);

        transform.localRotation = Quaternion.EulerAngles(data.positionRotationScale.rotationX, data.positionRotationScale.rotationY, data.positionRotationScale.rotationZ);

        transform.localScale = new Vector3(data.positionRotationScale.scaleX, data.positionRotationScale.scaleY, data.positionRotationScale.scaleZ);
    }
    public void Punch()
    {
        
        Debug.Log("Punch");
        animator.SetTrigger("Punch");
        StartCoroutine(ActionWhileMoving(0.5f));
        
    }

    public void Kick()
    {
        Debug.Log("Kick");
        animator.SetTrigger("Kick");
        StartCoroutine(ActionWhileMoving(1f));
    }

    public void Death()
    {
        //animator.Play("Death", 0, 0);
        animator.SetBool("IsAlive", false);
        isAlive = false;
    }

    public void fireProjectile()
    {
        Debug.Log("Pew Pew");
        if (projectileSpawnPoint && projectilePrefab)
        {
            // Make projectile
            Rigidbody temp = Instantiate(projectilePrefab, projectileSpawnPoint.position,
                projectileSpawnPoint.rotation);

            // Shoot projectile
            temp.AddForce(projectileSpawnPoint.forward * projectileForce, ForceMode.Impulse);

            // Destroy projectile after 2.0 seconds
            Destroy(temp.gameObject, 2.0f);
            StartCoroutine(ActionWhileMoving(0.5f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            healthbar.OnTakeDamage(5);
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        //Game ends when player enters portal
        if (collision.gameObject.tag == "EndGame")
        {
            Debug.Log("Collided with Portal");
            if (SceneManager.GetActiveScene().name == "LevelComplete")
            {
                SceneManager.LoadScene("levelComplete");
                Time.timeScale = 0f;
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Reduce player speed when in vines
        if (other.gameObject.tag == "Vines")
        {
            if (speed == speed)
            {
                speed = 1f;
                //Debug.Log(speed);
            }
        }

        //Quit game when player enters portal
        if (other.gameObject.tag == "EndGame")
        {
            SceneManager.LoadScene("LevelComplete");
        }

        //Player to take damage when landing in lava
        if (other.gameObject.tag == "PoisonPlant")
        {
            healthbar.OnTakeDamage(50);
            HitAnimation();
        }

        //Player gains health on powerup
        if (other.gameObject.tag == "Health")
        {
            Debug.Log("Collided with powerup");
            healthbar.OnTakeDamage(-10);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Ammo")
        {
            Debug.Log("Collided with powerup");
            GameManager.Instance.ammo += 5;
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Score")
        {
            Debug.Log("Collided with powerup");
            GameManager.Instance.score += 1;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Lava")
        {
            healthbar.OnTakeDamage(1);
            HitAnimation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vines")
        {
            if (speed == speed)
            {
                speed = 4f;
                Debug.Log(speed);
            }
        }
    }

    public void HitAnimation()
    {
        if (healthbar)
        {
            Debug.Log("Health lost");
            animator.SetTrigger("OnHit");
            Debug.Log("Hit anim played");
            if (healthbar.health <= 0)
            {
                Death();
                StartCoroutine(Wait(3));

            }
        }
    }



    // Adds a menu option to reset stats
    [ContextMenu("Reset Stats")]
    void ResetStats()
    {
        //Debug.Log("Perform operation");
        speed = 6.0f;
    }

    IEnumerator Wait(float time)
    {
        //Death();
        yield return new WaitForSeconds(time);
        //SceneManager.LoadScene("GameOver");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator ActionWhileMoving(float time)
    {
        speed = 0f;
        yield return new WaitForSeconds(time);
        speed = 4.0f;
    }
}
