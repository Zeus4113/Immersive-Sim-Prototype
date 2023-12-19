using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PatrolPathGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Vector3[] patrolPositions = new Vector3[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            patrolPositions[i] = transform.GetChild(i).position;
            Gizmos.DrawSphere(patrolPositions[i], 0.25f);
        }

        Gizmos.DrawLineStrip(patrolPositions, true);
    }
}
