using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Rigidbody rigidbody;

    public int enemyID;
    public int health;

    public HealthBar healthBar;

    public GameObject[] spawnPowerUps;

    public GameObject target;
    public Character player;
    public Transform playerPosition;
    private float playerHealth;

    bool isAlive;

    public GameObject powerUp;
    public Transform spawnPoint;

    public ParticleSystem bloodSpatter;

    //using enums for the dropdown menu
    enum EnemyType { Chase, Patrol }
    [SerializeField] EnemyType enemyType;

    //to change patrol we change physics of character

    enum PatrolType { DistanceBased, TriggerBased }
    [SerializeField] PatrolType patrolType;


    public bool autoGenPath;
    public string pathName;

    //to store
    public GameObject[] path;

    public int pathIndex;

    public float distanceToNextNode;

    public float aggroDistance;
    public float attackDistance;

    UniqueId uniqueId;

    // Start is called before the first frame update
    void Start()
    {
        enemyID = GetInstanceID();

        isAlive = true;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        //playerHealth = GetComponent<HealthBar>().health; //pulling the float within the health script

        animator.applyRootMotion = false;

        rigidbody.isKinematic = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;



        //Checks
        if (string.IsNullOrEmpty(pathName))
        {
            pathName = "PatrolNode";
        }

        if (distanceToNextNode <= 0)
        {
            distanceToNextNode = 1.0f;
        }

        if (!target && enemyType == EnemyType.Chase)
        {
            target = GameObject.FindWithTag("Player");
        }
        else if (enemyType == EnemyType.Patrol)
        {
            if (autoGenPath == true)
            {
                path = GameObject.FindGameObjectsWithTag(pathName);
            }
            if (path.Length > 0)
            {
                target = path[pathIndex];
            }
        }

        if(aggroDistance == 0)
        {
            aggroDistance = 10f;
        }

        if(attackDistance == 0)
        {
            attackDistance = 1f;
        }

        //always make checks
        if (target == true)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.transform.position, gameObject.transform.position);
        //Patrol
        if (target == true && enemyType == EnemyType.Patrol && patrolType == PatrolType.DistanceBased)
        {
            Debug.DrawLine(transform.position, target.transform.position, Color.red);

            /*if(Vector3.Distance(transform.position, target.transform.position)<distanceToNextNode)
            {}*/

            //or
            /*if((transform.position - target.transform.position).magnitude < distanceToNextNode)
            {}*/

            //or
            if (agent.remainingDistance < distanceToNextNode)
            {
                ChooseNextPatrolNode();
            }
        } 
        //Chase
        else if(target == true && enemyType == EnemyType.Chase)
        {
            target = player.gameObject;
            //transform.LookAt(playerPosition.transform);
            //Debug.Log(player);
        }


        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        //Chase or Patrol
        if (distanceToPlayer <= 10f)
        {
            enemyType = EnemyType.Chase;
            animator.SetBool("IsChasing", true);
            
            if (distanceToPlayer <= attackDistance)
            {

                //animator.SetBool("IsAttacking", true);
                animator.SetTrigger("IsAttack");
            }
        }
        if (distanceToPlayer > 10f) 
        {
            enemyType = EnemyType.Patrol;
            animator.SetBool("IsChasing", false);
            target = path[pathIndex];
        }

        if (target == true)
        {
            agent.SetDestination(target.transform.position);
            Debug.DrawLine(transform.position, target.transform.position, Color.yellow);
        }

        //animator.SetBool("IsGrounded", !agent.isOnOffMeshLink);
        animator.SetBool("IsGrounded", true);
        //will follow the character rotation
        animator.SetFloat("Speed", transform.InverseTransformDirection(agent.velocity).z);
    }

    public void SaveGamePrepare()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies = GameManager.StateManager.gameState.enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].enemyID == enemyID)
            {
                GameManager.StateManager.gameState.enemies.Remove(enemies[i]);
                break;
            }
        }


        LoadSaveManager.GameStateData.DataEnemy data = new LoadSaveManager.GameStateData.DataEnemy();

        data.enemyID = enemyID;
        Debug.Log(enemyID);
        //data.enemyID = GetComponent<UniqueId>().uniqueId;
        //Debug.Log(GetComponent<UniqueId>().uniqueId);
        //data.health = health;

        data.positionRotationScale.positionX = transform.position.x;
        data.positionRotationScale.positionY = transform.position.y;
        data.positionRotationScale.positionZ = transform.position.z;

        data.positionRotationScale.rotationX = transform.localEulerAngles.x;
        data.positionRotationScale.rotationY = transform.localEulerAngles.y;
        data.positionRotationScale.rotationZ = transform.localEulerAngles.z;

        data.positionRotationScale.scaleX = transform.localScale.x;
        data.positionRotationScale.scaleY = transform.localScale.y;
        data.positionRotationScale.scaleZ = transform.localScale.z;

        GameManager.StateManager.gameState.enemies.Add(data);
        Debug.Log(GameManager.StateManager.gameState.enemies.Count);

    }

    public void LoadGameComplete()
    {
        List<LoadSaveManager.GameStateData.DataEnemy> enemies = GameManager.StateManager.gameState.enemies;

        LoadSaveManager.GameStateData.DataEnemy data = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].enemyID == enemyID)
            {
                data = enemies[i];
                break;
            }
        }

        //if there is no enemy found, then it is destroyed on save
        if(data == null)
        {
            Destroy(gameObject);
            return;
        }

        enemyID = data.enemyID;

        transform.position = new Vector3(data.positionRotationScale.positionX, data.positionRotationScale.positionY, data.positionRotationScale.positionZ);

        transform.localRotation = Quaternion.Euler(data.positionRotationScale.rotationX, data.positionRotationScale.rotationY, data.positionRotationScale.rotationZ);

        transform.localScale = new Vector3(data.positionRotationScale.scaleX, data.positionRotationScale.scaleY, data.positionRotationScale.scaleZ);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Projectile" && isAlive == true)
        {
            Debug.Log("Hit");
            animator.SetTrigger("IsAlive");
            isAlive = false;
            this.enabled = false;
            StartCoroutine(PowerUpSpawner(2f));
            //stop enemy from moving after death
            Debug.Log("Coroutine");
        }
    }

    public void ChooseNextPatrolNode()
    {
        //if there is stuff in the array
        if (path.Length > 0)
        {
            //sequencial
            pathIndex++;

            //restart when get to end
            pathIndex %= path.Length;

            //or
            if(pathIndex >= path.Length)
            {
                pathIndex = 0;
            }

            //updates to go to the next path
            target = path[pathIndex];
            
        }
    }
    
    void attackPlayer()
    {
        player.healthbar.OnTakeDamage(25f);
        player.HitAnimation();
    }

    public IEnumerator PowerUpSpawner(float time)
    {
        bloodSpatter.Play();
        yield return new WaitForSeconds(time);
        Instantiate(spawnPowerUps[Random.Range(0, spawnPowerUps.Length)], spawnPoint.transform.position, spawnPoint.transform.rotation);
        Debug.Log("Instantiate");
        gameObject.SetActive(false);
        Debug.Log("Set to false");
    }
}
