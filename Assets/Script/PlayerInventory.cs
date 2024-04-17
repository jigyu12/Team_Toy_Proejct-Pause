using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Rigidbody2D rigid;
    Collider2D col;

    public GameObject[] weaponInventory;
    public int playerDamage;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("weapon") && Input.GetButtonDown("Interaction"))
        {
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

            if (isfull)
            {
                // 플레이어가 지정한 번호 무기 버리고 교체
            }
            else
            {
                weaponInventory[index] = collision.gameObject;
                collision.gameObject.SetActive(false);
                playerDamage = collision.gameObject.GetComponent<weapon>().weaponDamage;
            }
                
        }
    }
}
