using Sirenix.OdinInspector;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject toFollow;
    [SerializeField] float smoothTime;
    Vector3 _velocity = Vector3.zero;
    
    void LateUpdate()
    {
        CenterCamera();
    }
    
    [Button]
    void CenterCamera()
    {
        var targetPos = toFollow.transform.position;
        var currentPos = transform.position;
        var finalPos = new Vector3(targetPos.x, targetPos.y, currentPos.z);
        transform.position = Vector3.SmoothDamp(currentPos, finalPos, ref _velocity, smoothTime);
    }
}
