using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Rabbit : Monster
{
    [Header("���� ������Ʈ / ����")]
    public BoxCollider attackArea;
    public bool isChase = true;
    public bool isAttack;

    public Animator anim;
    public Transform player;
    private Rigidbody rb;
    private NavMeshAgent nav;
    public new CapsuleCollider collider;

    // ���� ������Ʈ�� �±׷� ã�� �� �̾��� Ȯ��

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerStep").transform;

        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        nav.SetDestination(player.position);

        if (currentHealth <= 0 && !doDie)
        {
            AttackAreaOff();
            collider.enabled = false;
            FixPosition(transform.position);
            doDie = true;
            isChase = false;
            nav.isStopped = true; // NavMeshAgent ����
            nav.speed = 0;
            nav.angularSpeed = 0;
            anim.SetTrigger("doDie");

            nav.velocity = Vector3.zero;  // NavMeshAgent�� �̵� �ӵ� �ʱ�ȭ
            rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
            rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
        }

        if(!isChase)
        {
            FixPosition(transform.position);
            nav.isStopped = true; // NavMeshAgent ����
            nav.speed = 0;
            nav.angularSpeed = 0;
            anim.SetBool("isWalk",false);

            nav.velocity = Vector3.zero;  // NavMeshAgent�� �̵� �ӵ� �ʱ�ȭ
            rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
            rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
        }
        else
        {
            nav.isStopped = false; // NavMeshAgent ����
            nav.speed = 12;
            nav.angularSpeed = 720;
            anim.SetBool("isWalk", true);
        }
        Targetting();
    }

    void FixedUpdate() 
    {
        
        FreezeVelocity();
    }


    void Targetting()
    {
        float targetRadius = 3f;
        float targetRange = 2f;

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

        // ���� ���� �� ��� �ӵ��� ȸ���� 0���� ����
        nav.velocity = Vector3.zero;  // NavMeshAgent�� �̵� �ӵ� �ʱ�ȭ
        rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
        rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ

        yield return new WaitForSeconds(0.6f);
        AttackAreaOn();
        yield return new WaitForSeconds(0.1f);
        AttackAreaOff();

        yield return new WaitForSeconds(1.3f);
        if (!doDie)
        {
            isChase = true;
        }
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
