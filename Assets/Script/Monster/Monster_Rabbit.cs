using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Rabbit : Monster
{
    [Header("°ü·Ã ¿ÀºêÁ§Æ® / º¯¼ö")]
    public BoxCollider attackArea;
    public bool isChase = true;
    public bool isAttack;

    public Animator anim;
    private Transform player;
    private Rigidbody rb;
    private NavMeshAgent nav;
    public new CapsuleCollider collider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        nav.SetDestination(player.position);

        if (currentHealth <= 0 && !doDie)
        {
            collider.enabled = false;
            FixPosition(transform.position);
            doDie = true;
            isChase = false;
            nav.isStopped = true; // NavMeshAgent ¸ØÃã
            nav.speed = 0;
            nav.angularSpeed = 0;
            anim.SetTrigger("doDie");
        }

        if(!isChase)
        {
            FixPosition(transform.position);
            nav.isStopped = true; // NavMeshAgent ¸ØÃã
            nav.speed = 0;
            nav.angularSpeed = 0;
            anim.SetBool("isWalk",false);
        }
        else
        {
            nav.isStopped = false; // NavMeshAgent ¸ØÃã
            nav.speed = 12;
            nav.angularSpeed = 720;
            anim.SetBool("isWalk", true);
        }
    }

    void FixedUpdate() 
    {
        Targetting();
        FreezeVelocity();
    }


    void Targetting()
    {
        float targetRadius = 2f;
        float targetRange = 3f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange,
                                    LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack && !doDie)
        {
            StartCoroutine(Attack());
        }
    }


    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.6f);
        AttackAreaOn();
        yield return new WaitForSeconds(0.1f);
        AttackAreaOff();

        yield return new WaitForSeconds(1.3f);
        isChase = true;
        isAttack = false;
    }


    public void AttackAreaOn()
    {
        attackArea.enabled = true;
    }
    public void AttackAreaOff()
    {
        attackArea.enabled = false;
    }


    void FreezeVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

  
}
