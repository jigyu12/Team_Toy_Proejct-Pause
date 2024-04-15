using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    public float fallSec = 0.3f, destroySec = 2f;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
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
