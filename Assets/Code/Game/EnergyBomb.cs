using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class EnergyBomb : MonoBehaviourPunCallbacks, IPunObservable
{

    public BasePlayer myplayer;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform mytransform;
    [SerializeField]
    private PhotonView photonview;

    [SerializeField]
    private GameObject[] myparticlesystems;

    [SerializeField]
    private GameObject OnHitExplosion;

    private GameObject explosion;


    [SerializeField]
    private AudioSource myaudio;

    [SerializeField]
    private AudioClip explosionsound;


    private bool isMoving = true;

    private bool toexplode;

    void Start()
    {
        mytransform = GetComponent<Transform>();

        if (MustBeExecute())
        {
            Vector3 cour = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var dir = new Vector2(mytransform.position.x, mytransform.position.y) - (Vector2)cour;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            mytransform.rotation = rot;
        }
    }

    void Update()
    {
        if (MustBeExecute() && isMoving)
        {
            mytransform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    private bool MustBeExecute()
    {
        return (photonview != null && photonview.IsMine) 
            || MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Offline;
    }

    private bool wasexploding;

    private void Explode()
    {
        isMoving = false;
        explosion = Instantiate(OnHitExplosion, mytransform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        myaudio.Stop();
        myaudio.clip = explosionsound;
        myaudio.Play();
        StartCoroutine(BeDestroyed());
    }

    private IEnumerator BeDestroyed()
    {
        foreach(var system in myparticlesystems)
        {
            system.SetActive(false);
        }
        yield return new WaitForSeconds(3f);
        Destroy(explosion);
        Destroy(mytransform.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (!MustBeExecute())
        {
            return;
        }
        if(!col.isTrigger && isMoving)
        {
            BasePlayer hitplayer = col.gameObject.GetComponent<BasePlayer>();
            if (hitplayer != null)
            {
                if (hitplayer != myplayer)
                {
                    if(MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
                    {
                        if (photonview.IsMine)
                        {
                            Debug.Log("mine");
                            if(hitplayer.GetGem())
                            {
                                Debug.Log("getting gem");
                                myplayer.AddGem();
                            }
                            RaiseEventOptions eventoptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                            SendOptions options = new SendOptions { Reliability = true };
                            object data = hitplayer.ID;
                            PhotonNetwork.RaiseEvent(3, data, eventoptions, options);
                        }
                    }
                    else
                    {
                        hitplayer.HitbyBomb();
                    }
                }
                else
                {
                    return;
                }
            }
            if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
            {
                if (photonview.IsMine)
                {
                    toexplode = true;
                }
            }
            Explode();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            if(!wasexploding)
            {
                wasexploding = true;
                Explode();
            }
        }
        else
        {
            stream.SendNext(toexplode);
        }
    }
}
