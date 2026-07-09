using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ObstacleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] carSpawnPosition;
    [SerializeField] private Transform[] peopleSpawnPosition;
    [SerializeField] private Transform[] carWaypoints;

    [Header("Prefabs")]
    [SerializeField] private CarAgent carPrefab;
    [SerializeField] private PersonAgent personPrefab;

    [Header("Zone Radius")]
    [SerializeField] private float safeZoneRadius = 5f;
    [SerializeField] private float spawnZoneRadius = 12f;
    [SerializeField] private float despawnRadius = 20f;

    [Header("Spawn Settings")]
    [SerializeField] private float carSpawnInterval = 3f;
    [SerializeField] private float personSpawnInterval = 4f;
    [SerializeField] private int personWaypointVisitCount = 3;

    private ObjectPool<CarAgent> carPool;
    private ObjectPool<PersonAgent> personPool;

    void Awake()
    {
        carPool = new ObjectPool<CarAgent>(
            () => Instantiate(carPrefab),
            car => car.gameObject.SetActive(true),
            car => car.gameObject.SetActive(false),
            car => Destroy(car.gameObject),
            false, 10
        );

        personPool = new ObjectPool<PersonAgent>(
            () => Instantiate(personPrefab),
            p => p.gameObject.SetActive(true),
            p => p.gameObject.SetActive(false),
            p => Destroy(p.gameObject),
            false, 10
        );
    }

    void Start()
    {
        StartCoroutine(SpawnCarsRoutine());
        StartCoroutine(SpawnPeopleRoutine());
    }

    IEnumerator SpawnCarsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(carSpawnInterval);
            TrySpawnCar();
        }
    }

    IEnumerator SpawnPeopleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(personSpawnInterval);
            TrySpawnPerson();
        }
    }

    void TrySpawnCar()
    {
        var candidates = carSpawnPosition
            .Where(p =>
            {
                float dist = Vector3.Distance(p.position, player.position);
                return dist > safeZoneRadius && dist <= spawnZoneRadius;
            })
            .ToArray();

        if (candidates.Length == 0) return;

        var point = candidates[Random.Range(0, candidates.Length)];
        var car = carPool.Get();
        car.transform.SetPositionAndRotation(point.position, point.rotation);
        car.Init(player, carWaypoints, despawnRadius, ReleaseCar);
    }

    void TrySpawnPerson()
    {
        var candidates = peopleSpawnPosition
            .Where(p => Vector3.Distance(p.position, player.position) <= spawnZoneRadius)
            .ToArray();

        if (candidates.Length == 0) return;

        var startPoint = candidates[Random.Range(0, candidates.Length)];

        var route = peopleSpawnPosition
            .Where(p => p != startPoint)
            .OrderBy(p => Vector3.Distance(p.position, startPoint.position))
            .Take(personWaypointVisitCount)
            .Select(p => p.position)
            .ToArray();

        if (route.Length == 0) return;

        var person = personPool.Get();
        person.transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
        person.Init(route, ReleasePerson);
    }

    void ReleaseCar(GameObject obj) => carPool.Release(obj.GetComponent<CarAgent>());
    void ReleasePerson(GameObject obj) => personPool.Release(obj.GetComponent<PersonAgent>());

    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, safeZoneRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, spawnZoneRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, despawnRadius);
    }
}