
using System;
using UnityEngine;


[Serializable]
public class ControllerParameters3D
{
        public enum JumpBehavior
        {
            CanJumpOnGround,
            CanJumpAnyWhere,
            CantJump
        }

    public  Vector3 MaxVelocity = new Vector3(float.MaxValue, float.MaxValue,  float.MaxValue);

    [Range(0, 90)] public float SlopeLimit = 30;

    public float Gravity = -25f;

    public JumpBehavior JumpRestrictions;

    public float JumpFrequency = .25f;

    public float JumpMagnitude = 12;
}
