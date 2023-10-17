using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Security.Cryptography;

public class Player : MonoBehaviour
{
    // �װų� ����ȭ������ ���� �� �ǹ�Ÿ���� �������� �����ؾ� ��

    public GameManager gameManager;
    public PlayerInformation playerInformation;
    public UI_InGame ingame_UI;

    [Header("�÷��̾� ����")]
    public int hp = 4; // �÷��̾� ü��
    public int weaponNumber; // 1 = ����, 2 = ���, 3 = �Ķ�
    public float moveSpeed;  // �÷��̾� ���ǵ�
    public float jumpForce; // ���� ��
    public float rollSpeed = 40f; // ������ �ӵ�
    public float rollDuration = 0.5f; // ������ ���� �ð�
    public int attackDamage = 10;    // ���� ������

    public bool isDie;
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
    public AudioSource soundGun; // �ѼҸ�
    public AudioSource soundGun_Fail; // ���� ���߱� ���� ����
    public AudioSource soundReload_In; // ���� �Ҹ� (źâ ����)
    public AudioSource soundReload_Out; // ���� �Ҹ� (źâ ����)
    public AudioSource soundMiniGun; // ������ �Ҹ�
    public AudioSource soundRoll; // ������ �Ҹ�


    [Header("FEEL ����")]
    public MMF_Player mmfPlayer_Camera;
    public MMF_Player mmfPlayer_OnDamage;
    public MMF_Player mmfPlayer_OnDie;
    // mmfPlayer_Camera?.PlayFeedbacks();




    // #. �÷��̾� Ű �Է�
    private bool jDown; // ���� Ű
    private bool wDown; // ��ũ���� Ű
    private bool shiftDown; // ������ Ű
    private bool rDown; // ������ Ű
    private int key_weapon = 1;


    private bool isJumping; // ���� ������ ���θ� ��Ÿ���� ����
    private bool isRolling; // ������ �ִ� ������ ���θ� ��Ÿ���� ����
    private bool isDamaging; // ������ �޾� ������ ����

    public Rigidbody rigid;
    public Camera mainCamera;



    // #. �ִϸ����� ����
    public Animator gunAnimator;
    Vector3 moveVec; // �÷��̾��� �̵� ��

    // #. ���̾� ���� ����
    private float layerChangeDuration = 1.5f;     // ���̾� ���� ���� �ð� (��)
    private bool isChangingLayer = false;
    private int playerLayer; // Player�� �ʱ� ���̾�


    // #. ���� ����
    private Vector3 rayDirection; // ����ĳ��Ʈ�� �߻�� ������ �����ϴ� ����



    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager�� ã�Ƽ� �Ҵ�
        playerInformation = FindObjectOfType<PlayerInformation>();
        rigid = GetComponent<Rigidbody>();

        CamLock();
        playerLayer = gameObject.layer;
    }

    private void Start()
    {
        CamLock(); // ���� ���� �� ī�޶� ��
        SetPlayerSound(); // ȯ�� ������ �µ��� ȿ���� ���� ����; 
                          // �޴�ȭ���� UI ��Ʈ�ѷ��� �÷��̾��� �Լ��� �����ų �� ���� ������ 1�� ��������ִ� ����


        if (!(gameManager.isFever))
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
            if (Input.GetButton("Fire1") && !isDie)
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

            soundReload_Out.Play();
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ingame_UI.OnOffSettingPanel();
        }

    }


    void Move() // �̵��� �����ϴ� �Լ�
    {
        // �÷��̾��� �ٶ󺸴� ������ �̿��Ͽ� �̵� ���͸� ���
        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // Rigidbody�� �ӵ� ����
        Vector3 newVelocity = transform.forward * moveVec.z * moveSpeed + transform.right * moveVec.x * moveSpeed;
        newVelocity.y = rigid.velocity.y; // ���� ���� �ӵ� ����
        rigid.velocity = newVelocity;

        // �÷��̾�� ���� �߷��� ��
        rigid.AddForce(Vector3.down * 15f, ForceMode.Acceleration);


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
        mmfPlayer_Camera?.PlayFeedbacks();

        float originalMoveSpeed = moveSpeed;
        moveSpeed = rollSpeed;
        Vector3 rollDirection = moveVec;

        // ���� �ð� ���� ������
        yield return new WaitForSeconds(rollDuration);

        // ������ ���� �� ���� �̵� �ӵ��� �������� ����
        moveSpeed = originalMoveSpeed;
        isRolling = false;
    }


    private void Jump()
    {
        isJumping = true;
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    private void Attack()
    {
        if (Input.GetButtonDown("Fire1") && gameManager.rhythmCorrect && !isDie)
        {
            if (!(gameManager.isReload) && gameManager.bulletCount > 0)
            {
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                RaycastHit hit;
                bool hasHit = Physics.Raycast(ray, out hit, attackRange);

                Debug.DrawRay(ray.origin, ray.direction * attackRange, hasHit ? Color.red : Color.green, 0.1f); // ���� �ð�ȭ
                gameManager.bulletCount--;

                if (hasHit && (hit.collider.CompareTag("Monster") || hit.collider.CompareTag("Boss")))
                {
                    rayDirection = ray.direction.normalized;

                    Monster monster = hit.collider.GetComponent<Monster>();
                    if (monster != null)
                    {
                        if (monster.monsterColor == weaponNumber)
                        {
                            monster.TakeDamage(attackDamage);
                            gameManager.ComboBarBounceUp();
                        }
                    }

                    Boss_Swan boss = hit.collider.GetComponent<Boss_Swan>();
                    if (boss != null)
                    {
                        if (boss.monsterColor == weaponNumber)
                        {
                            boss.TakeDamage(attackDamage);
                            gameManager.ComboBarBounceUp();
                        }
                    }

                    Boss_Bullet_Color bullet = hit.collider.GetComponent<Boss_Bullet_Color>();
                    if (bullet != null)
                    {
                        if (bullet.numColor == weaponNumber)
                        {
                            // �浹 �������� ������ ���� ���͸� ���
                            bullet.SetNewDirection(rayDirection);
                        }
                    }

                }
                gunAnimator.SetTrigger("Fire");
                soundGun.Play(); 
            }
        }
        else if(Input.GetButtonDown("Fire1") && !(gameManager.rhythmCorrect))   // ���� Ʋ�� Ÿ�ֿ̹� �����ϸ�
        {
            gameManager.ComboBarDown(20);
            soundGun_Fail.Play();
        }
    }

    private void FeverAttack() // �ǹ� ���� �Լ�
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
            }
        }
        soundMiniGun.Play();
    }




    public void OnDamage(int dmg) // �������� �޾��� ���� �Լ�, ���͵��� ����� �� �ֵ��� public���� ��
    {
        if (hp >= 1 && !isDie && !isDamaging)
        {
            isDamaging = true;
            hp -= dmg;
            gameManager.ActivateHpImage(hp - 1);
            mmfPlayer_OnDamage?.PlayFeedbacks();

            // ���̾� ������ ���� ������ ���� ���� ����
            if (!isChangingLayer)
            {
                StartCoroutine(ChangeLayerTemporarily());
            }
        }


        // #. ���� �Լ� ���
        if(hp == 0 && !isDie)
        {
            isDie = true;
            if(ingame_UI.isSettingPanel)
            {
                ingame_UI.OnOffSettingPanel_PlayerDie();  // ������ Ȱ��ȭ�Ǿ� �ִ� ����â�� ����
            }
            
            StartCoroutine(DoDie());
        }
    }

    private IEnumerator ChangeLayerTemporarily()
    {
        // ���̾ "PlayOnDamage"�� ����
        gameObject.layer = LayerMask.NameToLayer("PlayerOnDamage");
        isChangingLayer = true;

        // ���� �ð� �� Player ���̾�� �ٽ� ����
        yield return new WaitForSeconds(layerChangeDuration);

        // Player ���̾�� �ٽ� ����
        isDamaging = false;
        gameObject.layer = playerLayer;
        isChangingLayer = false;
    }

    public IEnumerator DoDie()
    {
        mmfPlayer_OnDie?.PlayFeedbacks();
        // �ڷ� �и��� �Ÿ��� �ð��� ����
        float pushBackDistance = 2.0f; // �ڷ� �и� �Ÿ�
        float pushBackTime = 0.8f; // �ڷ� �и��� �ð� (��)
        Vector3 currentPosition = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < pushBackTime)
        {
            Vector3 targetPosition = currentPosition - transform.forward * pushBackDistance;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, elapsedTime / pushBackTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return new WaitForSeconds(1.9f);

        ingame_UI.OnOffGameoverPanel();
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
            soundReload_In.Play();
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

    public void SetPlayerSound()
    {
        soundGun.volume = playerInformation.VolumeEffect;
        soundGun_Fail.volume = playerInformation.VolumeEffect;
        soundReload_In.volume = playerInformation.VolumeEffect;
        soundReload_Out.volume = playerInformation.VolumeEffect;
        soundMiniGun.volume = playerInformation.VolumeEffect;
        soundRoll.volume = playerInformation.VolumeEffect;
    }


    #region camLock
    public void CamLock() // ���콺 Ŀ���� ����� �Լ�
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void CamUnlock()
    {
        Cursor.visible = true; // ���콺 Ŀ���� ���̰� �մϴ�.
        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ���� ������ �����մϴ�.
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
