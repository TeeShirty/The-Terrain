using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class EnemyDiGraph : MonoBehaviour
{
    private Rigidbody rigidbody;
    public GameObject player;
    public float speed;

    public List<Transform> patrolNodes = new List<Transform>();
    public DirectedGraph<Transform> patrolGraph = new DirectedGraph<Transform>();
    //public int maxOutgoingEdges = 5;

    private Transform currentTransform;
    private int sourceNode;
    private int destinationNode;
    private int randomNode;
    private int currentNode;

    public float minimumDistance;
    public bool isChasing;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        //garbage values
        isChasing = false;

        if (speed == 0)
        {
            speed = 3f;
        }

        if (minimumDistance == 0)
        {
            minimumDistance = 3f;
        }

        //shuffles duplicates and gets the next non-duplicate node
        for (int i = 0; i < patrolNodes.Count; i++)
        {
            int randomIndex = Random.Range(i, patrolNodes.Count);
            Transform temp = patrolNodes[i];
            patrolNodes[i] = patrolNodes[randomIndex];
            patrolNodes[randomIndex] = temp;
        }

        //Add a node
        for (int i = 0; i < patrolNodes.Count; i++)
        {
            patrolGraph.addNode(patrolNodes[i]);

        }

        //add an edge between the src and dest node
        for (int i = 0; i < patrolNodes.Count; i++)
        {
            if (i == patrolNodes.Count - 1)
            {
                patrolGraph.addEdge(patrolNodes[i], patrolNodes[0]);
            }
            else
            {
                patrolGraph.addEdge(patrolNodes[i], patrolNodes[i + 1]);
            }
        }

        randomNode = Random.Range(0, patrolNodes.Count);

        currentTransform = patrolGraph.findNode(patrolNodes[randomNode]).getData();
        currentNode = randomNode;
    }

    private void Update()
    {
        //To chase the player
        if (Vector3.Distance(transform.position, player.transform.position) < minimumDistance)
        {
            isChasing = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) > minimumDistance)
        {
            isChasing = false;
        }

        if (isChasing == true && currentTransform != null)
        {
            Vector3 position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime);
            Debug.DrawLine(this.transform.position, player.transform.position, Color.red);
            rigidbody.MovePosition(position);
        }
        else if (isChasing == false && currentTransform != null)
        {
            Vector3 position = Vector3.MoveTowards(transform.position, currentTransform.transform.position, speed * Time.fixedDeltaTime);
            Debug.DrawLine(this.transform.position, currentTransform.transform.position, Color.yellow);
            rigidbody.MovePosition(position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Upon collision find the next target patrol node
        if (other.tag == "Patrol")
        {
            currentTransform = patrolGraph.findNode(other.transform).getOutgoing()[0].getData();
            Debug.Log(currentTransform);
        }
    }
}
