using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    public float fallSec = 0.3f, respawnSec = 2f;
    Rigidbody2D rb;
    Vector3 originalPosition;
    bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            isFalling = true;
            Invoke("FallPlatform", fallSec);
            Invoke("ResetPlatform", respawnSec + fallSec);
        }
    }

    void FallPlatform()
    {
        rb.isKinematic = false;
    }

    void ResetPlatform()
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        transform.position = originalPosition;
        isFalling = false;
    }
}
