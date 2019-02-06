using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region movement_variable
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region attack_variables
    public float damage;
    public float attackspeed; // flat value 
    float attackTimer; //keep track of time waited
    public float hitboxTiming;
    public float endAnimationTiming;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region health_variables
    public float maxHealth; //keep this cap
    float currHealth;
    public Slider hpSlider;
    #endregion

    #region physics_components
    Rigidbody2D playerRB;
    #endregion

    #region animation_components
    Animator anim;
    #endregion

    #region Unity_functions
    // called once on creation
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        attackTimer = 0;

        currHealth = maxHealth;

        hpSlider.value = currHealth / maxHealth;

    }
    // called every frame
    private void Update()
    {
        if (isAttacking)
        {
            return;
        }

        //access input values
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();

        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
        {
            Attack();
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Interact();
        }
    }
    #endregion


    #region movement_functions
    // moves player based on inputs
    private void Move()
    {
        anim.SetBool("Moving", true);

        //if player press d
        if (x_input > 0)
        {
            playerRB.velocity = Vector2.right * movespeed;
            currDirection = Vector2.right;
        }
        //press a
        else if (x_input < 0)
        {
            playerRB.velocity = Vector2.left * movespeed;
            currDirection = Vector2.left;
        }
        // press w
        else if (y_input > 0)
        {
            playerRB.velocity = Vector2.up * movespeed;
            currDirection = Vector2.up;
        }
        //press s
        else if (y_input < 0)
        {
            playerRB.velocity = Vector2.down * movespeed;
            currDirection = Vector2.down;
        }
        else
        {
            playerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);

        }

        // set animator directional values
        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    #region attack_functions
    // attacks in dir player is facing
    private void Attack()
    {
        Debug.Log("Attacking Now");
        Debug.Log(currDirection);

        //handles attack animation and calculates hitboxes
        StartCoroutine(AttackRoutine());

        attackTimer = attackspeed;
    }

    //handle animation n hitboxes for attqack mechanism
    IEnumerator AttackRoutine()
    {
        //freeze player for duration of attack
        isAttacking = true;
        playerRB.velocity = Vector2.zero;

        //attck animation
        anim.SetTrigger("Attack");

        //start sound effect
        FindObjectOfType<AudioManager>().Play("PlayerAttack");

        //brief pause before calculating hitbox
        yield return new WaitForSeconds(hitboxTiming);

        Debug.Log("Cast hitbox now");

        // create hitbox
        RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + currDirection, Vector2.one, 0f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Enemy")) 
            {
                Debug.Log("tons of damage");
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(endAnimationTiming);

        //unfreeze player
        isAttacking = false;
    }

    #endregion

    #region health_functions
    public void TakeDamage(float value)
    {
        //call sound effect
        FindObjectOfType<AudioManager>().Play("PlayerHurt");

        // decrease health
        currHealth -= value;
        Debug.Log("player Health is now " + currHealth.ToString());

        // change UI
        hpSlider.value = currHealth / maxHealth;

        // check if dead
        if (currHealth <= 0)
        {
            Die();
        }

    }

    public void Heal(float value)
    {
        // increase hp
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("player Health is now " + currHealth.ToString());

        // change UI
        hpSlider.value = currHealth / maxHealth;
    }

    //destroy player object, trigger end scene
    private void Die()
    {
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        Destroy(this.gameObject);

        GameManager gm = GameObject.FindWithTag("GameController");
        gm.GetComponent<GameManager>().LoseGame();
    }

    #endregion

    #region interact_functions
    void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<Chest>().Interact();
            }
        }

    }
    #endregion
}
