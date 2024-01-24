using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : UnityEditor.Editor
{
    private Waypoint _waypoint;

    private void Awake()
    {
        _waypoint = (Waypoint) target;
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmotype)
    {
        if (waypoint == null) return;
        Gizmos.color = (gizmotype & GizmoType.Selected) != 0 ? Color.yellow : Color.yellow * 0.5f;
        var position = waypoint.transform.position;
        Gizmos.DrawSphere(position, 0.2f);
    }

    private void OnSceneGUI()
    {
        var position = _waypoint.transform.position;
        
        //Draw Line to Next Waypoint
        if (_waypoint.nextWaypoint)
        {
            Handles.color = Color.green;
            Handles.DrawLine(position, _waypoint.nextWaypoint.transform.position);
        }
        
        //Draw Line to Previous Waypoint
        if (_waypoint.previousWaypoint)
        {
            Handles.color = Color.red;
            Handles.DrawLine(position, _waypoint.previousWaypoint.transform.position);
        }
    }
}