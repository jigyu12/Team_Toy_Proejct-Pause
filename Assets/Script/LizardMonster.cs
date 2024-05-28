using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardMonster : Monster
{
    public enum State
    {
        Run,
        Attack,
        Hit,
        Death,
    };

    public State currentState = State.Run;

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

    IEnumerator Run()
    {
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
            MonsterFlip();
        }
    }

    IEnumerator Hit()
    {
        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간

        currentState = State.Run;
    }

    IEnumerator Death()
    {
        MyAnimSetTrigger("Death");

        capsuleCollider.enabled = false;

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Attack");

        yield return new WaitForSeconds(Random.Range(0.5f, 1f)); // 총알 발사 간격
        currentState = State.Run;
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
