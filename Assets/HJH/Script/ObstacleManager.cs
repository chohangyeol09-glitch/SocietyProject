using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ObstacleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] peopleSpawnPosition;

    [Header("Car Waypoint")]
    [SerializeField] private Transform carWaypointsParent;
    [SerializeField] private Transform carSpawnPointsParent;

    private Transform[] carWaypoints;
    private Transform[] carSpawnPosition;

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

    [Header("Car Cap")]
    [SerializeField] private int maxCarCount = 6;
    [SerializeField] private float forceReplaceDelay = 3f;

    private ObjectPool<CarAgent> carPool;
    private ObjectPool<PersonAgent> personPool;

    private List<CarAgent> activeCars = new List<CarAgent>();
    private bool isReplacingCar = false;

    void Awake()
    {
        carWaypoints = carWaypointsParent
            .Cast<Transform>()
            .OrderBy(t => t.GetSiblingIndex())
            .ToArray();

        carSpawnPosition = carSpawnPointsParent
            .Cast<Transform>()
            .OrderBy(t => t.GetSiblingIndex())
            .ToArray();

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

            if (activeCars.Count < maxCarCount)
            {
                TrySpawnCar();
            }
            else if (!isReplacingCar)
            {
                StartCoroutine(ForceReplaceFarthestCar());
            }
        }
    }

    IEnumerator ForceReplaceFarthestCar()
    {
        isReplacingCar = true;
        yield return new WaitForSeconds(forceReplaceDelay);

        if (activeCars.Count >= maxCarCount)
        {
            var farthest = activeCars
                .OrderByDescending(c => Vector3.Distance(c.transform.position, player.position))
                .FirstOrDefault();

            if (farthest != null)
                RemoveCar(farthest);

            TrySpawnCar();
        }

        isReplacingCar = false;
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
        if (activeCars.Count >= maxCarCount) return;

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

        activeCars.Add(car);
    }

    void RemoveCar(CarAgent car)
    {
        activeCars.Remove(car);
        carPool.Release(car);
    }

    void ReleaseCar(GameObject obj)
    {
        var car = obj.GetComponent<CarAgent>();
        activeCars.Remove(car);
        carPool.Release(car);
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

        if (carWaypoints != null && carWaypoints.Length >= 2)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < carWaypoints.Length; i++)
            {
                var a = carWaypoints[i].position;
                var b = carWaypoints[(i + 1) % carWaypoints.Length].position;
                Gizmos.DrawLine(a, b);
            }
        }
    }
}