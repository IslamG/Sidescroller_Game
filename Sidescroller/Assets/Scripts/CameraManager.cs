using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform target;
    private float trackSpeed = 10;

    public void SetTarget(Transform t)
    {
        target = t;
        transform.position = new Vector3(t.position.x, t.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        if(target){
            float x = IncrementTowardsTargetValue(transform.position.x, target.position.x, trackSpeed);
            float y = IncrementTowardsTargetValue(transform.position.y, target.position.y, trackSpeed);
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }

    
    private float IncrementTowardsTargetValue(float n, float target, float a)
    {
        if (n == target) return n;
        else
        {
            float dir = Mathf.Sign(target - n);
            n += a * Time.deltaTime * dir;
            return (dir == Mathf.Sign(target-n)) ? n : target;
        }
    }
}
