using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("# Game Control")]
    [SerializeField] private bool isLive;
    [Header("# Player Info")]
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private GameObject[] weaponInventory;
    [SerializeField] private int playerDamage;
    [SerializeField] private Sprite defaultWeaponImage; // 기본 무기 이미지

    public GameObject player;
    public int sceneNumberNext;
    bool isButtonSet;
    public bool isbgmPlayingSave;

    private Button playButton;
    private Button volumeButton;
    private Button volumeOffButton;
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

        isbgmPlayingSave = true;
        Init();
    }

    void Init()
    {
        isLive = false;
        maxHealth = 5;
        health = maxHealth;
        maxSpeed = 5;
        jumpPower = 9f;
        isButtonSet = false;
        sceneNumberNext = 1;
        playerDamage = 1;
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
            if(isbgmPlayingSave)
            {
                volumeButton = GameObject.Find("Main").transform.Find("Volume").GetComponent<Button>();
                volumeButton.gameObject.SetActive(true);

                volumeOffButton = GameObject.Find("Main").transform.Find("Volume-OFF").GetComponent<Button>();
                volumeOffButton.gameObject.SetActive(false);
            }
            else
            {
                AudioManager.Instance.PauseBgm(true);

                volumeButton = GameObject.Find("Main").transform.Find("Volume").transform.GetComponent<Button>();
                volumeButton.gameObject.SetActive(false);

                volumeOffButton = GameObject.Find("Main").transform.Find("Volume-OFF").GetComponent<Button>();
                volumeOffButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (isbgmPlayingSave)
            {
                GameObject soundButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("sound").gameObject;
                soundButton.SetActive(true);


                GameObject soundOffButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("sound off").gameObject;
                soundOffButton.SetActive(false);
            }
            else
            {
                AudioManager.Instance.PauseBgm(true);

                GameObject soundButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("sound").gameObject;
                soundButton.SetActive(false);


                GameObject soundOffButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("sound off").gameObject;
                soundOffButton.SetActive(true);
            }
        }

        if (scene.name == "MainTitle")
        {
            Init();

            playButton = GameObject.Find("Play").GetComponent<Button>();
            playButton.onClick.AddListener(RestartGame);

            volumeButton = GameObject.Find("Main").transform.Find("Volume").GetComponent<Button>();
            volumeButton.onClick.AddListener(() => SwitchBgmPause(AudioManager.Instance.IsBgmPlaying));

            volumeOffButton = GameObject.Find("Main").transform.Find("Volume-OFF").GetComponent<Button>();
            volumeOffButton.onClick.AddListener(() => SwitchBgmPause(AudioManager.Instance.IsBgmPlaying));

            exitButton = GameObject.Find("Exit").GetComponent<Button>();
            exitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            player = GameObject.Find("player");

            if(transform.childCount > 0)
            {
                GameObject inventoryImage = GameObject.Find("ui").transform.Find("inventory").Find("WeaponImage").gameObject;
                Debug.Log(transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name);
                inventoryImage.GetComponent<Image>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                inventoryImage.SetActive(true);
            }
            
            if (scene.name == "stage1")
            {
                isLive = false;
                maxHealth = 5;
                health = maxHealth;
                maxSpeed = 5;
                jumpPower = 9f;
                sceneNumberNext = 2;
                playerDamage = 1;
                if (transform.childCount > 0)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }

                // 초기 설정 시 기본 무기 이미지를 설정
                GameObject inventoryImage = GameObject.Find("ui").transform.Find("inventory").Find("WeaponImage").gameObject;
                inventoryImage.GetComponent<Image>().sprite = defaultWeaponImage;
                inventoryImage.SetActive(true);

                int savedBulletIndex = PlayerPrefs.GetInt("PlayerBulletIndex", 0);
                player.GetComponent<PlayerMove>().ChangeBullet(savedBulletIndex);

                GameObject uiH5 = GameObject.Find("ui").transform.Find("h5").gameObject;
                GameObject uiH5_Empty = GameObject.Find("ui").transform.Find("h5_empty").gameObject;
                uiH5.SetActive(true);
                uiH5_Empty.SetActive(false);


                GameObject uiH4 = GameObject.Find("ui").transform.Find("h4").gameObject;
                GameObject uiH4_Empty = GameObject.Find("ui").transform.Find("h4_empty").gameObject;
                uiH4.SetActive(true);
                uiH4_Empty.SetActive(false);


                GameObject uiH3 = GameObject.Find("ui").transform.Find("h3").gameObject;
                GameObject uiH3_Empty = GameObject.Find("ui").transform.Find("h3_empty").gameObject;
                uiH3.SetActive(true);
                uiH3_Empty.SetActive(false);


                GameObject uiH2 = GameObject.Find("ui").transform.Find("h2").gameObject;
                GameObject uiH2_Empty = GameObject.Find("ui").transform.Find("h2_empty").gameObject;
                uiH2.SetActive(true);
                uiH2_Empty.SetActive(false);

                GameObject uiH1 = GameObject.Find("ui").transform.Find("h1").gameObject;
                GameObject uiH1_Empty = GameObject.Find("ui").transform.Find("h1_empty").gameObject;
                uiH1.SetActive(true);
                uiH1_Empty.SetActive(false);

                GameObject.Find("GameClearDetect").transform.Find("GAME_CLEAR_ui").gameObject.SetActive(false);
                GameObject.Find("GameOverDetect").transform.Find("GAMEOVER_ui").gameObject.SetActive(false);

                if (!isButtonSet)
                {
                    isButtonSet = true;

                    // 특정 버튼의 remove할 entry를 안전하게 제거해준다.
                    /*
                    GameObject leftButton = GameObject.Find("ui").transform.Find("left").gameObject;

                    List<EventTrigger.Entry> entriesToRemove = new List<EventTrigger.Entry>();
                    foreach (var entry in leftButton.GetComponent<EventTrigger>().triggers)
                    {
                        //entry.callback.RemoveAllListeners();
                        entriesToRemove.Add(entry);
                    }
                    foreach (var entry in entriesToRemove)
                    {
                        leftButton.GetComponent<EventTrigger>().triggers.Remove(entry);
                    }
                    */

                    GameObject LeftButton = GameObject.Find("ui").transform.Find("left").gameObject;

                    EventTrigger.Entry entry_PointerDown_LeftButton = new EventTrigger.Entry();
                    entry_PointerDown_LeftButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_LeftButton.callback.AddListener((data) => player.GetComponent<PlayerMove>().buttonLeftMoveDown((PointerEventData)data));
                    LeftButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_LeftButton);

                    EventTrigger.Entry entry_PointerUp_LeftButton = new EventTrigger.Entry();
                    entry_PointerUp_LeftButton.eventID = EventTriggerType.PointerUp;
                    entry_PointerUp_LeftButton.callback.AddListener((data) => player.GetComponent<PlayerMove>().buttonLeftMoveUp((PointerEventData)data));
                    LeftButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerUp_LeftButton);


                    GameObject RightButton = GameObject.Find("ui").transform.Find("right").gameObject;

                    EventTrigger.Entry entry_PointerDown_RightButton = new EventTrigger.Entry();
                    entry_PointerDown_RightButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_RightButton.callback.AddListener((data) => player.GetComponent<PlayerMove>().buttonRightMoveDown((PointerEventData)data));
                    RightButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_RightButton);

                    EventTrigger.Entry entry_PointerUp_RightButton = new EventTrigger.Entry();
                    entry_PointerUp_RightButton.eventID = EventTriggerType.PointerUp;
                    entry_PointerUp_RightButton.callback.AddListener((data) => player.GetComponent<PlayerMove>().buttonRightMoveUp((PointerEventData)data));
                    RightButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerUp_RightButton);


                    GameObject JumpButton = GameObject.Find("ui").transform.Find("jump").gameObject;

                    EventTrigger.Entry entry_PointerDown_JumpButton = new EventTrigger.Entry();
                    entry_PointerDown_JumpButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_JumpButton.callback.AddListener((data) => player.GetComponent<PlayerMove>().buttonJump((PointerEventData)data));
                    JumpButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_JumpButton);


                    GameObject soundButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("sound").gameObject;

                    EventTrigger.Entry entry_PointerDown_soundButton = new EventTrigger.Entry();
                    entry_PointerDown_soundButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_soundButton.callback.AddListener((data) => SwitchBgmPause());
                    soundButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_soundButton);


                    GameObject soundOffButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("sound off").gameObject;

                    EventTrigger.Entry entry_PointerDown_soundOffButton = new EventTrigger.Entry();
                    entry_PointerDown_soundOffButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_soundOffButton.callback.AddListener((data) => SwitchBgmPause());
                    soundOffButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_soundOffButton);


                    GameObject mainButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("main").gameObject;

                    EventTrigger.Entry entry_PointerDown_mainButton = new EventTrigger.Entry();
                    entry_PointerDown_mainButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_mainButton.callback.AddListener((data) => GotoMain());
                    entry_PointerDown_mainButton.callback.AddListener((data) => ResumeGame());
                    mainButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_mainButton);


                    GameObject restartButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("restart").gameObject;

                    EventTrigger.Entry entry_PointerDown_restartButton = new EventTrigger.Entry();
                    entry_PointerDown_restartButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_restartButton.callback.AddListener((data) => RestartGameUISetting());
                    restartButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_restartButton);


                    GameObject settingButton = GameObject.Find("setting_button").gameObject;

                    EventTrigger.Entry entry_PointerDown_settingButton = new EventTrigger.Entry();
                    entry_PointerDown_settingButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_settingButton.callback.AddListener((data) => PauseGame());
                    settingButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_settingButton);


                    GameObject cancelButton = GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").transform.Find("Canvas").transform.Find("cancel").gameObject;

                    EventTrigger.Entry entry_PointerDown_cancelButton = new EventTrigger.Entry();
                    entry_PointerDown_cancelButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_cancelButton.callback.AddListener((data) => ResumeGame());
                    cancelButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_cancelButton);


                    GameObject gameClearMainButton = GameObject.Find("GameClearDetect").transform.Find("GAME_CLEAR_ui").transform.Find("main").gameObject;

                    EventTrigger.Entry entry_PointerDown_gameClearMainButton = new EventTrigger.Entry();
                    entry_PointerDown_gameClearMainButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_gameClearMainButton.callback.AddListener((data) => GotoMain());
                    entry_PointerDown_gameClearMainButton.callback.AddListener((data) => ResumeGame());
                    gameClearMainButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_gameClearMainButton);

                    GameObject gameOverRestartButton = GameObject.Find("GameOverDetect").transform.Find("GAMEOVER_ui").transform.Find("restart").gameObject;

                    EventTrigger.Entry entry_PointerDown_gameOverRestartButton = new EventTrigger.Entry();
                    entry_PointerDown_gameOverRestartButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_gameOverRestartButton.callback.AddListener((data) => RestartGame());
                    entry_PointerDown_gameOverRestartButton.callback.AddListener((data) => ResumeGame());
                    gameOverRestartButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_gameOverRestartButton);

                    GameObject clickButton = GameObject.Find("click").gameObject;

                    EventTrigger.Entry entry_PointerDown_clickButton = new EventTrigger.Entry();
                    entry_PointerDown_clickButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_clickButton.callback.AddListener((data) => InteractionWithPortal());
                    entry_PointerDown_clickButton.callback.AddListener((data) => InteractionWithWeapon());
                    clickButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_clickButton);


                    GameObject attackButton = GameObject.Find("attack").gameObject;

                    EventTrigger.Entry entry_PointerDown_attackButton = new EventTrigger.Entry();
                    entry_PointerDown_attackButton.eventID = EventTriggerType.PointerDown;
                    entry_PointerDown_attackButton.callback.AddListener((data) => PlayerAttackButtonDown());
                    attackButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerDown_attackButton);

                    EventTrigger.Entry entry_PointerUp_attackButton = new EventTrigger.Entry();
                    entry_PointerUp_attackButton.eventID = EventTriggerType.PointerUp;
                    entry_PointerUp_attackButton.callback.AddListener((data) => PlayerAttackButtonUp());
                    attackButton.GetComponent<EventTrigger>().triggers.Add(entry_PointerUp_attackButton);
                }
            }
            if(scene.name == "stage3")
            {
                player.GetComponent<PlayerMove>().anim.SetBool("isJumping", true);
            }
        }
    }

    void Update()
    {

        // left shift 키를 눌러 기능 검사 // 임시 ////////////////////////////////////////////

        if (Input.GetButtonDown("Fire3"))
        {
            
            
            LoadScene(sceneNumberNext);
            sceneNumberNext++;
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

    public void RestartGameUISetting()
    {
        LoadScene(1); // 제일 첫 스테이지의 빌드 번호를 넣으면 된다.
        Time.timeScale = 1;
        GameObject.Find("OptionPanelDetect").transform.Find("OptionPanel").gameObject.SetActive(false);
    }

    public void GotoMain()
    {
        LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void GameClear()
    {
        PauseGame();
        GameObject gameClearUI = GameObject.Find("GameClearDetect").transform.Find("GAME_CLEAR_ui").gameObject;
        gameClearUI.SetActive(true);
    }
    
    public void GameOver()
    {
        PauseGame();
        GameObject gameOverUI = GameObject.Find("GameOverDetect").transform.Find("GAMEOVER_ui").gameObject;
        gameOverUI.SetActive(true);
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
            isbgmPlayingSave = false;
            AudioManager.Instance.PauseBgm(true);
        }
        else
        {
            isbgmPlayingSave = true;
            AudioManager.Instance.PauseBgm(false);
        }
    }

    public void SwitchBgmPause()
    {
        if (AudioManager.Instance.IsBgmPlaying)
        {
            isbgmPlayingSave = false;
            AudioManager.Instance.PauseBgm(true);
        }
        else
        {
            isbgmPlayingSave = true;
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

    public int GetHp()
    {
        return health;
    }

    // 1 추가할거면 1, 1 줄일거면 -1을 파라미터로 넣으면 됨
    public void SetHp(int change)
    {
        if (health + change > maxHealth)
            return;

        health += change;
    }

    public void InteractionWithWeapon()
    {
        if (!player.GetComponent<PlayerMove>().isWeapon)
            return;

        Collider2D collision = player.GetComponent<PlayerMove>().weaponCollision;

        GameManager.Instance.SetWeaponInventory(collision);

        GameObject inventoryImage = GameObject.Find("ui").transform.Find("inventory").Find("WeaponImage").gameObject;
        inventoryImage.GetComponent<Image>().sprite = collision.transform.GetComponent<SpriteRenderer>().sprite;
        inventoryImage.SetActive(true);

        int newBulletIndex = collision.GetComponent<weapon>().weaponId;
        player.GetComponent<PlayerMove>().ChangeBullet(newBulletIndex);
    }

    public void InteractionWithPortal()
    {
        if (!player.GetComponent<PlayerMove>().isPortal)
            return;

        LoadScene(sceneNumberNext);
        sceneNumberNext++;
    }

    public void PlayerAttackButtonDown()
    {
        player.GetComponent<PlayerMove>().anim.SetBool("isShoot", true);
    }

    public void PlayerAttackButtonUp()
    {
        player.GetComponent<PlayerMove>().anim.SetBool("isShoot", false);
    }
}
