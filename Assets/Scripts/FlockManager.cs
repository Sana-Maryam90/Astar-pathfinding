using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager Instance;
    public GameObject boidPrefab; // Assign your Boid prefab in the Inspector
    public int boidCount = 100;
    public Vector2 bounds = new Vector2(1, 1);
    public Boid1[] boids;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        boids = new Boid1[boidCount];
        for (int i = 0; i < boidCount; i++)
        {
            Boid1 newBoid = Instantiate(boidPrefab, new Vector3(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y), 0), Quaternion.identity).GetComponent<Boid1>();
            boids[i] = newBoid;
        }
    }

    //private void Update()
    //{
    //    foreach (var boid in boids)
    //    {
    //        boid.Update();
    //    }
    //}
}
