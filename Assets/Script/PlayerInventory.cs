using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    GameObject[] weaponInventory;  // 값을 변경 후, 게임 매니져의 같은 값도 변경해야 함(함수 사용).
    int playerDamage; // 값을 변경 후, 게임 매니져의 같은 값도 변경해야 함(함수 사용).

    Rigidbody2D rigid;
    Collider2D col;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        weaponInventory = GameManager.Instance.GetWeaponInventory();
        playerDamage = GameManager.Instance.GetPlayerDamage();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("weapon") && Input.GetButtonDown("Interaction"))
        {
            GameManager.Instance.SetWeaponInventory(collision);
        }
    }
}
