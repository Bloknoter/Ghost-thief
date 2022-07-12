using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllViewZone : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        BasePlayer pl = collision.gameObject.GetComponent<BasePlayer>();
        if (pl != null)
        {
            pl.BecomeVisible();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        BasePlayer pl = collision.gameObject.GetComponent<BasePlayer>();
        if (pl != null)
        {
            pl.BecomeInvisible();
        }
    }
}
