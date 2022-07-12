using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float DefaultCameraSize = 9.8f;

    [SerializeField]
    private GameObject MyCamera;

    private Transform cameratransform;

    private Transform targetrtransform;
    private Vector2 targetpoint = Vector2.zero;

    private enum MovingMode { to_transform, to_Vector2_point }
    private MovingMode movingmode = MovingMode.to_Vector2_point;

    public void SetTarget(Transform newtarget) { targetrtransform = newtarget; movingmode = MovingMode.to_transform; }
    public void SetTarget(Vector2 newtarget) { targetpoint = newtarget; movingmode = MovingMode.to_Vector2_point; }
    public void SetCameraSize(float newsize) { StartCoroutine(LerpingCameraSize(newsize)); }
    public void SetDefaultCameraSize() { StartCoroutine(LerpingCameraSize(DefaultCameraSize)); }

    [SerializeField]
    private float lerping;

    [SerializeField]
    private Transform LeftUpPoint;
    [SerializeField]
    private Transform RightDownPoint;
    public bool ConsiderWithBorders = true;


    void Start()
    {
        cameratransform = MyCamera.transform;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (movingmode == MovingMode.to_transform)
        {
            if (targetrtransform != null)
            {
                if ((Vector2)cameratransform.position != (Vector2)targetrtransform.position)
                {
                    Vector2 newpos;
                    Vector2 targetpos = targetrtransform.position;

                    if (ConsiderWithBorders)
                    {
                        if (targetpos.x < LeftUpPoint.position.x)
                        {
                            targetpos = new Vector2(LeftUpPoint.position.x, targetpos.y);
                        }
                        if (targetpos.y > LeftUpPoint.position.y)
                        {
                            targetpos = new Vector2(targetpos.x, LeftUpPoint.position.y);
                        }

                        if (targetpos.x > RightDownPoint.position.x)
                        {
                            targetpos = new Vector2(RightDownPoint.position.x, targetpos.y);
                        }
                        if (targetpos.y < RightDownPoint.position.y)
                        {
                            targetpos = new Vector2(targetpos.x, RightDownPoint.position.y);
                        }
                    }
                    newpos = Vector2.MoveTowards((Vector2)cameratransform.position, targetpos,
                            lerping * Time.deltaTime * Vector2.Distance((Vector2)cameratransform.position, targetpos));
                    cameratransform.position = new Vector3(newpos.x, newpos.y, cameratransform.position.z);


                }
            }
        }
        if(movingmode == MovingMode.to_Vector2_point)
        {
            if(targetpoint != null)
            {
                if ((Vector2)cameratransform.position != targetpoint)
                {
                    Vector2 newpos;
                    Vector2 targetpos = targetpoint;

                    if (ConsiderWithBorders)
                    {
                        if (targetpos.x < LeftUpPoint.position.x)
                        {
                            targetpos = new Vector2(LeftUpPoint.position.x, targetpos.y);
                        }
                        if (targetpos.y > LeftUpPoint.position.y)
                        {
                            targetpos = new Vector2(targetpos.x, LeftUpPoint.position.y);
                        }

                        if (targetpos.x > RightDownPoint.position.x)
                        {
                            targetpos = new Vector2(RightDownPoint.position.x, targetpos.y);
                        }
                        if (targetpos.y < RightDownPoint.position.y)
                        {
                            targetpos = new Vector2(targetpos.x, RightDownPoint.position.y);
                        }
                    }
                    newpos = Vector2.MoveTowards((Vector2)cameratransform.position, targetpos,
                            lerping * Time.deltaTime * Vector2.Distance((Vector2)cameratransform.position, targetpos));
                    cameratransform.position = new Vector3(newpos.x, newpos.y, cameratransform.position.z);


                }
            }
        }
    }

    private IEnumerator LerpingCameraSize(float newsize)
    {
        Camera cam = GetComponent<Camera>();
        if (newsize != cam.orthographicSize)
        {
            int time = 200;
            float delta = newsize - cam.orthographicSize;
            float acceleration = (2 * delta) / (time * time);
            float speed = acceleration * time;
            for (int i = 0; i < time; i++)
            {
                cam.orthographicSize += speed;
                speed -= acceleration;
                yield return new WaitForSeconds(0f);
            }
        }
    }

}
