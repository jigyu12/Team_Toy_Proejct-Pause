using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    Rigidbody2D rigid;
    Collider2D col;

    public int weaponDamage;
    public int weaponId;
    public int projectileId;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        
    }
}
