using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerController : BasePlayer, IPunObservable
{

    [SerializeField]
    private Transform mytransform;
    [SerializeField]
    private Rigidbody2D myrigidbody;

    protected IGemContainer gemcontainer;

    
    public void SetChancel(Chancel newchancel) { MyChancel = newchancel; }

    protected override void Start()
    {
        base.Start();
        mytransform = GetComponent<Transform>();
        myrigidbody = GetComponent<Rigidbody2D>();
        if (IsMine())
            BecomeVisible();
        else
            BecomeInvisible();

    }

    protected override void Update()
    {
        base.Update();
    }

    private Vector2 moving = Vector2.zero;

    public void OnRight()
    {
        moving += Vector2.right * speed * SPEED_MULTIPLIER;
    }

    public void OnLeft()
    {
        moving += Vector2.right * -speed * SPEED_MULTIPLIER;
    }

    public void OnUp()
    {
        moving += Vector2.up * speed * SPEED_MULTIPLIER;
    }

    public void OnDown()
    {
        moving += Vector2.up * -speed * SPEED_MULTIPLIER;
    }

    public void OnMove()
    {
        if(!IsMine())
        {
            return;
        }
        if (IsPlaying)
        {
            myrigidbody.MovePosition((Vector2)mytransform.position + moving);
        }
        else
        {
            ZeroMoving();
        }
    }

    public void ZeroMoving()
    {
        moving = Vector2.zero;
    }

    public void OnInteraction()
    {
        if (!IsMine())
        {
            return;
        }
        if (IsPlaying)
        {
            if (gemcontainer != null)
            {
                if (GemAmount() > 0)
                {
                    if (gemcontainer.CanBeAdded() && gemcontainer == (IGemContainer)MyChancel)
                    {
                        GetGem();
                        gemcontainer.AddGem();
                    }
                }
                else
                {
                    if (gemcontainer.CanBeTaken())
                    {
                        if (gemcontainer != (IGemContainer)MyChancel)
                        {
                            if (gemcontainer.GetGem())
                            {
                                AddGem();
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnShoot()
    {
        if (!IsMine())
        {
            return;
        }
        ShootEnergy();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.GetComponent<IGemContainer>() != null)
        {
            gemcontainer = col.gameObject.GetComponent<IGemContainer>();
        }
    }
    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<IGemContainer>() != null)
        {
            gemcontainer = null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(PhotonNetwork.NickName);
            stream.SendNext(new object[] { Skin.r, Skin.g, Skin.b });
            ID = GameController.Instance.ID;
            stream.SendNext(ID);
            stream.SendNext(gems);
            stream.SendNext(energy);
        }
        else
        {
            SetName((string)stream.ReceiveNext());
            object[] color = (object[])stream.ReceiveNext();
            Color c = new Color((float)color[0], (float)color[1], (float)color[2]);
            SetSkin(c);
            ID = (int)stream.ReceiveNext();
            gems = (int)stream.ReceiveNext();
            energy = (int)stream.ReceiveNext();
        }
    }

    protected override bool isBot()
    {
        return false;
    }

    protected override void onEvent(EventData photonEvent)
    {
        
    }

    protected override void OnBeingHitbyBomb()
    {
        
    }
}
