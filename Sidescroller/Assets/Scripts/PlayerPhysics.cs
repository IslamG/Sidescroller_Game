using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class PlayerPhysics : MonoBehaviour
{
    public LayerMask collisionMask;

    private BoxCollider col;
    private Vector3 size;
    private Vector3 center;

    private Vector3 originalSize; 
    private Vector3 originalCenter;
    private float colliderScale;

    private int collisionDivisionsX = 4;
    private int collisionDivisionsY = 6;

    private float skin = .005f;

    [HideInInspector]
    public bool grounded;
    [HideInInspector]
    public bool movementStopped;
    [HideInInspector]
    public bool canWallHold;

    private Transform platform;
    private Vector3 platformPositionOld;
    private Vector3 deltaPlatformPosition;

    Ray ray;
    RaycastHit hit;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        colliderScale = transform.localScale.x;

        originalSize = col.size;
        originalCenter = col.center;
        //SetCollider(originalSize, originalCenter);
    }

    public void Move(Vector2 moveAmount, float moveDirX)
    {

        float deltaY = moveAmount.y;
        float deltaX = moveAmount.x;

        Vector2 position = transform.position;

        if(platform)
        {
            deltaPlatformPosition = platform.position - platformPositionOld;
        }
        else
        {
            deltaPlatformPosition = Vector3.zero;
        }
        //Check collisions up/down
        grounded = false; 
        for(int i = 0; i < collisionDivisionsX; i++)
        {
            float dir = Mathf.Sign(deltaY);
            float x= (position.x + center.x - size.x/2) + size.x/(collisionDivisionsX -1) * i;
            float y = position.y + center.y + size.y/2 * dir;

            ray = new Ray(new Vector2(x,y), new Vector2(0,dir));
            Debug.DrawRay(ray.origin, ray.direction);


            if(Physics.Raycast(ray, out hit, Mathf.Abs(deltaY), collisionMask))
            {
                platform = hit.transform;
                platformPositionOld = platform.position;

                float dst = Vector3.Distance (ray.origin, hit.point);

                if (dst > skin) deltaY = (dst - skin) * dir; //-dst + skin;
                else
                {
                    deltaY = 0;
                }
                grounded = true;
                break;
            }
            else
            {
                platform = null;
            }
        }

        //Check collisions right/left
        movementStopped = false;
        canWallHold = false;

        if (deltaX != 0)
        {
            for(int i = 0; i < collisionDivisionsY; i++)
            {
                float dir = Mathf.Sign(deltaX);
                float x= position.x + center.x + size.x/2 * dir;
                float y = position.y + center.y - size.y/2 + size.y/(collisionDivisionsY -1) * i;

                ray = new Ray(new Vector2(x,y), new Vector2(dir, 0));
                Debug.DrawRay(ray.origin, ray.direction);

                if (Physics.Raycast(ray, out hit, Mathf.Abs(deltaX), collisionMask))
                {
                    if (hit.collider.tag == "Wall Jump")
                    {
                        if (Mathf.Sign(deltaX) == Mathf.Sign(moveDirX))
                            canWallHold = true;
                    }
                    float dst = Vector3.Distance (ray.origin, hit.point);

                    if (dst > skin) deltaX = -dst + skin;
                    else
                    {
                        deltaX = 0;
                    }
                    movementStopped = true;
                    break;
                }
            }
        }

        if (!grounded && !movementStopped)
        {
            Vector3 playerDir = new Vector3(deltaX, deltaY);
            Vector3 origin = new Vector3(position.x + center.x + size.x/2 * Mathf.Sign(deltaX), position.y + center.y + size.y/2 * Mathf.Sign(deltaY));
            ray = new Ray(origin, playerDir.normalized);
            if (Physics.Raycast(ray, Mathf.Sqrt(deltaX * deltaX * deltaY * deltaY)))
            {
                grounded = true;
                deltaY = 0;
            }
        }

        Vector2 finalTransform = new Vector2(deltaX + deltaPlatformPosition.x, deltaY);
        transform.Translate(finalTransform, Space.World);
    }

    public void SetCollider(Vector3 s, Vector3 c)
    {
        // col.size = s;
        // col.center = c;

        // center = c * colliderScale;
        // size = s * colliderScale;

        // Debug.Log(size + " " + center);
    }

    public void ResetCollider()
    {
        SetCollider(originalSize, originalCenter);
        //transform.Translate(new Vector2(0, 2f));
    }
}
