
    using UnityEditorInternal;
    using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;

    public Vector3
        Margin,
        Smoothing;

    public BoxCollider Bounds;

    private Vector3
        _min,
        _max;

    public bool isFollowing { set; get; }

    public void Start()
    {
        _min = Bounds.bounds.min;
        _max = Bounds.bounds.max;
        isFollowing = true;
    }

    public void Update()
    {
        var x = transform.position.x;
        var y = transform.position.y;
        var z = transform.position.z;

        if (isFollowing)
        {
            if (Mathf.Abs(x - Player.position.x) > Margin.x)
                x = Mathf.Lerp(x, Player.position.x, Smoothing.x * Time.deltaTime);
            if (Mathf.Abs(y - Player.position.y) > Margin.y)
                y = Mathf.Lerp(y, Player.position.y, Smoothing.y * Time.deltaTime);
            if (Mathf.Abs(z - Player.position.z) > Margin.z)
                z = Mathf.Lerp(z, Player.position.z, Smoothing.z * Time.deltaTime);
        }

        var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);

        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, _min.y + GetComponent<Camera>().orthographicSize, _max.y - GetComponent<Camera>().orthographicSize);
        z = Mathf.Clamp(z, _min.z + GetComponent<Camera>().orthographicSize,
            _max.z - GetComponent<Camera>().orthographicSize);



        transform.position = new Vector3(x, y, z);
    }
}
