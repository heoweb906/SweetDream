using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    [Header("플레이어 정보")]
    public int hp = 4; // 플레이어 체력
    public int weaponNumber; // 1 = 빨강, 2 = 노랑, 3 = 파랑
    public float moveSpeed;  // 플레이어 스피드
    public float jumpForce; // 점프 힘
    public float rollSpeed = 10f; // 구르기 속도
    public float rollDuration = 0.5f; // 구르기 지속 시간
    public int attackDamage = 10;    // 공격 데미지
    [Space(10f)]


    [Header("오브젝트")]
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;
    [Space(10f)]



    [Header("조작 관련 변수")]
    public float hAxis; // 이동 시 수평 값을 위한 변수
    public float vAxis; // 이동 시 수직 값을 위한 변수
    public float turnSpeed; // 회전 속도
    public float attackRange = 50000.0f; // 공격 범위
    [Space(10f)]


    [Header("사운드")]
    public AudioSource soundGun; // 게임 배경음악
    public AudioSource soundRoll; // 타이밍 맞추는 매트로놈


    // #. 플레이어 키 입력
    private bool jDown; // 점프 키
    private bool wDown; // 웅크리기 키
    private bool shiftDown; // 구르기 키
    private bool rDown; // 재장전 키
    private int key_weapon = 1;


    private bool isJumping; // 점프 중인지 여부를 나타내는 변수
    private bool isRolling; // 구르고 있는 중인지 여부를 나타내는 변수

    public Rigidbody rigid;
    public Camera mainCamera;

    Vector3 moveVec; // 플레이어의 이동 값


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager를 찾아서 할당
        playerInformation = FindObjectOfType<PlayerInformation>();
        CamLock();
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        CamLock(); // 게임 시작 시 카메라 락
        WeaponChange_SceneChange(playerInformation.WeponColor); // 씬이 전환될 때 들고 있던 무기의 정보가 이어지도록 무기 교체 함수 1회 실행
    }


    private void Update()
    {
        GetInput();
        Move();
        Attack();
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


    private void GetInput()  // 입력을 받는 함수
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


    void Move() // 이동을 관리하는 함수
    {
        // 플레이어의 바라보는 방향을 이용하여 이동 벡터를 계산
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        moveVec = transform.forward * moveVec.z + transform.right * moveVec.x;

        // 이동 속도를 적용하고 웅크리기 여부에 따라 조절
        float currentMoveSpeed = moveSpeed * (wDown ? 0.3f : 1f);
        transform.position += moveVec * currentMoveSpeed * Time.deltaTime;

        // 점프 체크
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
        // 구르기 동안 이동 속도를 증가시키고, 방향은 현재 이동 방향으로 설정
        soundRoll.Play();
        float originalMoveSpeed = moveSpeed;
        moveSpeed = rollSpeed;
        Vector3 rollDirection = moveVec;

        // 일정 시간 동안 구르기
        yield return new WaitForSeconds(rollDuration);

        // 구르기 종료 후 원래 이동 속도와 방향으로 복구
        moveSpeed = originalMoveSpeed;
        isRolling = false;
    }


    private void Jump() // 점프
    {
        isJumping = true;
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    private void Attack()
    {
        if (Input.GetButtonDown("Fire1") && gameManager.rhythmCorrect && !(gameManager.isReload) && gameManager.bulletCount > 0)
        {
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit, attackRange);

            Debug.DrawRay(ray.origin, ray.direction * attackRange, hasHit ? Color.red : Color.green, 0.1f); // 레이 시각화

            if (hasHit && hit.collider.CompareTag("Monster"))
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                if (monster != null)
                {
                    if(monster.monsterColor == weaponNumber)
                    {
                        gameManager.bulletCount--;

                        //soundGun.Play();
                        monster.TakeDamage(attackDamage);
                    }
                    
                }
            }
        }
    }




    public void OnDamage(int dmg) // 데미지를 받았을 때의 함수, 몬스터들이 사용할 수 있도록 public으로 함
    {
        if(hp >= 1)
        {
            hp -= dmg;
            gameManager.ActivateHpImage(hp - 1);
        }
    }

    void WeaponChange(int number) 
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


    void WeaponChange_SceneChange(int number)    // 씬이 전환될 때 들고 있던 무기의 정보가 이어지도록 하기 위한 함수
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
    private void CamLock() // 마우스 커서를 숨기는 함수
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
