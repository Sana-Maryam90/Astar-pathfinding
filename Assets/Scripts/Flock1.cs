using System.Collections.Generic;
using UnityEngine;

public class Flock1 : MonoBehaviour
{
    public List<Boid1> boids;
    public float visionRadius = 5f;
    public GameObject boidPrefab; // The Boid prefab


    void Start()
    {
        for (int i = 0; i < 20; i++) // Spawn 20 boids
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            Boid1 newBoid = Instantiate(boidPrefab, randomPosition, Quaternion.identity).GetComponent<Boid1>();
        
            // Set weights for each boid
            newBoid.alignmentWeight = 1f; // Adjust these values as needed
            newBoid.cohesionWeight = 1f; 
            newBoid.separationWeight = 1.5f; // Separation weight higher to keep boids apart

            boids.Add(newBoid);
        }
    }


    // Calculate the alignment force for a given boid
    public Vector2 GetAlignment(Boid1 boid)
    {
        Vector2 averageVelocity = Vector2.zero;
        int count = 0;

        foreach (var otherBoid in boids)
        {
            if (otherBoid != boid && Vector2.Distance(boid.transform.position, otherBoid.transform.position) < visionRadius)
            {
                averageVelocity += otherBoid.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            averageVelocity /= count;
            averageVelocity = averageVelocity.normalized * boid.maxSpeed;
            return (averageVelocity - boid.velocity).normalized * boid.maxForce;
        }

        return Vector2.zero;
    }

    // Calculate the cohesion force for a given boid
    public Vector2 GetCohesion(Boid1 boid)
    {
        Vector2 centerOfMass = Vector2.zero;
        int count = 0;

        foreach (var otherBoid in boids)
        {
            if (otherBoid != boid && Vector2.Distance(boid.transform.position, otherBoid.transform.position) < visionRadius)
            {
                centerOfMass += (Vector2)otherBoid.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            Vector2 desired = (centerOfMass - (Vector2)boid.transform.position).normalized * boid.maxSpeed;
            return (desired - boid.velocity).normalized * boid.maxForce;
        }

        return Vector2.zero;
    }

    // Calculate the separation force for a given boid
    public Vector2 GetSeparation(Boid1 boid)
    {
        Vector2 repulsion = Vector2.zero;
        int count = 0;

        foreach (var otherBoid in boids)
        {
            float distance = Vector2.Distance(boid.transform.position, otherBoid.transform.position);
            if (otherBoid != boid && distance < visionRadius)
            {
                repulsion += (Vector2)(boid.transform.position - otherBoid.transform.position).normalized / distance;
                count++;
            }
        }

        if (count > 0)
        {
            repulsion /= count;
            return repulsion.normalized * boid.maxForce;
        }

        return Vector2.zero;
    }
}
