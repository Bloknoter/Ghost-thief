using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{

    [SerializeField]
    private Animator anim;

    bool first;

    [SerializeField]
    private float YLevel;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<BasePlayer>() != null)
        {
            if(!first)
            {
                first = true;
                return;
            }
            if (collision.gameObject.transform.position.y > transform.position.y + YLevel)
                anim.SetTrigger("openright");
            else
                anim.SetTrigger("openleft");
        }
    }
}
