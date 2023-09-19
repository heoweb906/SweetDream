using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Bear : Monster
{
    [Header("관련 오브젝트 / 변수")]
    public BoxCollider attackArea;
    public bool isChase = true;
    public bool isAttack;

    private Animator anim;
    private Transform player;
    private Rigidbody rb;
    private NavMeshAgent nav;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        nav.SetDestination(player.position);
        if (currentHealth <= 0 && !doDie)
        {
            doDie = true;
            isChase = false;
            nav.speed = 0;
            anim.SetTrigger("doDie");
        }

        if (!isChase)
        {
            anim.SetBool("isWalk",false);
            nav.speed = 0;
            nav.angularSpeed = 0;
        }
        else
        {
            anim.SetBool("isWalk", true);
            nav.speed = 1.2f;
            nav.angularSpeed = 250;
        }
    }

    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
    }


    void Targetting()
    {
        float targetRadius = 1.6f;
        float targetRange = 1.6f;

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

        yield return new WaitForSeconds(2f);
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
