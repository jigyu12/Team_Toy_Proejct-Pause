using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHit : MonoBehaviour
{
    private Monster monster;
    void Start()
    {
        monster = GetComponentInParent<Monster>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile"))
        {
            int damage = GameManager.Instance.GetCurrentWeaponDamage();
            monster.TakeDamage(damage);
        }
    }
}
