using System;
using UnityEngine;
using System.Collections;
using DigitalRuby.PyroParticles;

public class Player : MonoBehaviour
{
    private Boolean _isFacingRight;
    private CharacterController _controller;
    private CharacterController3D _character;

    private float _normalizedHorizontalSpeed;
    private float _normalizedDepthSpeed;

    public GameObject SpellSpawnPoint;

    public GameObject DevestationSpellSpwanPoint;

    public float MaxSpeed = 8;
    public float Gravity;



    private Quaternion _lookLeft,
                     _lookRight,
                     _lookForward,
                     _lookBack,
                     _lookWandD,
                    _lookWandA,
                     _lookSandA,
                    _lookSandD;

   

    private Vector3 _moveDirection = Vector3.zero;

    public float JumpSpeed = 8.0f;

    public UnityEngine.UI.Text CurrentItemText;
    public GameObject[] Prefabs;


    private GameObject currentPrefabObject;
    private FireBaseScript currentPrefabScript;
    private int currentPrefabIndex;


    private Quaternion originalRotation;

    private Vector3 FireDirection;


    //public Transform StartPosition;
    // public Transform EndPosition;
    // float StartTime;
    // private float TotalDistanceToDestination;



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
    public void Awake()
    {

        // Cursor.visible = false;
        //   StartTime = Time.time;
        // TotalDistanceToDestination = Vector3.Distance(StartPosition.position, EndPosition.position);
        Time.timeScale = 1;

        _lookRight = transform.rotation;
        _lookLeft = _lookRight * Quaternion.Euler(0, 180, 0);

        _lookForward = _lookRight * Quaternion.Euler(0, 90, 0);
        _lookBack = _lookRight * Quaternion.Euler(0, -90, 0);
        _lookWandD = _lookRight * Quaternion.Euler(0, 45, 0);
        _lookWandA = _lookRight * Quaternion.Euler(0, -45, 0);
        _lookSandD = _lookRight * Quaternion.Euler(0, 135, 0);
        _lookSandA = _lookRight * Quaternion.Euler(0, -135, 0);
        originalRotation = transform.localRotation;
        UpdateUI();
        //Health = MaxHealth;
    }

    // Update is called once per frame
    public void Update()
    {
        UpdateEffect();
        _controller = GetComponent<CharacterController>();

        if (_controller.isGrounded)
        {

            _moveDirection = new Vector3((Input.GetAxis("Horizontal")), 0, Input.GetAxis("Vertical"));

            //Handle verticl slope down

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                //Debug.Log("Got in here");
                transform.rotation = _lookWandD;
                _moveDirection = new Vector3(1,0,1); 
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;

            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                transform.rotation = _lookWandA;
                _moveDirection = new Vector3(-1,0,1);
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                transform.rotation = _lookSandD;
                _moveDirection = new Vector3(1,0,-1);
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                transform.rotation = _lookSandA;
                _moveDirection = new Vector3(-1, 0, -1);
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;

            }
            //else if for other combinations

            else if (Input.GetKey(KeyCode.A))
            {
                transform.rotation = _lookBack;
                _moveDirection = Vector3.left;
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;
                //add animation here
            }
            else if (Input.GetKey(KeyCode.D))
            {

                transform.rotation = _lookForward;
                _moveDirection = Vector3.right;
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;
                //Add Animation here

            }

            else if (Input.GetKey(KeyCode.W))
            {
                transform.rotation = _lookRight;
                _moveDirection = Vector3.forward;
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;

            }
            else if (Input.GetKey(KeyCode.S))
            {
                // _moveDirection = transform.InverseTransformDirection(-_moveDirection);

                transform.rotation = _lookLeft;
                _moveDirection = -Vector3.forward;
                SetFireDirection(_moveDirection);
                _moveDirection *= MaxSpeed;
            }



            else
            {
                _moveDirection = Vector3.zero;
            }

            if (Input.GetKey(KeyCode.Space))
            {
               _moveDirection.y = JumpSpeed;
            }


        }
        //TODO: Figure out how to go down a ramp/stairs    


        _moveDirection.y -= Gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
        // gravity = 90; // Reset gravity

        //var movementFactor = _character.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        //if (IsDead)
        //    _character.SetHorizontalForce(0);
        //else
        //{
        //    _character.SetHorizontalForce(Mathf.Lerp(_character.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
        //}


    }

    private void UpdateEffect()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {

            StartCurrent();
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {

            NextPrefab();
        }
        //else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.KeypadMinus))
        //{

        //    PreviousPrefab();
        //}
    }



    private void BeginEffect()
    {
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        //Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forwardY = new Vector3(0, 0, 0);
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;
        currentPrefabObject = GameObject.Instantiate(Prefabs[currentPrefabIndex]);
        currentPrefabScript = currentPrefabObject.GetComponent<FireConstantBaseScript>();


        switch (Convert.ToInt32(yRot))
        {
            case 0:
                forwardY = new Vector3(0, 0, 1);
                break;
            case 90:
                forwardY = new Vector3(1, 0, 0);
                break;
            case 180:
                forwardY = new Vector3(0, 0, -1);
                break;
            case 270:
                forwardY = new Vector3(-1, 0, 0);
                break;
            default:
                break;



        }



        if (currentPrefabScript == null)
        {
            // temporary effect, like a fireball
            currentPrefabScript = currentPrefabObject.GetComponent<FireBaseScript>();
            if (currentPrefabScript.IsProjectile)
            {
                // set the start point near the player
                rotation = transform.rotation;
                pos = SpellSpawnPoint.transform.position;

            }
            else
            {
                // set the start point in front of the player a ways
                if (currentPrefabObject.name.Contains("Strike"))
                {

                    pos = DevestationSpellSpwanPoint.transform.position;
                }
                else
                {
                    pos = transform.position + (forwardY * 10.0f);

                }


            }
        }
        else
        {
            // set the start point in front of the player a ways, rotated the same way as the player
            pos = transform.position + (forwardY * 5.0f);
            rotation = transform.rotation;
            pos.y = 0.0f;
        }

        FireProjectileScript projectileScript = currentPrefabObject.GetComponentInChildren<FireProjectileScript>();
        if (projectileScript != null)
        {
            // make sure we don't collide with other friendly layers
            projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FriendlyLayer"));
        }

        currentPrefabObject.transform.position = pos;
        currentPrefabObject.transform.rotation = rotation;
    }

    public void StartCurrent()
    {
        StopCurrent();
        BeginEffect();
    }

    private void StopCurrent()
    {
        // if we are running a constant effect like wall of fire, stop it now
        if (currentPrefabScript != null && currentPrefabScript.Duration > 10000)
        {
            currentPrefabScript.Stop();
        }
        currentPrefabObject = null;
        currentPrefabScript = null;
    }

    public void NextPrefab()
    {
        currentPrefabIndex++;
        if (currentPrefabIndex == Prefabs.Length)
        {
            currentPrefabIndex = 0;
        }
        UpdateUI();
    }

    public void PreviousPrefab()
    {
        currentPrefabIndex--;
        if (currentPrefabIndex == -1)
        {
            currentPrefabIndex = Prefabs.Length - 1;
        }
        UpdateUI();
    }

    public void SetFireDirection(Vector3 direction)
    {
        FireDirection = direction;
    }

    private void UpdateUI()
    {
        CurrentItemText.text = "Spell Name: " + Prefabs[currentPrefabIndex].name;
    }


}
