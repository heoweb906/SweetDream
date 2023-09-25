using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static PlayerInformation instance;

    [Header("�÷��̾� ����")]
    [SerializeField]private bool isGame;  // �޴�ȭ������ ����ȭ������ �����ϴ� �뵵�� ���� bool ����
    [SerializeField]private bool isMenu;  // �� ���� �뵵�δ� ���� ������� �� ��

    [SerializeField]private int weponColor;  // �÷��̾��� ���� ���� ���� ����, �� ��ȯ or �ǹ��� ����Ǿ��� �� ������ ������ �����ϱ� ���ؼ�
    [SerializeField]private int jugde;  // ���� ���� �� �������� "���� ����" �����̸� �����ϴ� ���� - �̰ɷ� �÷��̾� ������ ���ڸ� ���� ����
    [SerializeField]private float mouseSpeed;  // ���콺 ���� ����
    [SerializeField]private float volume_BGM;  // ������� ũ�� ����
    [SerializeField]private float volume_Effect;  // ȿ���� ũ�� ����

    // ������Ƽ�� ���� �� �׼���
    public bool IsGame { get { return isGame; } set { isGame = value; } }
    public bool IsMenu { get { return isMenu; } set { isMenu = value; } }
    public int WeponColor { get { return weponColor; } set { weponColor = value; } }
    public int Jugde { get { return jugde; } set { jugde = value; }}
    public float MouseSpeed { get { return mouseSpeed; } set { mouseSpeed = value; }}
    public float VolumeBGM { get { return volume_BGM; } set { volume_BGM = value; }}
    public float VolumeEffect { get { return volume_Effect; } set { volume_Effect = value; }}

    // �̱��� �ν��Ͻ��� ������ �� �ִ� ������Ƽ
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

    // �ʿ��� �ʱ�ȭ�� �ٸ� ����� �߰��� �� �ֽ��ϴ�.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mouseSpeed = 0.5f;   // ���� ���콺 ������ 2f�� �� - ���� ����Ǵ� ������ 4�� ������ ����
            volume_BGM = 0.3f;
            volume_Effect = 0.3f;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
