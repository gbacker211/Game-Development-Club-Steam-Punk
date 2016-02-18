
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Player;  
    public float OffsetX = 0;
    public float OffsetY = 6;
    public float OffsetZ = -10;

    public Vector3 Smoothing,
        Margin;




    private Vector3 _relCameraPos;
    private float _relCameraPosMag;
    private Vector3 _newPos;
    private float _followNow;
    private bool _isFollowing;

    public void Start()
    {
        _isFollowing = true;
    }


    public void LateUpdate()
    {

        var x = transform.position.x;
        var y = transform.position.y;
        var z = transform.position.z;


        if (Player != null)
        {
            if (_isFollowing)
            {
                if (Mathf.Abs(x - Player.transform.position.x) > Margin.x)
                    x = Mathf.Lerp(x, Player.transform.position.x + OffsetX, Smoothing.x * Time.deltaTime);

                if (Mathf.Abs(y - Player.transform.position.y) > Margin.y)
                    y = Mathf.Lerp(y, Player.transform.position.y + OffsetY, Smoothing.y * Time.deltaTime);
                if (Mathf.Abs(z - Player.transform.position.z) > Margin.z)
                    z = Mathf.Lerp(z, Player.transform.position.z + OffsetZ, Smoothing.z * Time.deltaTime);
            }

            // z = Player.transform.position.z;
            // y = Player.transform.position.y;

            this.transform.position = new Vector3(x, y, z);
        }

    }
}
