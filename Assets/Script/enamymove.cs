using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enamymove : MonoBehaviour
{

    Rigidbody2D rigid;
    public int nextMove;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        
        //주어진 시간이 지난 뒤, 함수실행하는 ->invoke
        Invoke("Think", 3);
    }

    // Update is called once per frame
    void FixedUpdate()
    {    //Move
        rigid.velocity=new Vector2(nextMove,rigid.velocity.y);
        //Platfom Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextMove *= -1;
            CancelInvoke();
            Invoke("Think", 3);
        }
    }

    //재귀함수
    void Think()
    {
        nextMove = Random.Range(-1, 2);

        Invoke("Think", 3);
    }
}
