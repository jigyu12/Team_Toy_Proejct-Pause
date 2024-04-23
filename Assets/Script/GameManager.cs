using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("# Game Control")]
    public bool isLive;
    [Header("# Player Info")]
    public float health;
    public float maxHealth;
    public PlayerMove p_move;
    public PlayerInventory p_inventory;

    private static GameManager instance;

    void Awake()
    {
        Application.targetFrameRate = 60;

        isLive = true;
        maxHealth = 100;
        health = maxHealth;

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        

        // e 키를 눌러 기능 검사
        /*
        if(Input.GetButtonDown("Interaction"))
        {
            LoadScene("TestScene2");
        }
        */
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
        LoadScene("stage1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
