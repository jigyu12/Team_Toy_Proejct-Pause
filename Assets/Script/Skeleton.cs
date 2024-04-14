using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Skeleton : Monster
{
    float moveDir;

    private void Awake()
    {
        base.Awake();
        moveSpeed = 2f;
        jumpPower = 5f;
    }


    void FixedUpdate()
    {

        if (!isHit)
        {
            rb.velocity = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);

            if (MonsterDirRight == true)
                moveDir = -0.5f;
            else
                moveDir = 0.5f;

            

            Vector2 currentPos = transform.position; // 현재 위치 기준
            Vector2 frontVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y); // 앞 방향
            Vector2 topVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y + 0.7f); // 위 방향



            Debug.DrawRay(frontVec, MonsterDirRight ? Vector3.right : Vector3.left, new Color(0, 1, 0));
            Debug.DrawRay(topVec, MonsterDirRight ? Vector3.right : Vector3.left, new Color(0, 1, 0));

            RaycastHit2D front = Physics2D.Raycast(frontVec, MonsterDirRight ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));
            RaycastHit2D top = Physics2D.Raycast(topVec, MonsterDirRight ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));

            if (front.collider != null) // 점프
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);

            if (top.collider != null) // 벽 방향전환
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


    protected void OnTriggerEnter2D(Collider2D collision) // 플레이어와 부딪히면 방향 전환
    {
        base.OnTriggerEnter2D(collision);
        if (collision.transform.CompareTag("PlayerHitBox"))
        {
            MonsterFlip();
        }
    }


}
