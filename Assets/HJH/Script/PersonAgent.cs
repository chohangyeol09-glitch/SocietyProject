using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PersonAgent : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Vector3[] route;
    int currentIndex;
    Action<GameObject> releaseAction;

    [SerializeField] private AnimParamSO animParam;
    [SerializeField] private float deathDelay = 3f;
    [SerializeField] private float knockbackDistance = 1.5f;

    private bool isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(Vector3[] route, Action<GameObject> releaseAction)
    {
        this.route = route;
        this.releaseAction = releaseAction;
        currentIndex = 0;
        isDead = false;
        agent.Warp(transform.position);
        agent.SetDestination(route[currentIndex]);
    }

    void Update()
    {
        if (isDead) return;
        if (agent.pathPending || agent.remainingDistance > 0.3f) return;

        currentIndex++;
        if (currentIndex >= route.Length)
        {
            releaseAction?.Invoke(gameObject);
            return;
        }
        agent.SetDestination(route[currentIndex]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        if (collision.collider == null) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 hitPoint = collision.contacts.Length > 0
                ? collision.contacts[0].point
                : collision.gameObject.transform.position;

            OnRunOver();
            Die(hitPoint);
        }
    }

    private void Die(Vector3 hitPoint)
    {
        isDead = true;

        agent.SetDestination(transform.position);
        animator.Play(animParam.HashValue);

        Vector3 dir = transform.position - hitPoint;
        dir.y = 0f;
        dir = dir.sqrMagnitude > 0.001f ? dir.normalized : -transform.forward;

        StartCoroutine(DeathRoutine(dir));
    }

    private IEnumerator DeathRoutine(Vector3 knockbackDir)
    {
        float elapsed = 0f;

        while (elapsed < deathDelay)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / deathDelay;

            float speed = Mathf.Lerp(knockbackDistance / deathDelay * 2f, 0f, t);
            agent.Move(knockbackDir * speed * Time.deltaTime);

            yield return null;
        }

        releaseAction?.Invoke(gameObject);
    }
    public void OnRunOver()
    {
        // 강유야 치었을 때 이벤트 연결 메소드인데, 여기에서 별점 깎이는 거 하면 될 듯
        HealthManager.Instance.TakeDamage();
    }
}