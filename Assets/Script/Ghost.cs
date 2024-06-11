using System.Collections;
using UnityEngine;

public class Ghost : Monster
{
    public enum State
    {
        Idle,
        Run,
        Attack,
        Hit,
        Death,
    };

    public State currentState = State.Idle;

    public Transform genPoint;
    public GameObject Bullet;


    void Awake()
    {
        base.Awake();

        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;

        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
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
                case State.Hit:
                    yield return StartCoroutine(Hit());
                    break;
                case State.Death:
                    yield return StartCoroutine(Death());
                    break;
            }
        }
    }

    IEnumerator Idle()
    {
        MyAnimSetTrigger("Idle");

        if (Random.value > 0.5f)
        {
            MonsterFlip();
        }
        yield return new WaitForSeconds(1f);
        currentState = State.Run;
    }

    IEnumerator Run()
    {
        float runTime = Random.Range(2f, 4f);
        while (runTime >= 0f)
        {
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

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
            if (Random.value > 0.5f)
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

        MyAnimSetTrigger("Attack");

        float attackDuration = Random.Range(0.5f, 1f);
        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

            yield return null;
        }

        currentState = State.Idle;
    }

    IEnumerator Hit()
    {
        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간

        currentState = State.Idle;
    }

    IEnumerator Death()
    {
        MyAnimSetTrigger("Death");

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    void Fire()
    {
        GameObject bulletClone = Instantiate(Bullet, genPoint.position, genPoint.rotation);
        if (bulletClone != null)
        {
            bulletClone.GetComponent<Rigidbody2D>().velocity = transform.right * transform.localScale.x * 5f;
            Physics2D.IgnoreCollision(bulletClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }



    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (currentHp <= 0)
        {
            currentState = State.Death;
        }

        else
        {
            currentState = State.Hit;
        }
    }
}
