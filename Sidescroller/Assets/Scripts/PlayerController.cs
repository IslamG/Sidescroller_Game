using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : Entity
{
    //Player Handling
    public float gravity = 20;
    public float walkSpeed = 6;
    public float runSpeed = 12;
    public float acceleration = 30;
    public float jumpHeight = 12;
    public float slideDeceleration = 10;

    private float initialSlideThreshold = 7;

    //States
    private bool jumping;
    private bool sliding; 
    private bool wallHolding;
    private bool stopSliding;

    //System
    private float animationSpeed;
    private float currentSpeed;
    private float targetSpeed; 
    private Vector2 amountToMove;
    private float moveDirX;

    private PlayerPhysics playerPhysics;
    private Animator animator;
    private GameManagerScript manager;

    void Start()
    {
        playerPhysics = GetComponent<PlayerPhysics>();
        animator = GetComponent<Animator>();
        manager = Camera.main.GetComponent<GameManagerScript>();
    }

    void Update()
    {
        //Reset acceleration upon collision
        if(playerPhysics.movementStopped)
        {
            targetSpeed = 0;
            currentSpeed = 0;
        }
        
        //If player is touching the ground
        if(playerPhysics.grounded)
        {
            amountToMove.y = 0;

            if(wallHolding)
            {
                wallHolding = false;
                animator.SetBool("WallHold", false);
            }
            if(jumping)
            {
                jumping = false;
                animator.SetBool("Jumping", false);
            }
            if(sliding)
            {
                if(Mathf.Abs(currentSpeed) < .25f || stopSliding) 
                {
                    stopSliding = false;
                    sliding = false;
                    animator.SetBool("Sliding", false);
                    playerPhysics.ResetCollider();
                }
            }

            //Slide
            if(Input.GetButtonDown("Slide"))
            {
                if(Mathf.Abs(currentSpeed) > initialSlideThreshold)
                {
                    sliding = true;
                    animator.SetBool("Sliding", true);
                    targetSpeed = 0;

                    playerPhysics.SetCollider(new Vector3(3.5f, 4.8f, 3.5f), new Vector3(0.5f, 5f, 0));
                }
            }
        }
        else 
        {
            if (!wallHolding)
            {
                if (playerPhysics.canWallHold)
                {
                    wallHolding = true;
                    animator.SetBool("WallHold", true);
                }
            }
        }

        //Jump
        if(Input.GetButtonDown("Jump"))
        {
            if (sliding)
            {
                stopSliding = true;
            }
            else if(playerPhysics.grounded || wallHolding)
            {
                amountToMove.y = jumpHeight;
                jumping = true;
                animator.SetBool("Jumping", true);
                Debug.Log(animator.GetBool("Jumping"));

                if(wallHolding)
                {
                    wallHolding = false;
                    animator.SetBool("WallHold", false);
                }
            }
        }

        animationSpeed = IncrementTowardsTargetValue(animationSpeed, Mathf.Abs(targetSpeed), acceleration);
        animator.SetFloat("Speed", Mathf.Abs(animationSpeed));

        //Input
        moveDirX = Input.GetAxisRaw("Horizontal");
        if(!sliding)
        {
            float speed = Input.GetButton("Run") ? runSpeed : walkSpeed;
            targetSpeed= moveDirX * speed;
            currentSpeed = IncrementTowardsTargetValue(currentSpeed, targetSpeed, acceleration);

            //Face direction
            if(moveDirX != 0 && !wallHolding)
                transform.eulerAngles = moveDirX > 0 ? Vector3.up * 180 : Vector3.zero ;
        }
        else
        {
            currentSpeed = IncrementTowardsTargetValue(currentSpeed, targetSpeed, slideDeceleration);

        }

        //Set amount to move
        amountToMove.x = currentSpeed;

        if(wallHolding)
        {
            amountToMove.x = 0;
            if(Input.GetAxisRaw("Vertical") != -1)
            {
                amountToMove.y = 0;
            }
        }
        amountToMove.y -= gravity * Time.deltaTime;
        playerPhysics.Move(amountToMove * Time.deltaTime, moveDirX);

    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Checkpoint")
        {
            manager.SetCheckPoint(c.transform.position);
        }
        if (c.tag == "Finish")
        {
            manager.EndLevel();
        }
    }
    private float IncrementTowardsTargetValue(float n, float target, float a)
    {
        if (n == target) return n;
        else
        {
            float dir = Mathf.Sign(target - n);
            n += a * Time.deltaTime * dir;
            return (dir == Mathf.Sign(target-n)) ? n : target;
        }
    }
}
