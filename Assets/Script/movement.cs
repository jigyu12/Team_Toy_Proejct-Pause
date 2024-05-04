using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float maxSpeed; // 값을 변경 후, 게임 매니져의 같은 값도 변경해야 함(함수 사용).
    float jumpPower; // 값을 변경 후, 게임 매니져의 같은 값도 변경해야 함(함수 사용).

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoxCollider2D ladderCollider; // 사다리 콜라이더를 저장할 변수

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if(GameObject.FindGameObjectWithTag("ladder"))
        {
            ladderCollider = GameObject.FindGameObjectWithTag("ladder").GetComponent<BoxCollider2D>();
        }
        /*
        ladderCollider = GameObject.FindGameObjectWithTag("ladder").GetComponent<BoxCollider2D>();
        if (ladderCollider == null)
        {
            Debug.LogError("사다리 콜라이더를 찾을 수 없습니다!");
        }
        */
    }

    void Start()
    {
        maxSpeed = GameManager.Instance.GetMaxSpeed();
        jumpPower = GameManager.Instance.GetJumpPower();
    }

    private void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping") )
        {
            
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                anim.SetBool("isJumping", true);
            
        }

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); // Right Max Speed
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y); // Left Max Speed

        // Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 1.5f)
                    anim.SetBool("isJumping", false);
                
            }
        }


        //사다리타기
        if (isLadder)
        {
            float ver = Input.GetAxis("Vertical");
            anim.speed = ver != 0 ? Mathf.Abs(ver) : Mathf.Abs(h);
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.x, ver * 2);




            anim.SetBool("isJumping", false);

        }
        else
        {
            rigid.gravityScale = 2;
        }
    }
    public bool isLadder;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ladder"))
        {
            isLadder = true;
            anim.SetBool("isLaddering", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ladder"))
        {
            isLadder = false;
            anim.SetBool("isLaddering", false);
        }
    }
}
