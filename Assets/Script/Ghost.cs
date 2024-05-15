using System.Collections;
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

    WaitForSeconds delay = new WaitForSeconds(1f);

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
            Debug.Log("Current State: " + currentState);
            switch (currentState)
            {
                case State.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case State.Run:
                    yield return StartCoroutine(Run());
                    break;
                case State.Attack:
                    yield return StartCoroutine(Attack());
                    break;
            }
        }
    }

    IEnumerator Idle()
    {
        Debug.Log("Entering Idle state");
        MyAnimSetTrigger("Idle");

        if (Random.value > 0.5f)
        {
            MonsterFlip();
        }
        yield return delay;
        currentState = State.Run;
    }

    IEnumerator Run()
    {
        Debug.Log("Entering Run state");
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
                        yield break;
                    }
                }
            }
            yield return null;
        }

        if (currentState != State.Attack)
        {
            currentState = Random.value >= 0.5f ? State.Idle : State.Run;
            MonsterFlip();
        }
    }

    IEnumerator Attack()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Attack");

        yield return new WaitForSeconds(Random.Range(0.5f, 1f)); // 총알 발사 간격
        currentState = State.Idle;
    }

    void Fire()
    {
        Debug.Log("Fire function called");
        if (Bullet == null || genPoint == null)
        {
            Debug.LogError("Bullet prefab or genPoint is not set.");
            return;
        }

        GameObject bulletClone = Instantiate(Bullet, genPoint.position, genPoint.rotation);
        if (bulletClone != null)
        {
            bulletClone.GetComponent<Rigidbody2D>().velocity = transform.right * transform.localScale.x * 5f;
            Physics2D.IgnoreCollision(bulletClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Debug.Log("Bullet instantiated and fired");
        }
        else
        {
            Debug.LogError("Bullet instantiation failed");
        }
    }

}
