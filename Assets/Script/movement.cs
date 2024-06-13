using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    float maxSpeed; // 값을 변경 후, 게임 매니져의 같은 값도 변경해야 함(함수 사용).
    float jumpPower; // 값을 변경 후, 게임 매니져의 같은 값도 변경해야 함(함수 사용).
    
    bool isLeftMoveClick;
    bool isRightMoveClick;

    bool isHurt;

    public bool isWeapon;
    public Collider2D weaponCollision;

    public bool isPortal;


    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public Animator anim;
    CapsuleCollider2D ladderCollider; // 사다리 콜라이더를 저장할 변수

    public Transform genPoint;
    public GameObject[] Bullet;
    private int currentBulletIndex = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        isLeftMoveClick = false;
        isRightMoveClick = false;

        isHurt = false;

        isWeapon = false;
        isPortal = false;

        if (GameObject.FindGameObjectWithTag("ladder"))
        {
            ladderCollider = GameObject.FindGameObjectWithTag("ladder").GetComponent<CapsuleCollider2D>();
        }
        /*
        ladderCollider = GameObject.FindGameObjectWithTag("ladder").GetComponent<CapsuleCollider2D>();
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

        int savedBulletIndex = PlayerPrefs.GetInt("PlayerBulletIndex", 0);
        ChangeBullet(savedBulletIndex);
    }

    private void Update()
    {
        if (Time.deltaTime == 0 || isHurt)
            return;

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

        if (isLeftMoveClick && !isRightMoveClick)
        {
            spriteRenderer.flipX = true;
        }
        else if (!isLeftMoveClick && isRightMoveClick)
        {
            spriteRenderer.flipX = false;
        }

        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);


        if (Input.GetButtonDown("Fire1")) // Ctrl
        {
            anim.SetBool("isShoot", true);
        }

        else if (Input.GetButtonUp("Fire1"))
        {
            anim.SetBool("isShoot", false);
        }

        if (spriteRenderer.flipX) // 총알 생성 위치 조정
        {
            genPoint.localPosition = new Vector2(-Mathf.Abs(genPoint.localPosition.x), genPoint.localPosition.y);
        }

        else if (!spriteRenderer.flipX)
        {
            genPoint.localPosition = new Vector2(Mathf.Abs(genPoint.localPosition.x), genPoint.localPosition.y);
        }
    }

    void FixedUpdate()
    {
        if (isHurt)
            return;

        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); // Right Max Speed
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y); // Left Max Speed

        // Move By Button Control
        if(isLeftMoveClick &&  !isRightMoveClick)
        {
            rigid.AddForce(-Vector2.right, ForceMode2D.Impulse);
        }
        else if(!isLeftMoveClick && isRightMoveClick)
        {
            rigid.AddForce(Vector2.right, ForceMode2D.Impulse);
        }

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

        if ((collision.CompareTag("Monster") || collision.CompareTag("MonsterProjectile") || collision.CompareTag("MonsterAttack")) && !isHurt)
        {
            StartCoroutine(PlayerHurt());
        }

        if (collision.tag == "weapon")
        {
            isWeapon = true;
            weaponCollision = collision;
        }
        if (collision.tag == "portal")
        {
            isPortal = true;
        }
        if (collision.tag == "FallDetectWall")
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "stage1")
            {
                transform.position = new Vector3(31.81f, 20.67f, 0.4607419f);
            }
            else if (scene.name == "stage2")
            {
                transform.position = new Vector3(94.49f, -57.16f, 0.4607419f);
            }
            else if (scene.name == "stage3")
            {
                transform.position = new Vector3(12.80354f, -67.41419f, 0.4607419f);
            }
            else if(scene.name == "stage4")
            {
                transform.position = new Vector3(-23.51f, -41.22f, 0f);
            }

            if (collision.CompareTag("MonsterHitBox"))
                StartCoroutine(PlayerHurt());
        }
    }

    IEnumerator PlayerHurt()
    {
        isHurt = true;
        GameManager.Instance.SetHp(-1);
        anim.SetTrigger("isHurt");

        if(GameManager.Instance.GetHp() == 4) 
        {
            GameObject uiH5 = GameObject.Find("ui").transform.Find("h5").gameObject;
            GameObject uiH5_Empty = GameObject.Find("ui").transform.Find("h5_empty").gameObject;
            uiH5.SetActive(false);
            uiH5_Empty.SetActive(true);
        }
        else if (GameManager.Instance.GetHp() == 3)
        {
            GameObject uiH4 = GameObject.Find("ui").transform.Find("h4").gameObject;
            GameObject uiH4_Empty = GameObject.Find("ui").transform.Find("h4_empty").gameObject;
            uiH4.SetActive(false);
            uiH4_Empty.SetActive(true);
        }
        else if (GameManager.Instance.GetHp() == 2)
        {
            GameObject uiH3 = GameObject.Find("ui").transform.Find("h3").gameObject;
            GameObject uiH3_Empty = GameObject.Find("ui").transform.Find("h3_empty").gameObject;
            uiH3.SetActive(false);
            uiH3_Empty.SetActive(true);
        }
        else if (GameManager.Instance.GetHp() == 1)
        {
            GameObject uiH2 = GameObject.Find("ui").transform.Find("h2").gameObject;
            GameObject uiH2_Empty = GameObject.Find("ui").transform.Find("h2_empty").gameObject;
            uiH2.SetActive(false);
            uiH2_Empty.SetActive(true);
        }
        else if (GameManager.Instance.GetHp() == 0)
        {
            GameObject uiH1 = GameObject.Find("ui").transform.Find("h1").gameObject;
            GameObject uiH1_Empty = GameObject.Find("ui").transform.Find("h1_empty").gameObject;
            uiH1.SetActive(false);
            uiH1_Empty.SetActive(true);

            if(!spriteRenderer.flipX)
            {
                transform.Translate(0, -0.5f, 0);
                transform.rotation = Quaternion.Euler(0,0,90);
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 360);
                
            }
            else
            {
                transform.Translate(0, -0.5f, 0);
                transform.rotation = Quaternion.Euler(0, 0, -90);
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, -360);
            }

            GameManager.Instance.GameOver();
        }

        yield return new WaitForSeconds(0.5f);

        isHurt = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ladder"))
        {
            isLadder = false;
            anim.SetBool("isLaddering", false);
        }

        if (collision.tag == "weapon")
        {
            isWeapon = false;
            weaponCollision = null;
        }
        if (collision.tag == "portal")
        {
            isPortal = false;
        }
    }

    public void buttonJump(PointerEventData data)
    {
        if (!anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
    }

    public void buttonLeftMoveDown(PointerEventData data)
    {
        isLeftMoveClick = true;
    }

    public void buttonLeftMoveUp(PointerEventData data)
    {
        isLeftMoveClick = false;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
    }

    public void buttonRightMoveDown(PointerEventData data)
    {
        isRightMoveClick = true;
    }

    public void buttonRightMoveUp(PointerEventData data)
    {
        isRightMoveClick = false;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
    }

    void shoot()
    {
        GameObject bulletClone = Instantiate(Bullet[currentBulletIndex], genPoint.position, genPoint.rotation);

        if (bulletClone != null)
        {
            Rigidbody2D bulletRigidbody = bulletClone.GetComponent<Rigidbody2D>();
            if (bulletRigidbody != null)
            {
                float direction = spriteRenderer.flipX ? -1 : 1;
                bulletRigidbody.velocity = new Vector2((transform.right.x * direction * 5f) + rigid.velocity.x, 0);
                Collider2D bulletCollider = bulletClone.GetComponent<Collider2D>();
                if (bulletCollider != null)
                {
                    Physics2D.IgnoreCollision(bulletCollider, GetComponent<Collider2D>());
                }
                SpriteRenderer bulletSpriteRenderer = bulletClone.GetComponent<SpriteRenderer>();

                /*
            if (spriteRenderer.flipX) // 총알 스프라이트 반전
            {
                bulletSpriteRenderer.flipX = true;
            }

            else if (!spriteRenderer.flipX)
            {
                bulletSpriteRenderer.flipX = false;
            }
                */
                if (bulletSpriteRenderer != null)
                {
                    bulletSpriteRenderer.flipX = spriteRenderer.flipX;
                }
            }
        }
    }

    public void ChangeBullet(int bulletIndex)
    {
        if (bulletIndex >= 0 && bulletIndex < Bullet.Length)
        {
            currentBulletIndex = bulletIndex;

            PlayerPrefs.SetInt("PlayerBulletIndex", currentBulletIndex);
            PlayerPrefs.Save();
        }
    }
}
