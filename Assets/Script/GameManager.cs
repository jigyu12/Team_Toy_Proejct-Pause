using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("# Game Control")]
    [SerializeField] private bool isLive;
    [Header("# Player Info")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private GameObject[] weaponInventory;
    [SerializeField] private int playerDamage;

    public GameObject player;
    public int sceneNumberTemp; // 임시 ////////////////////////////////////////////

    private Button playButton;
    private Button volumeButton;
    private Button exitButton;

    private static GameManager instance;

    void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        Init();
    }

    void Init()
    {
        isLive = false;
        maxHealth = 100;
        health = maxHealth;
        maxSpeed = 5;
        jumpPower = 13;
        sceneNumberTemp = 1; // 임시 ////////////////////////////////////////////
    }

    // 씬이 로드 될때 마다 델리게이트 체인으로 걸어놓은 함수들이 실행된다.
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainTitle")
        {
            Init();

            playButton = GameObject.Find("Play").GetComponent<Button>();
            playButton.onClick.AddListener(RestartGame);

            volumeButton = GameObject.Find("Volume").GetComponent<Button>();
            volumeButton.onClick.AddListener(() => SwitchBgmPause(AudioManager.Instance.IsBgmPlaying));

            exitButton = GameObject.Find("Exit").GetComponent<Button>();
            exitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            player = GameObject.Find("player");
        }
    }

    void Update()
    {


        // left alt 키를 눌러 기능 검사

        if (Input.GetButtonDown("Fire2"))
        {
            LoadScene(0);
        }

        // left shift 키를 눌러 기능 검사 // 임시 ////////////////////////////////////////////

        if (Input.GetButtonDown("Fire3"))
        {
            sceneNumberTemp++;
            LoadScene(sceneNumberTemp);
        }

    }

    public static GameManager Instance
    {
        get { return instance; }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        LoadScene(1); // 제일 첫 스테이지의 빌드 번호를 넣으면 된다.
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void LoadScene(int SceneBuildIndex)
    {
        SceneManager.LoadScene(SceneBuildIndex);
        AudioManager.Instance.LoadStageBgmClip(SceneBuildIndex);
    }

    public void SwitchBgmPause(bool bgmIsPlaying)
    {
        if(bgmIsPlaying)
        {
            AudioManager.Instance.PauseBgm(true);
        }
        else
        {
            AudioManager.Instance.PauseBgm(false);
        }
    }

    public int GetPlayerDamage()
    {
        return playerDamage;
    }

    public void SetPlayerDamage(int damage)
    {
        if (damage < 0)
            return;

        playerDamage = damage;
    }

    public GameObject[] GetWeaponInventory()
    {
        return weaponInventory;
    }

    public void SetWeaponInventory(Collider2D collision)
    {
        if (collision == null)
            return;

        bool isfull = true;
        int index = -1;
        for (int i = 0; i < weaponInventory.Length; i++)
        {
            if (weaponInventory[i] == null)
            {
                index = i;
                isfull = false;
                break;
            }
        }

        if (isfull) // 인벤토리 한 칸이라 가정한 임시 코드
        {
            collision.gameObject.SetActive(false);
            collision.transform.parent = transform;
            Destroy(weaponInventory[0]);
            weaponInventory[0] = collision.gameObject;
            playerDamage = collision.gameObject.GetComponent<weapon>().weaponDamage;
            GameManager.Instance.SetPlayerDamage(playerDamage);
        }
        else
        {
            collision.gameObject.SetActive(false);
            collision.transform.parent = transform;
            weaponInventory[index] = collision.gameObject;
            playerDamage = collision.gameObject.GetComponent<weapon>().weaponDamage;
            GameManager.Instance.SetPlayerDamage(playerDamage);
        }
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public void SetMaxSpeed(float speed)
    {
        if (speed < 0f)
            return;

        maxSpeed = speed;
    }

    public float GetJumpPower()
    {
        return jumpPower;
    }

    public void SetJumpPower(float jpower)
    {
        if (jpower < 0f)
            return;

        jumpPower = jpower;
    }
}
