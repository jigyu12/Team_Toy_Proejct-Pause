using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int currentHp;
    public float moveSpeed;
    public float jumpPower;
    public float atkCoolTime;
    public float atkCoolTimeCalc;

    public bool isHit = false;
    public bool isGround = true;
    public bool canAtk = true;
    public bool MonsterDirLeft;
    public float moveDir;

    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    protected BoxCollider2D childCollider;
    public GameObject hitBoxCollider;
    public Animator Anim;
    public LayerMask layerMask;
    

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        childCollider = transform.Find("MonsterHitBox").GetComponent<BoxCollider2D>(); 
        Anim = GetComponent<Animator>();

        StartCoroutine(CalcCoolTime());
        StartCoroutine(ResetCollider());
    }

    IEnumerator ResetCollider()
    {
        while (true)
        {
            yield return null;
            if (!hitBoxCollider.activeInHierarchy)
            {
                yield return new WaitForSeconds(0.5f);
                hitBoxCollider.SetActive(true);
                isHit = false;
            }
        }
    }
    IEnumerator CalcCoolTime()
    {
        while (true)
        {
            yield return null;
            if (!canAtk)
            {
                atkCoolTimeCalc -= Time.deltaTime;
                if (atkCoolTimeCalc <= 0)
                {
                    atkCoolTimeCalc = atkCoolTime;
                    canAtk = true;
                }
            }
        }
    }

    public bool IsPlayingAnim(string AnimName)
    {
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName(AnimName))
        {
            return true;
        }
        return false;
    }
    public void MyAnimSetTrigger(string AnimName)
    {
        if (!IsPlayingAnim(AnimName))
        {
            Anim.SetTrigger(AnimName);
        }
    }

    protected void MonsterFlip()
    {
        MonsterDirLeft = !MonsterDirLeft;

        Vector3 thisScale = transform.localScale;
        if (MonsterDirLeft)
        {
            thisScale.x = -Mathf.Abs(thisScale.x);
        }
        else
        {
            thisScale.x = Mathf.Abs(thisScale.x);
        }
        transform.localScale = thisScale;
        rb.velocity = Vector2.zero;
    }

    protected bool IsPlayerDir()
    {
        if (transform.position.x > GameManager.Instance.player.transform.position.x ? MonsterDirLeft : !MonsterDirLeft)
        {
            return true;
        }
        return false;
    }

    protected void GroundCheck()
    {
        if (Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.size, 0, Vector2.down, 0.05f, layerMask))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    public virtual void TakeDamage(int dam)
    {
        currentHp -= dam;
        isHit = true;
        hitBoxCollider.SetActive(false);
        // Knock Back or Dead
        if (currentHp <= 0)
        {
            Debug.Log("Monster Dead");
        }

        else
        {
            rb.velocity = Vector2.zero;
            if (transform.position.x > GameManager.Instance.player.transform.position.x)
            {
                rb.velocity = new Vector2(3f, 0);
            }
            
            else
            {
                rb.velocity = new Vector2(-3f, 0);
            }
        }
    }

    //protected void OnTriggerEnter2D(Collider2D collision) // 플레이어 투사체에 피격
    //{
    //    //if ( collision.transform.CompareTag ( ?? ) )
    //    //{
    //    //TakeDamage ( 0 );
    //    //}
    //}

    public void Move()
    {
        rb.velocity = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);

        if (MonsterDirLeft == true)
            moveDir = -0.5f;
        else
            moveDir = 0.5f;



        Vector2 currentPos = transform.position; // 현재 위치 기준
        Vector2 frontVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y); // 앞 방향
        Vector2 topVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y + 0.7f); // 위 방향



        Debug.DrawRay(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, new Color(0, 1, 0));
        Debug.DrawRay(topVec, MonsterDirLeft ? Vector3.right : Vector3.left, new Color(0, 1, 0));

        RaycastHit2D front = Physics2D.Raycast(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D top = Physics2D.Raycast(topVec, MonsterDirLeft ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));

        if (front.collider != null && top.collider == null) // 점프
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

        if (front.collider != null && top.collider != null) // 벽 방향전환
            MonsterFlip();


        GroundCheck();
        if (top.collider == null && front.collider == null && isGround)
        {
            Vector2 downVec = new Vector2(transform.position.x + moveDir, transform.position.y);
            Debug.DrawRay(downVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D down = Physics2D.Raycast(downVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (down.collider == null)
                MonsterFlip();
        }
    }
}
