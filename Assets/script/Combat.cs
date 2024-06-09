using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{

    public AudioSource src;
    public AudioClip sfx1, sfx2;

    [SerializeField]
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private float stunDamageAmount = 1f;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;

    private bool gotInput, isAttacking, isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;

    private AttackDetails attackDetails;

    private Animator anim;
    private PlayerMouvement PM;
    private PlayerStat PS;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        PM = GetComponent<PlayerMouvement>();
        PS = GetComponent<PlayerStat>();
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
        
    }

    private void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Vertical"))
        {
            if (combatEnabled)
            {
                //combat
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            //faire attaque 1
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            //attendre new input
            gotInput = false;
        }
    }
    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails.damageAmount = attack1Damage;
        attackDetails.position = transform.position;
        attackDetails.stunDamageAmount = stunDamageAmount;

        src.clip = sfx2;
        src.Play();

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
            //Instanciate hit particle

            src.clip = sfx1;
            src.Play();
        }
    }
    private void FinishAtack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }
    private void Damage(AttackDetails attackDetails){
        if(!PM.GetDashStatus()){
            int direction;
            //Damage Player attackDetails
            PS.DecreaseHealth(attackDetails.damageAmount);

            if(attackDetails.position.x < transform.position.x){
                direction = 1;
            }else{
                direction = -1;
            }
            PM.Knockback(direction);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
