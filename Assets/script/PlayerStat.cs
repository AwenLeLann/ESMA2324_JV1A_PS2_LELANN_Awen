using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{

    public AudioSource src;
    public AudioClip sfx1;
    public Image healthBar;
    public float healthAmount = 100f;


    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle;

    [SerializeField]
    private float currentHealth;

    private GameManager GM;

    private void Start(){
        currentHealth = maxHealth;
        HealthBar();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void DecreaseHealth(float amount){
        currentHealth -= amount;
        HealthBar();
        
        src.clip = sfx1;
        src.Play();

        if(currentHealth <= 0.0f){
            Die();
        }
    }

    public void HealthBar()
    {
        healthBar.fillAmount = currentHealth / 100f;
    }

    private void Die(){
        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        GM.Respawn();
        Destroy(gameObject);
    }
}
