using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationSpeed = 2;
    [SerializeField] private float approachSpeed = 10;
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float offsetDistance = 0.4f; /*避免摄像机来回拉扯抖动*/
    [SerializeField] private float minVerticalAngle = -45;
    [SerializeField] private float maxVerticalAngle = 45;

    [SerializeField] private Vector2 framingOffset;

    //相机是否颠倒
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;

    [SerializeField] private LayerMask obstaclesLayer;

    private float rotationX;
    private float rotationY;

    private float invertXVal;
    private float invertYVal;

    private float trueDistance;
    private RaycastHit raycastHit;


    void Start()
    {
        //游戏一开始就隐藏并锁定鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        trueDistance = maxDistance;
    }

    void Update()
    {
        // 相机移动方向是否要颠倒
        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        // 摄像机旋转
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // 摄像机看的位置应该在人物胸部高度，不应该在脚底，需要增加一个偏移量
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y, 0);

        // 用期望方向检测障碍
        var desiredDir = -(targetRotation * Vector3.forward);

        // 如果玩家和照相机之间有障碍就要改变distance的值
        if (HasObstacles(focusPosition, desiredDir))
        {
            trueDistance = Mathf.Lerp(trueDistance, raycastHit.distance - offsetDistance, Time.deltaTime * approachSpeed);
            if(trueDistance > maxDistance) trueDistance = maxDistance;
        }
        else
        {
            trueDistance = Mathf.Lerp(trueDistance, maxDistance, Time.deltaTime * approachSpeed);
        }

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, trueDistance);
        transform.LookAt(focusPosition);
    }

    /// <summary>
    /// 检测玩家和照相机之间是否有障碍
    /// </summary>
    /// <returns></returns>
    private bool HasObstacles(Vector3 focusPosition, Vector3 desiredDir)
    {
        // 这里不能用transform.position，因为摄像机每帧都会变化，可能导致拉近拉远摄像机时会抖动，这里用期望的方向来检测障碍，距离用摄像机与玩家之间的最远距离
        return Physics.Raycast(focusPosition, desiredDir, out raycastHit, maxDistance, obstaclesLayer);
    }

    /// <summary>
    /// 当人物移动时，只允许人物相对相机水平面角度移动
    /// </summary>
    /// <returns></returns>
    public Quaternion PlanarRotation()
    {
        return Quaternion.Euler(0, rotationY, 0);
    }
}
