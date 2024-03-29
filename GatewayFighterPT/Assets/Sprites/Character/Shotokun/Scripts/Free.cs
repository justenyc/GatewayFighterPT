﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Code.CharacterControl;

namespace Assets.Code.Shoto
{
    public class Free : ICharacterBase
    {
        ShotokunManager manager;
        bool pressed = true;
        Vector3 myTrans;

        public Free(ShotokunManager managerRef)
        {
            manager = managerRef;
            manager.invulToStrike = false; //On animation cancel
            manager.grounded = true;
            manager.inputManager.DashEvent += manager.DashCheck;
            manager.dashCancel = true;

            if (Mathf.Abs(Input.GetAxis(manager.myAxisX)) < 0.1f)
                manager.anim.Play("0_Idle");
            else
                manager.anim.Play("1_Run");
        }

        public void StateStart()
        {
            throw new System.NotImplementedException();
        }

        public void StateUpdate()
        {
            myTrans = manager.transform.position;
            manager.AttackCheck();
            FreeMovement();
        }

        void FreeMovement()
        {
            manager.FlipX();
            DetectGround();

            if (Input.GetAxis(manager.myAxisY) > 0.5f) //Check for jump
            {
                //add the x and y axis to create the jump vector
                //manager.rb.velocity = new Vector2(Mathf.Round(Input.GetAxis(manager.myAxisX)) * manager.moveSpeed * Time.fixedDeltaTime, Mathf.Round(Input.GetAxis(manager.myAxisY)) * manager.jumpStrength * Time.fixedDeltaTime); ?? Why does this work?
                Object.Instantiate(manager.vfx["Jump"], new Vector3(manager.transform.position.x, manager.transform.position.y - 1.5f, manager.transform.position.z), Quaternion.identity);
                manager.activeState = new Jump(manager, new Vector2(Mathf.Round(Input.GetAxis(manager.myAxisX)), Mathf.Round(Input.GetAxis(manager.myAxisY))));
            }
            else if(manager.grounded == false)
            {
                manager.activeState = new Jump(manager, new Vector2(Mathf.Round(Input.GetAxis(manager.myAxisX)), 0));
            }
            else if (Mathf.Abs(Input.GetAxis(manager.myAxisX)) == 0)
            {
                pressed = false;
                manager.rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                manager.rb.velocity = Vector2.zero;
                manager.anim.Play("0_Idle");
            }
            else if (Input.GetAxis(manager.myAxisY) < 0.5f) //No input on the Y axis
            {
                //Set Velocity to be the input on the X axis and set animations on whether or not it's being used
                if (Mathf.Abs(Mathf.Round(Input.GetAxis(manager.myAxisX))) == 1)
                    manager.rb.velocity = new Vector2(Mathf.Round(Input.GetAxis(manager.myAxisX)) * manager.moveSpeed * Time.fixedDeltaTime, NegateGravityUpSlope() * Time.fixedDeltaTime);

                    /*manager.rb.velocity = new Vector2(Mathf.Round(Input.GetAxis(manager.myAxisX)) * manager.moveSpeed * Time.fixedDeltaTime,
                        (manager.CalculateSlope(manager.CalculateGroundAngle()) * -manager.transform.right.x) * manager.moveSpeed * Time.fixedDeltaTime);*/

                    //CheckSlopeY(manager.transform.right.x * Mathf.Ceil(manager.CalculateGroundAngle().x) / manager.CalculateGroundAngle().y) * manager.moveSpeed * Time.fixedDeltaTime);

                    //Debug.Log(manager.CalculateGroundAngle());

                    //Debug.Log(new Vector2(Mathf.Round(Input.GetAxis(manager.myAxisX)) * (Mathf.Abs(CheckSlope(manager.CalculateGroundAngle().x)) * manager.moveSpeed) * Time.fixedDeltaTime, manager.CalculateGroundAngle().y * manager.moveSpeed * Time.fixedDeltaTime));

                manager.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                if (Mathf.Abs(Input.GetAxis(manager.myAxisX)) != 0)
                {
                    if (pressed == false)
                    {
                        manager.anim.Play("1_Run");

                        if (manager.transform.rotation == Quaternion.Euler(Vector3.zero))
                            manager.vfx["Walk"].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 0, 0);
                        else
                            manager.vfx["Walk"].GetComponent<ParticleSystemRenderer>().flip = new Vector3(1, 0, 0);

                        Object.Instantiate(manager.vfx["Walk"], new Vector3(myTrans.x, myTrans.y - 0.8f, myTrans.z), Quaternion.identity);
                    }
                    pressed = true;
                }
            }

            manager.LayerByVelocity();
        }

        void DetectGround()
        {
            if (manager.passThrough == false)
            {
                int layerMask = ~(1 << 8);
                RaycastHit2D hit;
                hit = Physics2D.Raycast(manager.transform.position, -Vector2.up, 2f, layerMask);

                if (hit.collider == null)
                    manager.grounded = false;
                else
                    manager.grounded = true;
            }
            else
                return;
        }

        float NegateGravityUpSlope()
        {
            Vector2 climbingSlope = new Vector2(Mathf.Abs(manager.CalculateGroundAngle().x) * manager.transform.right.x, 0);
            float modifier = manager.rb.gravityScale;

            if (manager.CalculateSlope(manager.CalculateGroundAngle()) != 0)
            {
                if (climbingSlope.x > 0)
                {
                    return manager.moveSpeed * manager.CalculateSlope(manager.CalculateGroundAngle());
                }
                else
                    return -manager.moveSpeed * manager.CalculateSlope(manager.CalculateGroundAngle());
            }
            else
            {
                return -manager.moveSpeed * manager.CalculateSlope(manager.CalculateGroundAngle());
            }
        }

        float CheckSlopeY(float f)
        {
            if (f > 0)
                return 1;
            else
                return f;
        }
    }
}