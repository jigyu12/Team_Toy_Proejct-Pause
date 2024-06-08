using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "PlayerProjectile")
        {
            Destroy(gameObject);
        }
    }
}
