using UnityEngine;

public class MapDev : MonoBehaviour
{
    private CameraController cam;

    void Awake()
    {
        cam = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        //得到摄像机的水平角度
        float angle = cam.transform.eulerAngles.y;

        //用这个角度控制小地图旋转
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
