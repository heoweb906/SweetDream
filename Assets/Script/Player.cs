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
    // 죽거나 메인화면으로 나갈 때 피버타임이 끝나도록 수정해야 함

    public GameManager gameManager;
    public PlayerInformation playerInformation;
    public UI_InGame ingame_UI;

    [Header("플레이어 정보")]
    public int hp = 4; // 플레이어 체력
    public int weaponNumber; // 1 = 빨강, 2 = 노랑, 3 = 파랑
    public float moveSpeed;  // 플레이어 스피드
    public float jumpForce; // 점프 힘
    public float rollSpeed = 40f; // 구르기 속도
    public float rollDuration = 0.5f; // 구르기 지속 시간
    public int attackDamage = 10;    // 공격 데미지

    public bool isDie;
    [Space(10f)]


    [Header("오브젝트")]
    public GameObject weapon_main;
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;
    public GameObject weapon4_Gatling;
    [Space(10f)]


    [Header("조작 관련 변수")]
    public float hAxis; // 이동 시 수평 값을 위한 변수
    public float vAxis; // 이동 시 수직 값을 위한 변수
    public float turnSpeed; // 회전 속도
    public float attackRange = 50000.0f; // 공격 범위

    private float timeSinceLastAttack = 0f;  // FeverAttack
    public float attackInterval = 0.5f; // everAttack Ray를 발사하는 주기
    [Space(10f)]


    [Header("사운드")]
    public AudioSource soundGun; // 총소리
    public AudioSource soundGun_Fail; // 리듬 맞추기 실패 공격
    public AudioSource soundReload_In; // 장전 소리 (탄창 장착)
    public AudioSource soundReload_Out; // 장전 소리 (탄창 장착)
    public AudioSource soundMiniGun; // 구르기 소리
    public AudioSource soundRoll; // 구르기 소리


    [Header("FEEL 관련")]
    public MMF_Player mmfPlayer_Camera;
    public MMF_Player mmfPlayer_OnDamage;
    public MMF_Player mmfPlayer_OnDie;
    // mmfPlayer_Camera?.PlayFeedbacks();




    // #. 플레이어 키 입력
    private bool jDown; // 점프 키
    private bool wDown; // 웅크리기 키
    private bool shiftDown; // 구르기 키
    private bool rDown; // 재장전 키
    private int key_weapon = 1;


    private bool isJumping; // 점프 중인지 여부를 나타내는 변수
    private bool isRolling; // 구르고 있는 중인지 여부를 나타내는 변수
    private bool isDamaging; // 공격을 받아 무적인 상태

    public Rigidbody rigid;
    public Camera mainCamera;



    // #. 애니메이터 관련
    public Animator gunAnimator;
    Vector3 moveVec; // 플레이어의 이동 값

    // #. 레이어 변경 관련
    private float layerChangeDuration = 1.5f;     // 레이어 변경 지속 시간 (초)
    private bool isChangingLayer = false;
    private int playerLayer; // Player의 초기 레이어


    // #. 레이 관련
    private Vector3 rayDirection; // 레이캐스트가 발사된 방향을 저장하는 변수



    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager를 찾아서 할당
        playerInformation = FindObjectOfType<PlayerInformation>();
        rigid = GetComponent<Rigidbody>();

        CamLock();
        playerLayer = gameObject.layer;
    }

    private void Start()
    {
        CamLock(); // 게임 시작 시 카메라 락
        SetPlayerSound(); // 환경 설정에 맞도록 효과음 사운드 조절; 
                          // 메뉴화면의 UI 컨트롤러는 플레이어의 함수를 실행시킬 수 없기 때문에 1번 실행시켜주는 거임


        if (!(gameManager.isFever))
        {
            WeaponChange_SceneChange(playerInformation.WeponColor); // 씬이 전환될 때 들고 있던 무기의 정보가 이어지도록 무기 교체 함수 1회 실행
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ingame_UI.OnOffSettingPanel();
        }

    }


    void Move() // 이동을 관리하는 함수
    {
        // 플레이어의 바라보는 방향을 이용하여 이동 벡터를 계산
        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // Rigidbody에 속도 적용
        Vector3 newVelocity = transform.forward * moveVec.z * moveSpeed + transform.right * moveVec.x * moveSpeed;
        newVelocity.y = rigid.velocity.y; // 현재 수직 속도 유지
        rigid.velocity = newVelocity;

        // 플레이어에게 강한 중력을 줌
        rigid.AddForce(Vector3.down * 15f, ForceMode.Acceleration);


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
        mmfPlayer_Camera?.PlayFeedbacks();

        float originalMoveSpeed = moveSpeed;
        moveSpeed = rollSpeed;
        Vector3 rollDirection = moveVec;

        // 일정 시간 동안 구르기
        yield return new WaitForSeconds(rollDuration);

        // 구르기 종료 후 원래 이동 속도와 방향으로 복구
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

                Debug.DrawRay(ray.origin, ray.direction * attackRange, hasHit ? Color.red : Color.green, 0.1f); // 레이 시각화
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
                            // 충돌 지점에서 나가는 방향 벡터를 사용
                            bullet.SetNewDirection(rayDirection);
                        }
                    }

                }
                gunAnimator.SetTrigger("Fire");
                soundGun.Play(); 
            }
        }
        else if(Input.GetButtonDown("Fire1") && !(gameManager.rhythmCorrect))   // 만약 틀린 타이밍에 공격하면
        {
            gameManager.ComboBarDown(20);
            soundGun_Fail.Play();
        }
    }

    private void FeverAttack() // 피버 공격 함수
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, attackRange);

        Debug.DrawRay(ray.origin, ray.direction * attackRange, hasHit ? Color.red : Color.green, 0.1f); // 레이 시각화
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




    public void OnDamage(int dmg) // 데미지를 받았을 때의 함수, 몬스터들이 사용할 수 있도록 public으로 함
    {
        if (hp >= 1 && !isDie && !isDamaging)
        {
            isDamaging = true;
            hp -= dmg;
            gameManager.ActivateHpImage(hp - 1);
            mmfPlayer_OnDamage?.PlayFeedbacks();

            // 레이어 변경이 진행 중이지 않을 때만 실행
            if (!isChangingLayer)
            {
                StartCoroutine(ChangeLayerTemporarily());
            }
        }


        // #. 죽음 함수 기능
        if(hp == 0 && !isDie)
        {
            isDie = true;
            if(ingame_UI.isSettingPanel)
            {
                ingame_UI.OnOffSettingPanel_PlayerDie();  // 죽으면 활성화되어 있는 설정창을 꺼줌
            }
            
            StartCoroutine(DoDie());
        }
    }

    private IEnumerator ChangeLayerTemporarily()
    {
        // 레이어를 "PlayOnDamage"로 변경
        gameObject.layer = LayerMask.NameToLayer("PlayerOnDamage");
        isChangingLayer = true;

        // 일정 시간 후 Player 레이어로 다시 변경
        yield return new WaitForSeconds(layerChangeDuration);

        // Player 레이어로 다시 변경
        isDamaging = false;
        gameObject.layer = playerLayer;
        isChangingLayer = false;
    }

    public IEnumerator DoDie()
    {
        mmfPlayer_OnDie?.PlayFeedbacks();
        // 뒤로 밀리는 거리와 시간을 설정
        float pushBackDistance = 2.0f; // 뒤로 밀릴 거리
        float pushBackTime = 0.8f; // 뒤로 밀리는 시간 (초)
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

    public void WeaponChange_SceneChange(int number)    // 씬이 전환될 때 들고 있던 무기의 정보가 이어지도록 하기 위한 함수
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
    public void CamLock() // 마우스 커서를 숨기는 함수
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void CamUnlock()
    {
        Cursor.visible = true; // 마우스 커서를 보이게 합니다.
        Cursor.lockState = CursorLockMode.None; // 마우스 커서의 제한을 해제합니다.
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
