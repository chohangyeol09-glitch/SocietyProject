using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PersonAgent : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3[] route;
    int currentIndex;
    Action<GameObject> releaseAction;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    public void Init(Vector3[] route, Action<GameObject> releaseAction)
    {
        this.route = route;
        this.releaseAction = releaseAction;
        currentIndex = 0;

        agent.Warp(transform.position);
        agent.SetDestination(route[currentIndex]);
    }

    void Update()
    {
        if (agent.pathPending || agent.remainingDistance > 0.3f) return;

        currentIndex++;

        if (currentIndex >= route.Length)
        {
            releaseAction?.Invoke(gameObject);
            return;
        }

        agent.SetDestination(route[currentIndex]);
    }
}