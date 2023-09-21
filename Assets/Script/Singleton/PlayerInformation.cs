using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static PlayerInformation instance;

    [Header("플레이어 정보")]
    [SerializeField]private bool isGame;  // 메뉴화면인지 게임화면인지 구분하는 용도로 만든 bool 값임
    [SerializeField]private bool isMenu;  // 그 외의 용도로는 절대 사용하지 말 것

    [SerializeField]private int weponColor;  // 플레이어의 무기 색상 정보 저장, 씬 전환 or 피버가 종료되었을 때 무기의 정보를 유지하기 위해서
    [SerializeField] private int jugde;  // 게임 시작 시 판정선의 "최초 생성" 딜레이를 결정하는 변수 - 이걸로 플레이어 개인의 박자를 맞출 거임


    // 프로퍼티를 통한 값 액세스
    public bool IsGame { get { return isGame; } set { isGame = value; } }
    public bool IsMenu { get { return isMenu; } set { isMenu = value; } }
    public int WeponColor { get { return weponColor; } set { weponColor = value; } }
    public int Jugde { get { return jugde; } set { jugde = value; }}

    // 싱글톤 인스턴스에 접근할 수 있는 프로퍼티
    public static PlayerInformation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInformation>();

                if (instance == null)
                {
                    GameObject singleton = new GameObject("PlayerInformation");
                    instance = singleton.AddComponent<PlayerInformation>();
                }
            }
            return instance;
        }
    }

    // 필요한 초기화나 다른 기능을 추가할 수 있습니다.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
