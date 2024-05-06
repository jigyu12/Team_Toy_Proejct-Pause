using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Monster
{
    public enum State
    {
        Idle,
        Run,
        Attack,
    };

    public State currentState = State.Idle;

    public Transform genPoint;
    public GameObject Bullet;



    WaitForSeconds Delay = new WaitForSeconds(1f);

    void Awake()
    {
        base.Awake();
        moveSpeed = 1f;
        jumpPower = 0.3f;
        currentHp = 4;
        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;

        StartCoroutine(FSM());

    }

    IEnumerator FSM()
    {
        while (true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
    }

    IEnumerator Idle()
    {
        yield return null;
        

        MyAnimSetTrigger("Idle");

        if (Random.value > 0.5f)
        {
            MonsterFlip();
        }
        yield return Delay;
        currentState = State.Run;
    }

    IEnumerator Run()
    {
        yield return null;
        

        float runTime = Random.Range(2f, 4f);
        while (runTime >= 0f)
        {
            runTime -= Time.deltaTime;
            MyAnimSetTrigger("Run");
            if (!isHit)
            {
                Move();

                if (canAtk && IsPlayerDir())
                {
                    if (Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                    {
                        currentState = State.Attack;
                        break;
                    }
                }

            }
            yield return null;
        }
        if (currentState != State.Attack)
        {
            if (Random.value >= 0.5f)
            {
                MonsterFlip();
            }
            else
            {
                currentState = State.Idle;
            }
        }
    }

    IEnumerator Attack()
    {
        yield return null;
        

        canAtk = false;
        // rb.velocity = new Vector2(0, jumpPower);

        MyAnimSetTrigger("Attack");
        while (currentState == State.Attack)
        {
            Fire();
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
            break;
        }

        yield return Delay;
        currentState = State.Idle;
    }



    void Fire()
    {
        GameObject bulletClone = Instantiate(Bullet, genPoint.position, transform.rotation);

        // GameObject bulletClone = Instantiate(Bullet, genPoint.position, Quaternion.identity);
        Physics2D.IgnoreCollision(bulletClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bulletClone.GetComponent<Rigidbody2D>().velocity = transform.right * transform.localScale.x * 5f; // -transform.localScale.x
        bulletClone.transform.localScale = new Vector2(transform.localScale.x * 0.3f, 0.3f);
        
    }
}
