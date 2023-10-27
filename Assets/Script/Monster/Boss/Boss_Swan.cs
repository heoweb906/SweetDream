using DG.Tweening;
using DG.Tweening.Core.Easing;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// #. 기억!!!



//
// 보스 포인트 0에서 체력이 1/8로 깎여 색을 변경할 때 오류가 남
//






// 보스는 이동할 때를 제외하면 플레이어를 바라보지 않음, 6개의 각 포인트에 위치할 때 바라보는 각도를 수정해줘야 함
// 보스가 사용하는 공격 관련 기믹 함수는 이름을 Shoot으로 시작
// 모빌 생성 기믹에 떨어질 위치를 알려주지 않는 것은 백조의 날개짓 애니메이션으로 시전 한다는 걸 알려줄 것이기 때문이다.

public class Boss_Swan : MonoBehaviour
{
    private GameManager gameManager;
    private StageManager stagemanager;
    public Player player;

    private Coroutine currentCoroutine;

    [Header("테스트용 변수")]
    public GameObject[] patternSphere;

    [Header("몬스터 정보")]
    public int currentHealth; // 체력
    public int monsterColor; // 컬러 넘버 / 파랑 - 1 / 오렌지 - 1 / 보라 - 1
    public int damage = 1; // 부딥혔을 때 대미지
    public bool doDie;  // 죽었는지 살았는지

    [Header("색상 관련 변수들")]
    public new Renderer renderer; // 렌더러 컴포넌트
    public Color originalColor; // 원래 색상
    public Color newColor; // 변경하려는 색상
    private Material monsterMaterial; // 최초 실행 시 머테리얼
    public Material[] newMaterial; // 적용할 새로운 머티리얼

    public int takeDamage = 0;

    // #.깜박임 관련
    private Tweener colorTween;
    private bool isFlashing;

    [Header("보스 Position 관련")]
    public Transform position_FeildCenter;
    public Transform[] position_nest;
    public Transform position_Mouse; // 보스의 입의 위치, 탄환이 발사되는 장소
    [Space(15f)]

    [Header("보스 알고리즘")]
    public bool isBossStart; // 보스전 시작
    public bool isColorChanged; // 보스가 색을 변경하고 있는지
    public bool isPattern; // 하나의 행동을 취하고 있는지, 다음 행동을 넘어갈 때 사용할 거임
    public int nowPatternNum = 1;  // 지금 이동 관련 기믹을 쓰고 있는지, 공격 관련 기믹을 쓰고 있는지   / 1-이동 관련 기믹 / 2-공격 관련 기믹


    // 아직 미사용 중
    public bool isMoving; // 이동 중인 상태인지
    public bool isAttacking; // 공격 중인 상태인지
    public bool isGrogging; // 그로기 상태인지
    [Space(15f)]

    [Header("이동 관련")]
    public int nowpositionNum; // 현재 포지션 위치, 0이 1번 위치임
    public float moveSpeed; // 이동 속도
    [Space(15f)]

    [Header("돌진 관련")]
    public float rushSpeed;  // 돌진 속도
    public float slowdownDistance;  // 감속 시작 거리 - 제자리 돌아가기에서도 같이 쓰고 있음
    private float currentMoveSpeed; 
    [Space(15f)] 

    [Header("제자리 돌아가기 관련")]
    public float backHomeSpeed;  // 돌진 속도
    [Space(15f)]


    [Header("기본 공격 관련")]
    public GameObject bullet_Simple; // 기본 탄환
    public float bulletSpeed;
    [Space(15f)]

    [Header("모빌 패턴 관련")]
    public GameObject mobile;
    public float fieldRadius_mobile;  // 필드의 반지름
    public int mobileCnt;   // 생성할 모빌 개수
    [Space(15f)]

    [Header("즉사기 관련")]
    public GameObject diePlate;
    public GameObject diePlate_alrm;
    [Space(15f)]

    [Header("유도탄 관련")]
    public GameObject[] bullet_Color; // 색상 탄환 / 1-파랑 탄환 / 2-주황 탄환 / 3-보라 탄환
    public float bulletSpeed_Color;
    [Space(15f)]

    [Header("가시 장판 관련")]
    public GameObject thorn;
    public GameObject thorn_alrm;
    public float fieldRadius_thorn;  // 필드의 반지름
    public int thornCount;      // 생성할 가시 장판 개수
    [Space(15f)]

    [Header("UI 관련")]
    public Slider hpBar_Slider;

    


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); 
        stagemanager = FindObjectOfType<StageManager>();

        monsterMaterial = renderer.material;
        originalColor = monsterMaterial.GetColor("_BaseColor"); // 원래 색상 저장

        hpBar_Slider.value = 1600f; // 보스 체력 1600
    }

    private void Start()
    {
        // 게임 시작 시 최초로 위치한 장소
        transform.position = position_nest[0].position;
        nowpositionNum = 0;

        Invoke("BossStart",3f);
    }
    public void BossStart()
    {
        isBossStart = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeMaterial(0);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeMaterial(1);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeMaterial(2);
        }
    }

    private void FixedUpdate()
    {
        BoosAlgorithm();
    }


    // #. 보스 알고리즘
    private void BoosAlgorithm()
    {
        if (isBossStart)
        {
            if (!isPattern && nowPatternNum == 1 && !isColorChanged)
            {
                isPattern = true;
                nowPatternNum = 2;

                // 패턴 사용 확률 - 현재 모빌 패턴은 나오지 않도록 설정되어 있음
                int randomFunction = Random.Range(6, 101);
                if(randomFunction  <= 5) 
                {
                    Debug.Log("모빌 생성");
                    ShootMobile();
                }
                else if(6 <= randomFunction  && randomFunction <= 10)
                {
                    Debug.Log("즉사기 시작");
                    ShootDiePlate();
                }
                else if (11 <= randomFunction && randomFunction <= 60)
                {
                    Debug.Log("유도탄 생성");
                    ShootColorBullet();
                }
                else if (61 <= randomFunction && randomFunction <= 100)
                {
                    Debug.Log("가시 생성");
                    ShootThorn();
                }
            }

            if (!isPattern && nowPatternNum == 2 && !isColorChanged)
            {
                isPattern = true;
                nowPatternNum = 1;

                int randomFunction = Random.Range(1, 11);
                if (randomFunction <= 2)
                {
                    Debug.Log("돌진");
                    RushBoss();
                }
                else if(3 <= randomFunction)
                {
                    Debug.Log("이동");
                    MoveBoss();
                }
            }
        }
    }
    private void PatternOff()
    {
        isPattern = false;
        currentCoroutine = null;
    }


    // #. 데미지 받음 함수 , 절대 private로 하지 마라
    public void TakeDamage(int damageAmount)
    {
        if(!isColorChanged)
        {
            // hp 깎기
            currentHealth -= damageAmount * 20;
            takeDamage += damageAmount * 20;
            hpBar_Slider.value = (float)currentHealth;

            // 색상 변동
            if (renderer != null)
            {
                if (colorTween != null)
                {
                    colorTween.Kill();
                }
                colorTween = DOTween.To(() => renderer.material.GetColor("_BaseColor"),
                    x => renderer.material.SetColor("_BaseColor", x), newColor, 0.1f)
                    .OnComplete(() => ColorBack());

                isFlashing = true;
                StartCoroutine(FlashAfterColorChange());

                if (currentHealth <= 0)
                {
                    Invoke("Die", 3.0f);
                }
            }


            if (takeDamage >= 200)  // 체력이 일정 수준 이하가 되면
            {
                if(currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                isPattern = true;
                isColorChanged = true;
                BackNestBoss();
            }
        }   
    }
    private IEnumerator FlashAfterColorChange()
    {
        yield return new WaitForSeconds(0.1f); // 약간의 딜레이 (색상 변경 후 깜박임 시작)

        // 색상을 깜박이게 하려면 여기에서 원래 색상과 다른 색상으로 교체합니다.
        if (isFlashing)
        {
            renderer.material.color = Color.black;
        }
    }
    private void ColorBack()
    {
        if (monsterMaterial != null)
        {
            monsterMaterial.SetColor("_BaseColor", originalColor);
        }
        isFlashing = false; // 깜박임 중지
    }

    // #. 보스 색상 변동 함수
    private void ChangeMaterial(int index)
    {
        if (index == 0)
        {
            Debug.Log("파란색으로 바뀝니다.");
        }
        else if(index == 1)
        {
            Debug.Log("노란색으로 바뀝니다.");
        }
        else if (index == 2)
        {
            Debug.Log("보라색으로 바뀝니다.");
        }


        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null && newMaterial != null)
        {
            monsterColor = index + 1;
            renderer.material = newMaterial[index];
            monsterMaterial = renderer.material;
            originalColor = monsterMaterial.GetColor("_BaseColor"); // 원래 색상 저장
        }
    }
    // #. 죽음 함수
    private void Die()
    {
        stagemanager.MonsterCount--;

        Destroy(gameObject);
    }





    // #. 이동 함수
    private void MoveBoss()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // 이미 실행 중인 코루틴을 중지합니다.
        }
        currentCoroutine = StartCoroutine(MoveBoss_());
    }
    private IEnumerator MoveBoss_()
    {
        int startindex = nowpositionNum;
        int endindex;
        do
        {
            endindex = Random.Range(0, 6);
        } while (endindex == nowpositionNum);

        Debug.Log(startindex + "에서" + endindex + "로 이동");

        Vector3 center = position_FeildCenter.position;
        Vector3 startPoint = position_nest[startindex].position - center;
        
        float assistNum = AsistNum_MoveBoss(startindex, endindex);

        float journeyDuration = 1.5f * Mathf.Abs(assistNum); // 이동에 걸리는 시간을 조절할 수 있습니다.
        float anglehow = 60 * assistNum; // 몇도가 이동할지 구할 수 있습니다. 

        if(assistNum > 0)
        {
            RotateBossDegrees(-90f, 1.6f); // 90도 회전
        }
        else
        {
            RotateBossDegrees(90f, 1.6f); // 90도 회전
        }

        yield return new WaitForSeconds(1.7f);

        float startTime = Time.time;
        float elapsedTime = 0f;

        while (Time.time < startTime + journeyDuration) // 이동 하는데 얼마의 시간이 걸릴지
        {
            float journeyFraction = (Time.time - startTime) / journeyDuration;

            // 몇도를 이동하는지
            float angle = Mathf.Lerp(0, anglehow, journeyFraction);
            Vector3 newPosition = center + Quaternion.Euler(0, angle, 0) * startPoint;

            // 연구실 선배님의 도움
            Vector3 dir = newPosition - transform.position;
            dir.Normalize();
            transform.forward = dir;

            transform.position = newPosition;

            // 이동 중에 0.5초마다 ShootBullet 함수 호출
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 0.5f)
            {
                ShootBullet();
                elapsedTime = 0f;
            }

            yield return null;
        }

        Debug.Log(startindex + "에서"+ endindex+ "로 이동");

        transform.position = position_nest[endindex].position;
        nowpositionNum = endindex;


        LookAtCenterWithTween();

        yield return new WaitForSeconds(3.0f);
        Invoke("PatternOff", 3f);
        yield return new WaitForSeconds(3.0f);
    }
    int AsistNum_MoveBoss(int startindex, int endindex)
    {
        bool b_dir = (Random.Range(0, 2) == 0); // true면 시계 방향으로 이동, false면 반시계 방향으로 이동

        if (startindex == 0)
        {
            if (endindex == 1)
            {
                return b_dir ? 1 : -5;
            }
            else if(endindex == 2)
            {
                return b_dir ? 2 : -4;
            }
            else if (endindex == 3)
            {
                return b_dir ? 3 : -3;
            }
            else if (endindex == 4)
            {
                return b_dir ? 4 : -2;
            }
            else if (endindex == 5)
            {
                return b_dir ? 5 : -1;
            }
        }
        else if (startindex == 1)
        {
            if (endindex == 0)
            {
                return b_dir ? 5 : -1;
            }
            else if (endindex == 2)
            {
                return b_dir ? 1 : -5;
            }
            else if (endindex == 3)
            {
                return b_dir ? 2 : -4;
            }
            else if (endindex == 4)
            {
                return b_dir ? 3 : -3;
            }
            else if (endindex == 5)
            {
                return b_dir ? 4 : -2;
            }
        }
        else if (startindex == 2)
        {
            if (endindex == 0)
            {
                return b_dir ? 4 : -2;
            }
            else if (endindex == 1)
            {
                return b_dir ? 5 : -1;
            }
            else if (endindex == 3)
            {
                return b_dir ? 1 : -5;
            }
            else if (endindex == 4)
            {
                return b_dir ? 2 : -4;
            }
            else if (endindex == 5)
            {
                return b_dir ? 3 : -3;
            }
        }
        else if (startindex == 3)
        {
            if (endindex == 0)
            {
                return b_dir ? 3 : -3;
            }
            else if (endindex == 1)
            {
                return b_dir ? 4 : -2;
            }
            else if (endindex == 2)
            {
                return b_dir ? 5 : -1;
            }
            else if (endindex == 4)
            {
                return b_dir ? 1 : -5;
            }
            else if (endindex == 5)
            {
                return b_dir ? 2 : -4;
            }
        }
        else if (startindex == 4)
        {
            if (endindex == 0)
            {
                return b_dir ? 2 : -4;
            }
            else if (endindex == 1)
            {
                return b_dir ? 3 : -3;
            }
            else if (endindex == 2)
            {
                return b_dir ? 4 : -2;
            }
            else if (endindex == 3)
            {
                return b_dir ? 5 : -1;
            }
            else if (endindex == 5)
            {
                return b_dir ? 1 : -5;
            }
        }
        else if (startindex == 5)
        {
            if (endindex == 0)
            {
                return b_dir ? 1 : -5;
            }
            else if (endindex == 1)
            {
                return b_dir ? 2 : -4;
            }
            else if (endindex == 2)
            {
                return b_dir ? 3 : -3;
            }
            else if (endindex == 3)
            {
                return b_dir ? 4 : -2;
            }
            else if (endindex == 4)
            {
                return b_dir ? 5 : -1;
            }
        }

        return 0;
    }

    // #. 돌진 함수
    private void RushBoss()
    {
        currentCoroutine = StartCoroutine(RushBoss_());
    }
    private IEnumerator RushBoss_()
    {
        int targetIndex = AsistNum_RushBoss(nowpositionNum);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = position_nest[targetIndex].position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (transform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * currentMoveSpeed;
            float journeyFraction = distanceCovered / journeyLength;

            // 서서히 느려지도록 가중치 조절
            float slowdownFactor = Mathf.Clamp01(journeyFraction / (journeyLength / slowdownDistance));
            currentMoveSpeed = Mathf.Lerp(rushSpeed, 0, slowdownFactor);

            transform.position = Vector3.Lerp(startPosition, targetPosition, journeyFraction);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        transform.position = position_nest[targetIndex].position;
        nowpositionNum = targetIndex;

        yield return new WaitForSeconds(0.5f);
        LookAtCenterWithTween();

        yield return new WaitForSeconds(3.0f);
        Invoke("PatternOff", 3f);
        yield return new WaitForSeconds(3.0f);
    }
    int AsistNum_RushBoss(int nowpositionNum)
    {
        if(nowpositionNum == 0)
        {
            return 3;
        }
        else if(nowpositionNum == 1)
        {
            return 4;
        }
        else if (nowpositionNum == 2)
        {
            return 5;
        }
        else if (nowpositionNum == 3)
        {
            return 0;
        }
        else if (nowpositionNum == 4)
        {
            return 1;
        }
        else if (nowpositionNum == 5)
        {
            return 2;
        }
        return 0;
    }

    // #. 제자리 돌아가기 함수
    private void BackNestBoss()
    {
        currentCoroutine = StartCoroutine(BackNestBoss_());
    }
    private IEnumerator BackNestBoss_()
    {
        Debug.Log("집으로 돌아갑니다");
        transform.DOLookAt(position_nest[0].position, 2f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(2.0f);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = position_nest[0].position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (transform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * currentMoveSpeed;
            float journeyFraction = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, targetPosition, journeyFraction);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        transform.position = position_nest[0].position;
        nowpositionNum = 0;
        LookAtCenterWithTween();


        yield return new WaitForSeconds(2.5f);

        // 랜덤 색상 변동
        int randomFunction = Random.Range(0,3);
        ChangeMaterial(randomFunction);

        yield return new WaitForSeconds(3.0f);
        Debug.Log("색상 변경을 완료합니다");
        isColorChanged = false;
        Invoke("PatternOff", 3f);
    }




    // #. 기본 공격
    private void ShootBullet()
    {
        if (player != null && bullet_Simple != null && position_Mouse != null)
        {
            Vector3 bossMouthPosition = position_Mouse.position;
            Vector3 playerPosition = player.transform.position;

            // 방향 벡터 계산
            Vector3 direction = (playerPosition - bossMouthPosition).normalized;

            // 각도 간격 설정
            float angleInterval = 20f;

            // 생성할 총알 개수
            for (int i = -2; i <= 2; i++)
            {
                // 현재 각도 계산
                float currentAngle = i * angleInterval;

                GameObject newBullet = Instantiate(bullet_Simple, bossMouthPosition, Quaternion.identity);
                Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
                if (bulletRigidbody != null)
                {
                    Vector3 bulletVelocity = Quaternion.Euler(0, currentAngle, 0) * direction;
                    bulletRigidbody.velocity = bulletVelocity * bulletSpeed;
                }
            }
        }
    }




    // #. 모빌 떨어뜨리기 공격 - 1
    private void ShootMobile()
    {
        currentCoroutine = StartCoroutine(ShootMobile_());
    }
    private IEnumerator ShootMobile_()
    {
        Vector3[] thornPositions = new Vector3[mobileCnt]; // 가시 장판 위치를 저장하는 배열

        for (int i = 0; i < mobileCnt; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float radianAngle = randomAngle * Mathf.Deg2Rad;

            // 원 안의 랜덤한 위치 계산 (0부터 반지름까지)
            float randomRadius = Random.Range(0f, fieldRadius_mobile);
            Vector3 spawnPosition = position_FeildCenter.position + new Vector3(Mathf.Cos(radianAngle), 0, Mathf.Sin(radianAngle)) * randomRadius;

            thornPositions[i] = spawnPosition; // 위치를 배열에 저장
        }

        // 가시 생성 알림
        for (int i = 0; i < mobileCnt; i++)
        {
            Vector3 originalPosition = thornPositions[i];
            originalPosition.y = 60.0f; // 원하는 높이로 조정
            GameObject mobile_ = Instantiate(mobile, originalPosition, Quaternion.identity);
        }

        yield return new WaitForSeconds(3.0f);
        Invoke("PatternOff", 3f);
    }

    // #. 즉사기 패턴 - 2
    private void ShootDiePlate()
    {
        currentCoroutine = StartCoroutine(SpawnDiePlate());
    }
    private IEnumerator SpawnDiePlate()
    {
        float randomAngle = Random.Range(0f, 360f);
        float radianAngle = randomAngle * Mathf.Deg2Rad;

        // 원 안의 랜덤한 위치 계산 (0부터 반지름까지)
        float randomRadius = Random.Range(0f, fieldRadius_thorn);
        Vector3 spawnPosition = position_FeildCenter.position + new Vector3(Mathf.Cos(radianAngle), 0, Mathf.Sin(radianAngle)) * randomRadius;

        GameObject diePlate_alrm_ = Instantiate(diePlate_alrm, spawnPosition, Quaternion.identity);


        yield return new WaitForSeconds(4.0f);  // 2초 대기
        diePlate.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        diePlate.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        diePlate.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        diePlate.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        diePlate.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        diePlate.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        diePlate_alrm_.SetActive(false);

       
        yield return new WaitForSeconds(3.0f);
        Invoke("PatternOff", 3f);

    }

    // #. 유도탄 공격 - 3
    private void ShootColorBullet()
    {
        currentCoroutine = StartCoroutine(ShootColorBullet_());
    }
    private IEnumerator ShootColorBullet_()
    {
        if (bullet_Color != null && position_Mouse != null)
        {
            Vector3 bossMouthPosition = position_Mouse.position;

            // 부채꼴 각도 설정
            float angleInterval = 40f;

            for (int i = -1; i <= 1; i++)
            {
                int randomNum = Random.Range(0, 3); // 랜덤 총알 선택
                float currentAngle = i * angleInterval;

                GameObject newBullet = Instantiate(bullet_Color[randomNum], bossMouthPosition, Quaternion.identity);
                Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();

                if (bulletRigidbody != null)
                {
                    Vector3 direction = Quaternion.Euler(0, currentAngle, 1) * new Vector3(0, 1, -1f);
                    bulletRigidbody.velocity = direction * bulletSpeed_Color;
                }
            }

            yield return new WaitForSeconds(3.0f);
            Invoke("PatternOff", 3f);
        }
    }

    // #. 가시 장판 공격 - 4
    private void ShootThorn()
    {
        // 진짜 가시를 생성하는 코루틴 호출
        currentCoroutine = StartCoroutine(SpawnThorns());
    }
    IEnumerator SpawnThorns()
    {
        Vector3[] thornPositions = new Vector3[thornCount]; // 가시 장판 위치를 저장하는 배열

        for (int i = 0; i < thornCount; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float radianAngle = randomAngle * Mathf.Deg2Rad;

            // 원 안의 랜덤한 위치 계산 (0부터 반지름까지)
            float randomRadius = Random.Range(0f, fieldRadius_thorn);
            Vector3 spawnPosition = position_FeildCenter.position + new Vector3(Mathf.Cos(radianAngle), 0, Mathf.Sin(radianAngle)) * randomRadius;

            thornPositions[i] = spawnPosition; // 위치를 배열에 저장
        }

        // 가시 생성 알림
        for (int i = 0; i < thornCount; i++)
        {
            GameObject thorn_alrm_1 = Instantiate(thorn_alrm, thornPositions[i], Quaternion.identity);

            // 여기에서 1번 가시 장판 오브젝트에 추가한 스크립트를 호출하거나 설정할 수 있습니다.
        }

        yield return new WaitForSeconds(1.5f);  // 2초 대기

        // 가시 생성
        for (int i = 0; i < thornCount; i++)
        {
            // 원래 위치 가져오기
            Vector3 originalPosition = thornPositions[i];
            originalPosition.y = -8.0f; // 원하는 높이로 조정
            GameObject thorn_ = Instantiate(thorn, originalPosition, Quaternion.identity);
        }

        
        yield return new WaitForSeconds(3.0f);
        Invoke("PatternOff", 3f);
    }



    // 맵 중앙을 보도록 하는 함수
    private void LookAtCenterWithTween()
    {
        transform.DOLookAt(position_FeildCenter.position, 2f).SetEase(Ease.InOutQuad);
    }
    // 원하는 각도만큼 매개변수 - 회전각도, 회전속도
    private void RotateBossDegrees(float angle, float speedAngle)
    {
        Vector3 currentRotation = transform.eulerAngles + new Vector3(0, angle, 0);
        transform.DOLocalRotate(currentRotation, speedAngle, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
    }





    // #. 패턴 확인용으로 어떤 패턴을 쓰고 있는지 확인하기 위한 함수임
    // 구체 1개 - 공격관련     구체 2개 - 이동 관련      구체 3개 - 색상 변동
    private void PatternCheckSphere(int index)
    {
        for (int i = 0; i < patternSphere.Length; i++)
        {
            if (i <= index)
            {
                // 인덱스까지 활성화
                patternSphere[i].SetActive(true);
            }
            else
            {
                // 나머지는 비활성화
                patternSphere[i].SetActive(false);
            }
        }
    }



}