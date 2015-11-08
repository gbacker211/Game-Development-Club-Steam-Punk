using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Boolean _isFacingRight;
    private CharacterController _controller;
    private CharacterController3D _character;

    private float _normalizedHorizontalSpeed;
    private float _normalizedDepthSpeed;


    public float MaxSpeed = 8;
    public float gravity = 20.0f;

    private Quaternion lookLeft;
    private Quaternion lookRight;
    public float smooth = 1f;

    private Vector3 moveDirection = Vector3.zero;
   
    public float jumpSpeed = 8.0f;







    public bool IsDead { get; private set; }

    public CharacterController PlayerParameters3D
    {
        get { return _controller; }
        set { _controller = value; }
    }


    public CharacterController3D PlayCharacterController3D
    {
        get { return _character; }
        set { _character = value; }
    }


    //TODO: Set up player class, use 2D game player class as a template.

    // Use this for initialization
    public void Start()
    {

       // Cursor.visible = false;

        Time.timeScale = 1;

        lookRight = transform.rotation;
        lookLeft = lookRight * Quaternion.Euler(0, 180, 0);
        //Health = MaxHealth;
    }

    // Update is called once per frame
    public void Update()
    {
        _controller = GetComponent<CharacterController>();

        if (_controller.isGrounded)
        {

            moveDirection = new Vector3((Input.GetAxis("Horizontal")), 0, Input.GetAxis("Vertical"));

           
            
            if (Input.GetKey(KeyCode.A))
            {
                transform.rotation = lookLeft;
                moveDirection = transform.TransformDirection(-moveDirection);
                moveDirection *= MaxSpeed;
                //add animation here
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.rotation = lookRight;
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= MaxSpeed;
                //Add Animation here

            }
            else if (Input.GetKey(KeyCode.W))
            {
                moveDirection = Vector3.forward;
                moveDirection *= MaxSpeed;

            }
            else if (Input.GetKey(KeyCode.S))
            {
               // moveDirection = transform.InverseTransformDirection(-moveDirection);
               moveDirection = -Vector3.forward;
                
                moveDirection *= MaxSpeed;
            }
            
            
            else
            {
                moveDirection = Vector3.zero;
            }

            if (Input.GetKey(KeyCode.Space))
            {
               

                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        _controller.Move(moveDirection * Time.deltaTime);


        //var movementFactor = _character.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        //if (IsDead)
        //    _character.SetHorizontalForce(0);
        //else
        //{
        //    _character.SetHorizontalForce(Mathf.Lerp(_character.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
        //}


    }

    private void HandleInput()
    {
        // float Speed = 5;


    }
}
