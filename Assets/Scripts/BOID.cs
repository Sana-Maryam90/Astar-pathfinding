using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BOID : MonoBehaviour
{
    public float maxSpeed = 5f;               // Maximum speed of the BOID
    public float maxForce = 0.5f;             // Maximum steering force
    public float neighborDistance = 3f;        // Distance to consider other BOIDs
    public float separationDistance = 1f;      // Distance to maintain from other BOIDs

    private Vector2 velocity;                  // Current velocity of the BOID
    // public Tilemap tilemap;

    void Start()
    {
        // Randomly initialize the velocity
        velocity = Random.insideUnitCircle * maxSpeed;
    }

    void Update()
    {
        // Get all BOID components in the scene
        BOID[] allBOIDs = FindObjectsOfType<BOID>();

        // Calculate the new velocity based on flocking behavior
        Vector2 acceleration = Vector2.zero;

        // Calculate flocking behaviors
        Vector2 separation = Separate(allBOIDs);
        Vector2 alignment = Align(allBOIDs);
        Vector2 cohesion = Cohere(allBOIDs);

        // Apply the forces
        acceleration += separation;
        acceleration += alignment;
        acceleration += cohesion;

        // Update velocity and position
        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Optional: Keep within certain bounds
        StayWithinBounds();
    }

    Vector2 Separate(BOID[] BOIDs)
    {
        Vector2 steer = Vector2.zero;
        int count = 0;

        // Iterate through all BOIDs
        foreach (BOID b in BOIDs)
        {
            if (b != this) // Avoid self
            {
                float distance = Vector2.Distance(transform.position, b.transform.position);
                if (distance < separationDistance)
                {
                    Vector2 diff = (Vector2)transform.position - (Vector2)b.transform.position;
                    diff.Normalize();
                    steer += diff / distance; // Weight by distance
                    count++;
                }
            }
        }

        // Average out the steering force
        if (count > 0)
        {
            steer /= count;
            steer.Normalize();
            steer *= maxSpeed;
            steer -= velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce);
        }

        return steer;
    }

    Vector2 Align(BOID[] BOIDs)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        // Calculate average velocity of neighbors
        foreach (BOID b in BOIDs)
        {
            if (b != this)
            {
                float distance = Vector2.Distance(transform.position, b.transform.position);
                if (distance < neighborDistance)
                {
                    sum += (Vector2)b.velocity;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= maxSpeed;
            Vector2 steer = sum - velocity;
            return Vector2.ClampMagnitude(steer, maxForce);
        }

        return Vector2.zero;
    }

    Vector2 Cohere(BOID[] BOIDs)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        // Calculate average position of neighbors
        foreach (BOID b in BOIDs)
        {
            if (b != this)
            {
                float distance = Vector2.Distance(transform.position, b.transform.position);
                if (distance < neighborDistance)
                {
                    sum += (Vector2)b.transform.position;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            sum /= count;
            return Seek(sum);
        }

        return Vector2.zero;
    }

    Vector2 Seek(Vector2 target)
    {
        Vector2 desired = target - (Vector2)transform.position;
        desired.Normalize();
        desired *= maxSpeed;
        Vector2 steer = desired - velocity;
        return Vector2.ClampMagnitude(steer, maxForce);
    }

    void StayWithinBounds()
    {
        Vector3 position = transform.position;

        // Define your tilemap bounds (manually setting them based on grid size)
        float minX = 0;  // Bottom-left corner X
        float maxX = 18; // Number of columns
        float minY = 0;  // Bottom-left corner Y
        float maxY = 10; // Number of rows

        // Clamp the position within the bounds of the tilemap
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        // Apply the clamped position
        transform.position = position;
    }

}
