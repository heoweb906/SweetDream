using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �װų� ����ȭ������ ���� �� �ǹ�Ÿ���� �������� �����ؾ� ��


    public GameManager gameManager;
    public PlayerInformation playerInformation;

    [Header("�÷��̾� ����")]
    public int hp = 4; // �÷��̾� ü��
    public int weaponNumber; // 1 = ����, 2 = ���, 3 = �Ķ�
    public float moveSpeed;  // �÷��̾� ���ǵ�
    public float jumpForce; // ���� ��
    public float rollSpeed = 10f; // ������ �ӵ�
    public float rollDuration = 0.5f; // ������ ���� �ð�
    public int attackDamage = 10;    // ���� ������
    [Space(10f)]


    [Header("������Ʈ")]
    public GameObject weapon_main;
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;
    public GameObject weapon4_Gatling;
    [Space(10f)]


    [Header("���� ���� ����")]
    public float hAxis; // �̵� �� ���� ���� ���� ����
    public float vAxis; // �̵� �� ���� ���� ���� ����
    public float turnSpeed; // ȸ�� �ӵ�
    public float attackRange = 50000.0f; // ���� ����

    private float timeSinceLastAttack = 0f;  // FeverAttack
    public float attackInterval = 0.5f; // everAttack Ray�� �߻��ϴ� �ֱ�
    [Space(10f)]


    [Header("����")]
    public AudioSource soundGun; // ���� �������
    public AudioSource soundRoll; // Ÿ�̹� ���ߴ� ��Ʈ�γ�


    // #. �÷��̾� Ű �Է�
    private bool jDown; // ���� Ű
    private bool wDown; // ��ũ���� Ű
    private bool shiftDown; // ������ Ű
    private bool rDown; // ������ Ű
    private int key_weapon = 1;


    private bool isJumping; // ���� ������ ���θ� ��Ÿ���� ����
    private bool isRolling; // ������ �ִ� ������ ���θ� ��Ÿ���� ����

    public Rigidbody rigid;
    public Camera mainCamera;

    Vector3 moveVec; // �÷��̾��� �̵� ��


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager�� ã�Ƽ� �Ҵ�
        playerInformation = FindObjectOfType<PlayerInformation>();
        CamLock();
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        CamLock(); // ���� ���� �� ī�޶� ��
        if(!(gameManager.isFever))
        {
            WeaponChange_SceneChange(playerInformation.WeponColor); // ���� ��ȯ�� �� ��� �ִ� ������ ������ �̾������� ���� ��ü �Լ� 1ȸ ����
        }
        else if(gameManager.isFever)
        {
            FeverOn();
            weaponNumber = playerInformation.WeponColor;
        }
    }


    private void Update()
    {
        GetInput();
        Move();
        
        if(!(gameManager.isFever))
        {
            Attack();
        }
        else if(gameManager.isFever)
        {
            if (Input.GetButton("Fire1"))
            {
                timeSinceLastAttack += Time.deltaTime;

                if (timeSinceLastAttack >= attackInterval)
                {
                    FeverAttack();
                    timeSinceLastAttack = 0f; 
                }
            }
        }
    }


    private void Reload()
    {
        if(rDown && gameManager.rhythmCorrect && !(gameManager.isReload))
        {
            gameManager.isReload = true;

            weapon1.SetActive(false);
            weapon2.SetActive(false);
            weapon3.SetActive(false);
            gameManager.ActivateImage(1);
        }
    }


    private void GetInput()  // �Է��� �޴� �Լ�
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButtonDown("Jump");
        wDown = Input.GetButton("Bowingdown");
        shiftDown = Input.GetButtonDown("Roll");
        rDown = Input.GetButtonDown("Reload");

        if(rDown)
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            key_weapon = 1;
            WeaponChange(key_weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            key_weapon = 2;
            WeaponChange(key_weapon);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            key_weapon = 3;
            WeaponChange(key_weapon);
        }

    }


    void Move() // �̵��� �����ϴ� �Լ�
    {
        // �÷��̾��� �ٶ󺸴� ������ �̿��Ͽ� �̵� ���͸� ���
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        moveVec = transform.forward * moveVec.z + transform.right * moveVec.x;

        // �̵� �ӵ��� �����ϰ� ��ũ���� ���ο� ���� ����
        float currentMoveSpeed = moveSpeed * (wDown ? 0.3f : 1f);
        transform.position += moveVec * currentMoveSpeed * Time.deltaTime;

        // ���� üũ
        if (jDown && !isJumping)
        {
            Jump();
        }

        if (shiftDown && !isRolling && gameManager.rhythmCorrect)
        {
            isRolling = true;
            StartCoroutine(PerformRoll());
        }
    }

    private IEnumerator PerformRoll()
    {
        // ������ ���� �̵� �ӵ��� ������Ű��, ������ ���� �̵� �������� ����
        soundRoll.Play();
        float originalMoveSpeed = moveSpeed;
        moveSpeed = rollSpeed;
        Vector3 rollDirection = moveVec;

        // ���� �ð� ���� ������
        yield return new WaitForSeconds(rollDuration);

        // ������ ���� �� ���� �̵� �ӵ��� �������� ����
        moveSpeed = originalMoveSpeed;
        isRolling = false;
    }


    private void Jump() // ����
    {
        isJumping = true;
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    private void Attack()
    {
        if (Input.GetButtonDown("Fire1") && gameManager.rhythmCorrect)
        {
            if (!(gameManager.isReload) && gameManager.bulletCount > 0)
            {
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                RaycastHit hit;
                bool hasHit = Physics.Raycast(ray, out hit, attackRange);

                Debug.DrawRay(ray.origin, ray.direction * attackRange, hasHit ? Color.red : Color.green, 0.1f); // ���� �ð�ȭ
                gameManager.bulletCount--;

                if (hasHit && hit.collider.CompareTag("Monster"))
                {
                    Monster monster = hit.collider.GetComponent<Monster>();
                    if (monster != null)
                    {
                        if (monster.monsterColor == weaponNumber)
                        {

                            monster.TakeDamage(attackDamage);
                            gameManager.ComboBarBounceUp();
                            //soundGun.Play(); // ���� �Ҹ� ������ ����
                        }
                    }
                }
            }
        }
        else if(Input.GetButtonDown("Fire1") && !(gameManager.rhythmCorrect))   // ���� Ʋ�� Ÿ�ֿ̹� �����ϸ�
        {
            gameManager.ComboBarDown(20);
        }
    }

    private void FeverAttack()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, attackRange);

        Debug.DrawRay(ray.origin, ray.direction * attackRange, hasHit ? Color.red : Color.green, 0.1f); // ���� �ð�ȭ
        gameManager.bulletCount--;

        if (hasHit && hit.collider.CompareTag("Monster"))
        {
            Monster monster = hit.collider.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(attackDamage);
                //soundGun.Play(); // ���� �Ҹ� ������ ����
            }
        }
    }




    public void OnDamage(int dmg) // �������� �޾��� ���� �Լ�, ���͵��� ����� �� �ֵ��� public���� ��
    {
        if(hp >= 1)
        {
            hp -= dmg;
            gameManager.ActivateHpImage(hp - 1);
        }
    }

    public void FeverOn()
    {
        weapon_main.SetActive(false);
        weapon1.SetActive(false);
        weapon2.SetActive(false);
        weapon3.SetActive(false);
        weapon4_Gatling.SetActive(true);
        attackDamage = 3;
    }

    public void FeverOff()
    {
        weapon_main.SetActive(true);
        weapon4_Gatling.SetActive(false);
        WeaponChange_SceneChange(weaponNumber);
        gameManager.bulletCount = 10;
        attackDamage = 10;
    }

    public void WeaponChange(int number) 
    {
        if (gameManager.rhythmCorrect && gameManager.isReload)
        {
            if (number == 1)
            {
                weaponNumber = 1;
                playerInformation.WeponColor = 1;
                weapon1.SetActive(true);
                weapon2.SetActive(false);
                weapon3.SetActive(false);

                gameManager.isReload = false;
                gameManager.bulletCount = 10;
            }
            if (number == 2)
            {
                weaponNumber = 2;
                playerInformation.WeponColor = 2;
                weapon1.SetActive(false);
                weapon2.SetActive(true);
                weapon3.SetActive(false);

                gameManager.isReload = false;
                gameManager.bulletCount = 10;
            }
            if (number == 3)
            {
                weaponNumber = 3;
                playerInformation.WeponColor = 3;
                weapon1.SetActive(false);
                weapon2.SetActive(false);
                weapon3.SetActive(true);

                gameManager.isReload = false;
                gameManager.bulletCount = 10;
            }

            gameManager.ActivateImage(number);
        }
    }

    public void WeaponChange_SceneChange(int number)    // ���� ��ȯ�� �� ��� �ִ� ������ ������ �̾������� �ϱ� ���� �Լ�
    {
        if (number == 1)
        {
            weaponNumber = 1;
            playerInformation.WeponColor = 1;
            weapon1.SetActive(true);
            weapon2.SetActive(false);
            weapon3.SetActive(false);
        }
        if (number == 2)
        {
            weaponNumber = 2;
            playerInformation.WeponColor = 2;
            weapon1.SetActive(false);
            weapon2.SetActive(true);
            weapon3.SetActive(false);
        }
        if (number == 3)
        {
            weaponNumber = 3;
            playerInformation.WeponColor = 3;
            weapon1.SetActive(false);
            weapon2.SetActive(false);
            weapon3.SetActive(true);
        }

        if(gameManager.isReload)
        {
            weapon1.SetActive(false);
            weapon2.SetActive(false);
            weapon3.SetActive(false);
        }

        gameManager.ActivateImage(number);
    }


    #region camLock
    private void CamLock() // ���콺 Ŀ���� ����� �Լ�
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }
    #endregion
}
