using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region movement_variables
    public float movespeed;
    #endregion

    #region physics_components
    Rigidbody2D enemyRB;
    #endregion

    #region targeting_variables
    public Transform player;
    #endregion

    #region attack_variables
    public float explosionDamage;
    public float explosionRadius;
    public GameObject explosionObj;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region Unity_functions
    private void Awake()
    {
        enemyRB = GetComponent<Rigidbody2D>();

        currHealth = maxHealth;
    }

    private void Update()
    {
        //check if we know where player is
        if (player == null)
        {
            return;
        }

        Move();
    }
    #endregion

    #region movement_functions
    //move toward player
    private void Move()
    {
        //some math
        Vector2 direction = player.position - transform.position;
        enemyRB.velocity = direction.normalized * movespeed; //make magnitude of direction 1


    }
    #endregion

    #region attack_functions
    // raycast (?) box for player, spawn explosion prefab
    private void Explode()
    {

        FindObjectOfType<AudioManager>().Play("Explosion");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {

            if (hit.transform.CompareTag("Player"))
            {
                // cause damage
                Debug.Log("hits player with explosion");
                hit.transform.GetComponent<PlayerController>().TakeDamage(explosionDamage);

                //spawn explosion prefab in game
                Instantiate(explosionObj, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Explode();
        }
    }

    #endregion

    #region health_functions
    //enemy take damage
    public void TakeDamage(float value)
    {
        FindObjectOfType<AudioManager>().Play("BatHurt");

        currHealth -= value;
        Debug.Log("enemy Health is now " + currHealth.ToString());
    

        // check if dead
        if (currHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
