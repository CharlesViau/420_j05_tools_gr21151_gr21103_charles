using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : UnityEditor.Editor
{
    private WaypointManager _waypointManager;
    private Transform _managerTransform;
    public List<Waypoint> waypoints;

    private Rect _dragRect;

    private void Awake()
    {
        _waypointManager = target as WaypointManager;
        _managerTransform = _waypointManager!.transform;
        waypoints = _waypointManager.waypoints ??= new List<Waypoint>();
    }

    public override void OnInspectorGUI()
    {
        RefreshWaypoints();

        DrawListLayout();

        if (GUILayout.Button("Add Waypoint"))
        {
            AddWaypoint();
        }

        DrawDragAndDropZone();
    }

    private void DrawDragAndDropZone()
    {
        _dragRect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(_dragRect, "Drag Object(s) Here to Add as Waypoint(s)", new GUIStyle(GUI.skin.box));

        if (!_dragRect.Contains(Event.current.mousePosition)) return;
        switch (Event.current.type)
        {
            case EventType.DragUpdated:
                OnDragAndDropUpdate();
                break;
            case EventType.DragPerform:
                OnDragAndDropPerform();
                break;
            case EventType.Repaint:
                return;
        }
            
        Event.current.Use();
    }

    private void OnDragAndDropPerform()
    {
        foreach (var obj in DragAndDrop.objectReferences)
        {
            if (!(obj is GameObject gameObject)) continue;
            if (!gameObject.TryGetComponent<Waypoint>(out var waypoint))
            {
                waypoint = gameObject.AddComponent<Waypoint>();
            }
            if (waypoints.Contains(waypoint)) continue;
            gameObject.transform.SetParent(_managerTransform);
            SetWaypoint(waypoint);
            waypoints.Add(waypoint);
        }
    }

    private static void OnDragAndDropUpdate()
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    }

    private void OnSceneGUI()
    {
        foreach (var waypoint in waypoints)
        {
            var position = waypoint.transform.position;
            var positionV2 = HandleUtility.WorldToGUIPoint(position);

            if (waypoint.nextWaypoint)
            {
                Handles.color = Color.green;
                Handles.DrawLine(position, waypoint.nextWaypoint.transform.position);
            }

            Handles.BeginGUI();
            GUI.backgroundColor = Color.grey;
            GUI.contentColor = Color.black;
            GUI.Box(new Rect(positionV2.x - 10, positionV2.y - 50, 70, 25), waypoint.name);
            Handles.EndGUI();
        }
    }

    private void AddWaypoint()
    {
        var waypointGameObject =
            new GameObject("Waypoint" + _managerTransform.childCount, typeof(Waypoint));
        waypointGameObject.transform.SetParent(_managerTransform);

        var waypoint = waypointGameObject.GetComponent<Waypoint>();
        
        SetWaypoint(waypoint);
        waypoints.Add(waypoint);

        Selection.activeGameObject = waypoint.gameObject;
    }

    private void DrawListLayout()
    {
        for (var i = 0; i < waypoints.Count(); i++)
        {
            if (!waypoints[i]) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(waypoints[i], typeof(Waypoint), false);
            if (GUILayout.Button(((char) 0x25B2).ToString()))
            {
                WaypointUp(waypoints[i]);
            }

            if (GUILayout.Button(((char) 0x25BE).ToString()))
            {
                WaypointDown(waypoints[i]);
            }

            if (GUILayout.Button("X"))
            {
                WaypointDelete(waypoints[i]);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void WaypointUp(Waypoint waypoint)
    {
        if (!waypoint.previousWaypoint) return;

        Waypoint tempWaypoint1 = null;

        //Changer point 1
        if (waypoint.previousWaypoint.previousWaypoint != null)
        {
            //Point 1 . Next is Changed
            waypoint.previousWaypoint.previousWaypoint.nextWaypoint = waypoint;
            tempWaypoint1 = waypoint.previousWaypoint.previousWaypoint;
        }

        //Change point 2
        var tempWaypoint2 = waypoint.previousWaypoint;
        waypoint.previousWaypoint.previousWaypoint = waypoint;
        waypoint.previousWaypoint.nextWaypoint = waypoint.nextWaypoint;

        //Change point 3
        waypoint.nextWaypoint = tempWaypoint2;
        waypoint.previousWaypoint = tempWaypoint1;

        Swap(waypoints, waypoints.IndexOf(waypoint), waypoints.IndexOf(waypoint.previousWaypoint));
    }

    private void WaypointDown(Waypoint waypoint)
    {
        if (!waypoint.nextWaypoint) return;

        Waypoint tempWaypoint3 = null;

        //Changer point 3
        if (waypoint.nextWaypoint.nextWaypoint != null)
        {
            //Point 3 . Previous is Changed
            waypoint.nextWaypoint.nextWaypoint.previousWaypoint = waypoint;
            tempWaypoint3 = waypoint.nextWaypoint.nextWaypoint;
        }

        //Change point 2
        var tempWaypoint2 = waypoint.nextWaypoint;
        waypoint.nextWaypoint.nextWaypoint = waypoint;
        waypoint.nextWaypoint.previousWaypoint = waypoint.previousWaypoint;

        //Change point 1
        waypoint.previousWaypoint = tempWaypoint2;
        waypoint.nextWaypoint = tempWaypoint3;

        Swap(waypoints, waypoints.IndexOf(waypoint), waypoints.IndexOf(waypoint.nextWaypoint));
    }

    private void WaypointDelete(Waypoint waypoint)
    {
        if (waypoint.nextWaypoint)
        {
            if (waypoint.previousWaypoint) waypoint.previousWaypoint.nextWaypoint = waypoint.nextWaypoint;
        }

        if (waypoint.previousWaypoint)
        {
            if (waypoint.nextWaypoint) waypoint.nextWaypoint.previousWaypoint = waypoint.previousWaypoint;
        }

        waypoints.Remove(waypoint);

        DestroyImmediate(waypoint.gameObject);
    }

    private void RefreshWaypoints()
    {
        for (var i = waypoints.Count() - 1; i >= 0; i--)
        {
            if (waypoints[i] == null)
            {
                if (i > 0 && i < waypoints.Count())
                {
                    waypoints[i - 1].nextWaypoint = waypoints[i + 1];
                    waypoints[i + 1].previousWaypoint = waypoints[i - 1];
                }

                waypoints.Remove(waypoints[i]);
            }
            else waypoints[i].gameObject.name = "Waypoint " + i;
        }
    }

    private static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    private void SetWaypoint(Waypoint waypoint)
    {
        if (!waypoints.Any()) return;
        //Assign Previous and Next Waypoints
        waypoint.previousWaypoint = waypoints[waypoints.Count() - 1];
        waypoint.previousWaypoint.nextWaypoint = waypoint;

        //Put the new Waypoint at the last one position
        var transform = waypoint.transform;
        var transform1 = waypoint.previousWaypoint.transform;

        transform.position = transform1.position;
        transform.forward = transform1.forward;
    }
}