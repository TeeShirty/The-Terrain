using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Character character;
    
    public Image healthBar;
    public float health;
    public float startHealth = 100f;


    public void OnTakeDamage(float damage)
    {
        health = health - damage;
        healthBar.fillAmount = health / startHealth;

        //if(health == 0f)
        //{
        //    Debug.Log("Too much damage taken, Player dies");
        //    GetComponent<Character>().Death();
        //}
    }
}
