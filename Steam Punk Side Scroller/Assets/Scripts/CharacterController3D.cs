
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterController3D : MonoBehaviour
{
    private const float SkinWidth = .02f;
    private const int TotalHorizontalRays = 8;
    private const int TotalVerticalRays = 4;


    private static readonly float SlopeLimitTangent = Mathf.Tan(75f*Mathf.Deg2Rad);

    public LayerMask PlatformMask;

    public ControllerParameters3D DefaultParameters;

    public GameObject StandingOn { get; private set; }

    public ControllerState3D State { get; private set; }

    public Vector2 Velocity { get { return _velocity; } }

    public bool CanJump
    {
        get
        {
            if (Parameters.JumpRestrictions == ControllerParameters3D.JumpBehavior.CanJumpAnyWhere)
                return _jumpIn <= 0;

            if (Parameters.JumpRestrictions == ControllerParameters3D.JumpBehavior.CanJumpOnGround)
                return State.IsGrounded;

            return false;
        }
    }

    public bool HandleCollisions { get; set; }

    public ControllerParameters3D Parameters { get { return _overrideParameters ?? DefaultParameters;  } }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private CapsuleCollider _boxCollider;
    private ControllerParameters3D _overrideParameters;
    private Vector3 _raycastTopLeft,
                    _raycastBottomLeft,
                    _raycastBottomRight;


    private float
        _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;

    private float _jumpIn;

    private Vector3 _activeGlobalPlatformPoint,
                     _activeLocalPlatformPoint;
    private GameObject _lastStandingOn;


    public void Awake()
    {
        HandleCollisions = true;
        State = new ControllerState3D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<CapsuleCollider>();


    }

    public void LateUpdate()
    {
        _jumpIn -= Time.deltaTime;
        _velocity.y += Parameters.Gravity*Time.deltaTime;
        Move(Velocity * Time.deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        _velocity = force;
    }

    public void SetForce(Vector2 force)
    {
        _velocity += force;
    }

    public void SetVeticalForce(float y)
    {
        _velocity.y = y;

    }

    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }

    

    private void Move(Vector3 deltaMovement)
    {
        var wasGround = State.IsCollidingBelow;
        State.Reset();

        if (HandleCollisions)
        {
            //HandlePlatforms();
            //CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGround)
                HandleVerticalSlope(ref deltaMovement);
            if (Mathf.Abs(deltaMovement.x) > 0.01f)
                MoveHorizontally(ref deltaMovement);

            MoveVertically(ref deltaMovement);
            CorrectHorizontalPlacment(ref deltaMovement, true);
            CorrectHorizontalPlacment(ref deltaMovement, false);

            _transform.Translate(deltaMovement, Space.World);

            if (Time.deltaTime > 0)
                _velocity = deltaMovement / Time.deltaTime;

            _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
            _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);
           // _velocity.z = Mathf.Min(_velocity.z, Parameters.MaxVelocity.z);

            if (StandingOn != null)
            {
                _activeGlobalPlatformPoint = transform.position;
                _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);

                //Debug.DrawLine(transform.position, _activeGlobalPlatformPoint);
                // Debug.DrawLine(transform.position, _activeLocalPlatformPoint);

                if (_lastStandingOn != StandingOn)
                {
                    if (_lastStandingOn != null)
                        _lastStandingOn.SendMessage("ControllerExit3D", this, SendMessageOptions.DontRequireReceiver);

                    StandingOn.SendMessage("ControllerEnter3D", this, SendMessageOptions.DontRequireReceiver);
                    _lastStandingOn = StandingOn;
                }
                else if (StandingOn != null)
                    StandingOn.SendMessage("ControllerStay3D", this, SendMessageOptions.DontRequireReceiver);


            }
            else if (_lastStandingOn != null)
            {
                _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
                _lastStandingOn = null;
            }


        }
    }

    private void CorrectHorizontalPlacment(ref Vector3 deltaMovement, bool isRight)
    {
        var halfWidth = (_boxCollider.radius * _localScale.x) / 2f;
        var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        if (isRight)
            rayOrigin.x -= (halfWidth - SkinWidth);
        else
            rayOrigin.x += (halfWidth - SkinWidth);


        var rayDirection = isRight ? Vector2.right : -Vector2.right;

        var offset = 0f;

        for (var i = 1; i < TotalHorizontalRays - 1; i++)
        {
            var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            //Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
            if (!raycastHit)
                continue;
            offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
        }

        //Push player to the left or right if hit by platform
        deltaMovement.x += offset;
    }

    private void CalculateRayOrigins()
    {
        //var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        //var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        //_raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        //_raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        //_raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);


    }

    private void MoveHorizontally(ref Vector3 deltaMovement)
    {
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        for (var i = 0; i < TotalHorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            

            if (!rayCastHit)
                continue;
            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;

            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;

            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }
            if (rayDistance < SkinWidth + .0001f)
                break;

            //TODO: Watch next video on Vertical movement.
        }
    }

    private void MoveVertically(ref Vector3 deltaMovement)
    {
        var isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;

        var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

        rayOrigin.x += deltaMovement.x;

        var standingOnDistance = float.MaxValue;

        for (var i = 0; i < TotalVerticalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            if (!raycastHit)
                continue;
            if (!isGoingUp)
            {
                var vecticalDistanceToHit = _transform.position.y - raycastHit.point.y;
                if (vecticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = vecticalDistanceToHit;
                    StandingOn = raycastHit.collider.gameObject;

                }
            }
            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp)
            {
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            }
            else
            {
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }

            if (!isGoingUp && deltaMovement.y > .0001f)
                State.IsMovingUpSlope = true;

            if (rayDistance < SkinWidth + .0001)
                break;
        }

    }

    private void HandleVerticalSlope(ref Vector3 deltaMovement)
    {
        var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
        var direction = -Vector2.up;

        var slopeDistance = SlopeLimitTangent * (_raycastBottomRight.x - center);
        var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);

        Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.white);
        var raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        if (!raycastHit)
            return;

        //ReSharper disable CompareOfFloatsByEqualityOperator

        var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);

        if (!isMovingDownSlope)
            return;
        var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        if (Mathf.Abs(angle) < .0001f)
            return;

        State.IsMovingDownSlope = true;
        State.SlopeAngle = angle;
        deltaMovement.y = raycastHit.point.y - slopeRayVector.y;

    }

    private bool HandleHorizontalSlope(ref Vector3 deltaMovement, float angle, bool isGoingRight)
    {
        if (Mathf.RoundToInt(angle) == 90)
            return false;

        if (angle > Parameters.SlopeLimit)
        {
            deltaMovement.x = 0;
            return true;

        }
        if (deltaMovement.y > .07f)
            return true;

        deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;
        return true;

    }

    public void OnTriggerEnter3D(Collider other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume>();
        if(parameters == null)
            return;
        _overrideParameters = parameters.Parameters;
    }

    public void OnTriggerExit3D(Collider other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume>();
        if (parameters == null)
            return;
        _overrideParameters = null;
    }

}
