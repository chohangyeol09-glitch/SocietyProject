using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CarAgent : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    Transform[] waypoints;
    int currentIndex;
    float maxDistanceFromPlayer;
    Action<GameObject> releaseAction;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    public void Init(Transform player, Transform[] waypoints, float maxDistance, Action<GameObject> releaseAction)
    {
        this.player = player;
        this.waypoints = waypoints;
        this.maxDistanceFromPlayer = maxDistance;
        this.releaseAction = releaseAction;

        agent.Warp(transform.position);

        currentIndex = FindNearestWaypointIndex();
        agent.SetDestination(waypoints[currentIndex].position);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > maxDistanceFromPlayer)
        {
            releaseAction?.Invoke(gameObject);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentIndex].position);
        }
    }

    int FindNearestWaypointIndex()
    {
        float minDist = float.MaxValue;
        int nearest = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, waypoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }
        return nearest;
    }
}