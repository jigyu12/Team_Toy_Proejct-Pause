using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    [SerializeField] float fallSec = 0.5f, destroySec = 2f;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void onCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("player"))
        {
            Invoke("FallPlatform", fallSec);
            Destroy(gameObject, destroySec);
        }
    }

    void FallPlatform()
    {
        rb.isKinematic = false;
    }
}
