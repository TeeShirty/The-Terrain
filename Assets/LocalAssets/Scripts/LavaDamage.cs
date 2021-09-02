using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    float timer = 0;
    // set this up in the inspector!
    public float damageTime = 2;
    public float damageAmount = 15;
    void OnTriggerStay(Collider hit)
    {
        if (hit.gameObject.tag == "Player")
        {
            // Damage the player every 'damageTime'
            if (timer >= damageTime)
            {
                timer -= damageTime;
                // use the generic version of GetComponent, because it's faster
                HealthBar hp = hit.GetComponent<HealthBar>();
                hp.OnTakeDamage(1);
            }
            timer += Time.deltaTime;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Reset the damage timer
            timer = 0;
        }
    }
}
