using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private List<Transform> poseBones = new List<Transform>();
    private List<Transform> dollBones = new List<Transform>();

    public void CopyPose(Transform pose) 
    {
        AddChildren(pose, poseBones);
        AddChildren(transform, dollBones);

        foreach(Transform b in poseBones)
        {
            foreach(Transform d in dollBones)
            {
                if(d.name == b.name)
                {
                    d.eulerAngles = b.eulerAngles;
                    break;
                }
            }
        }
    }

    private void AddChildren(Transform parent, List<Transform> list)
    {
        list.Add(parent);
        foreach(Transform t in parent)
        {
            AddChildren(t, list);
        }
    }
}
