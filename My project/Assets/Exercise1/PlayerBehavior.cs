using UnityEngine.AI;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private NavMeshAgent _agent;
    private WaypointManager _manager;
    private Waypoint _currentWaypoint;

    private void Awake()
    {
        _manager = FindObjectOfType<WaypointManager>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _currentWaypoint = _manager.waypoints[0];
        _agent.SetDestination(_currentWaypoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!(Vector3.Distance(transform.position, _currentWaypoint.transform.position) < 2)) return;
        _currentWaypoint = _currentWaypoint.nextWaypoint;
        _agent.SetDestination(_currentWaypoint.transform.position);
    }
}
