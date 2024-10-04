//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Boid1 : MonoBehaviour
//{
//    public Vector2 velocity;
//    public float maxSpeed = 0.1f;
//    public float maxForce = 0.05f;
//    public float alignmentWeight = 1f;
//    public float cohesionWeight = 3f;
//    public float separationWeight = 1f;

//    private Flock1 flock;

//    void Start()
//    {
//        flock = FindObjectOfType<Flock1>();
//        velocity = Random.insideUnitCircle.normalized * maxSpeed; // Initial random velocity
//    }

//    void Update()
//    {
//        // Get the steering forces
//        Vector2 alignment = flock.GetAlignment(this) * alignmentWeight;
//        Vector2 cohesion = flock.GetCohesion(this) * cohesionWeight;
//        Vector2 separation = flock.GetSeparation(this) * separationWeight;

//        // Sum the steering behaviors
//        Vector2 steering = alignment + cohesion + separation;

//        // Update velocity and clamp it to the max speed
//        velocity = Vector2.ClampMagnitude(velocity + steering, maxSpeed);

//        // Move the boid
//        transform.position += (Vector3)velocity * Time.deltaTime;

//        // Ensure the boid always faces the direction it's moving
//        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
//        transform.rotation = Quaternion.Euler(0, 0, angle);

//        // Keep the boids within the screen bounds
//        KeepInBounds();
//    }

//    private void KeepInBounds()
//    {
//        // Get the camera bounds
//        Camera camera = Camera.main;
//        Vector3 screenPosition = camera.WorldToViewportPoint(transform.position);

//        if (screenPosition.x < 0) screenPosition.x = 1;
//        if (screenPosition.x > 1) screenPosition.x = 0;
//        if (screenPosition.y < 0) screenPosition.y = 1;
//        if (screenPosition.y > 1) screenPosition.y = 0;

//        transform.position = camera.ViewportToWorldPoint(screenPosition);
//        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid1 : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 acceleration;
    public float maxSpeed = 5f;
    public float maxForce = 0.2f;
    public float alignmentRadius = 25f; // radius for alignment
    public float cohesionRadius = 50f; // radius for cohesion
    public float separationRadius = 24f; // radius for separation

    void Start()
    {
        // Randomize initial velocity
        velocity = Random.insideUnitSphere * Random.Range(2f, 4f);
    }

    void Update()
    {
        // Update behavior
        Vector3 alignment = Align(FlockManager.Instance.boids);
        Vector3 cohesion = Cohere(FlockManager.Instance.boids);
        Vector3 separation = Separate(FlockManager.Instance.boids);

        // Combine the steering forces
        acceleration += alignment + cohesion + separation;

        // Update velocity and position
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        // Reset acceleration for the next frame
        acceleration = Vector3.zero;

        // Handle edge wrapping
        WrapAround();
    }

    private Vector3 Align(Boid1[] boids)
    {
        Vector3 steering = Vector3.zero;
        int total = 0;

        foreach (var other in boids)
        {
            if (other != this)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance < alignmentRadius)
                {
                    steering += other.velocity;
                    total++;
                }
            }
        }

        if (total > 0)
        {
            steering /= total; // Average
            steering = Vector3.ClampMagnitude(steering, maxSpeed);
            steering -= velocity; // Desired - current
            steering = Vector3.ClampMagnitude(steering, maxForce);
        }

        return steering;
    }

    private Vector3 Cohere(Boid1[] boids)
    {
        Vector3 steering = Vector3.zero;
        int total = 0;

        foreach (var other in boids)
        {
            if (other != this)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance < cohesionRadius)
                {
                    steering += other.transform.position;
                    total++;
                }
            }
        }

        if (total > 0)
        {
            steering /= total; // Average
            steering -= transform.position; // Desired - current
            steering = Vector3.ClampMagnitude(steering, maxSpeed);
            steering -= velocity; // Desired - current
            steering = Vector3.ClampMagnitude(steering, maxForce);
        }

        return steering;
    }

    private Vector3 Separate(Boid1[] boids)
    {
        Vector3 steering = Vector3.zero;
        int total = 0;

        foreach (var other in boids)
        {
            if (other != this)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance < separationRadius)
                {
                    Vector3 diff = transform.position - other.transform.position; // Vector away from the other boid
                    diff /= distance * distance; // Weight by distance
                    steering += diff;
                    total++;
                }
            }
        }

        if (total > 0)
        {
            steering /= total; // Average
            steering = Vector3.ClampMagnitude(steering, maxSpeed);
            steering -= velocity; // Desired - current
            steering = Vector3.ClampMagnitude(steering, maxForce);
        }

        return steering;
    }

    private void WrapAround()
    {
        // Wrap around the edges
        if (transform.position.x > FlockManager.Instance.bounds.x)
        {
            transform.position = new Vector3(-FlockManager.Instance.bounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -FlockManager.Instance.bounds.x)
        {
            transform.position = new Vector3(FlockManager.Instance.bounds.x, transform.position.y, transform.position.z);
        }

        if (transform.position.y > FlockManager.Instance.bounds.y)
        {
            transform.position = new Vector3(transform.position.x, -FlockManager.Instance.bounds.y, transform.position.z);
        }
        else if (transform.position.y < -FlockManager.Instance.bounds.y)
        {
            transform.position = new Vector3(transform.position.x, FlockManager.Instance.bounds.y, transform.position.z);
        }
    }
}

