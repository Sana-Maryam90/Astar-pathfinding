using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid1 : MonoBehaviour
{
    public Vector2 velocity;
    public float maxSpeed = 0.1f;
    public float maxForce = 0.05f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 3f;
    public float separationWeight = 1f;

    private Flock1 flock;

    void Start()
    {
        flock = FindObjectOfType<Flock1>();
        velocity = Random.insideUnitCircle.normalized * maxSpeed; // Initial random velocity
    }

    void Update()
    {
        // Get the steering forces
        Vector2 alignment = flock.GetAlignment(this) * alignmentWeight;
        Vector2 cohesion = flock.GetCohesion(this) * cohesionWeight;
        Vector2 separation = flock.GetSeparation(this) * separationWeight;

        // Sum the steering behaviors
        Vector2 steering = alignment + cohesion + separation;

        // Update velocity and clamp it to the max speed
        velocity = Vector2.ClampMagnitude(velocity + steering, maxSpeed);

        // Move the boid
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Ensure the boid always faces the direction it's moving
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Keep the boids within the screen bounds
        KeepInBounds();
    }

    private void KeepInBounds()
    {
        // Get the camera bounds
        Camera camera = Camera.main;
        Vector3 screenPosition = camera.WorldToViewportPoint(transform.position);

        if (screenPosition.x < 0) screenPosition.x = 1;
        if (screenPosition.x > 1) screenPosition.x = 0;
        if (screenPosition.y < 0) screenPosition.y = 1;
        if (screenPosition.y > 1) screenPosition.y = 0;

        transform.position = camera.ViewportToWorldPoint(screenPosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
