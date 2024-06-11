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
    protected CapsuleCollider2D capsuleCollider;
    protected BoxCollider2D childCollider;
    public GameObject hitBoxCollider;
    public Animator Anim;
    public LayerMask layerMask;


    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
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
        if (GameManager.Instance.player == null)
        {
            return false; // player가 null이면 false를 반환합니다.
        }
        
        if (transform.position.x > GameManager.Instance.player.transform.position.x ? MonsterDirLeft : !MonsterDirLeft)
        {
            return true;
        }
        return false;
    }

    protected void GroundCheck()
    {
        if (Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.size * transform.localScale.y, 0, Vector2.down, 0.2f, layerMask))
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
                rb.velocity = new Vector2(1f, 0);
            }

            else
            {
                rb.velocity = new Vector2(-1f, 0);
            }
        }
    }


    public virtual void Move()
    {
        if (GameManager.Instance.player == null)
        {
            return;
        }

        rb.velocity = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);

        if (MonsterDirLeft)
            moveDir = -0.3f;
        else
            moveDir = 0.3f;

        Vector2 currentPos = transform.position; // 현재 위치 기준
        Vector2 frontVec = new Vector2(currentPos.x + (MonsterDirLeft ? -capsuleCollider.size.x / 2 : capsuleCollider.size.x / 2), currentPos.y); // 앞 방향
        Vector2 topVec = new Vector2(currentPos.x + (MonsterDirLeft ? -capsuleCollider.size.x / 2 : capsuleCollider.size.x / 2), currentPos.y + 0.7f); // 위 방향

        // 레이 길이를 설정합니다.
        float frontRayLength = 0.5f;
        float topRayLength = 0.5f;

        // 디버그 레이를 그립니다.
        Debug.DrawRay(frontVec, (MonsterDirLeft ? Vector3.left : Vector3.right) * frontRayLength, new Color(0, 1, 0));
        Debug.DrawRay(topVec, (MonsterDirLeft ? Vector3.left : Vector3.right) * topRayLength, new Color(0, 1, 0));

        // 실제 레이캐스트를 수행합니다.
        RaycastHit2D front = Physics2D.Raycast(frontVec, MonsterDirLeft ? Vector3.left : Vector3.right, frontRayLength, LayerMask.GetMask("Platform"));
        RaycastHit2D top = Physics2D.Raycast(topVec, MonsterDirLeft ? Vector3.left : Vector3.right, topRayLength, LayerMask.GetMask("Platform"));

        if (front.collider != null && top.collider == null) // 점프
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (front.collider != null && top.collider != null) // 벽 방향전환
        {
            MonsterFlip();
        }

        GroundCheck();


        if (isGround)
        {
            Vector2 downVec = new Vector2(transform.position.x + moveDir, transform.position.y - 0.3f);
            Debug.DrawRay(downVec, Vector3.down * 0.5f, new Color(0, 1, 0));
            RaycastHit2D down = Physics2D.Raycast(downVec, Vector3.down, 0.5f, LayerMask.GetMask("Platform"));

            if (down.collider == null)
            {
                MonsterFlip();
            }
        }
    }

}

