using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Transform target;
    // public bool StopMove { get; set; }
    public void SetTarget(Transform target)
    {
        this.target = target;
        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;
    }
    private void LateUpdate()
    {
        if (target)
        {
            Vector3 targetPosition = target.position;
            targetPosition.z = transform.position.z;
            transform.position = targetPosition;
        }
    }
}
