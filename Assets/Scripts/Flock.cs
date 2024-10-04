//using UnityEngine;

//public class Flock : MonoBehaviour
//{
//    public float speed = 0.5f;
//    float rotationSpeed = 4.0f;
//    Vector3 averageHeading;
//    Vector3 averagePosition;
//    float neighbourDistance = 3.0f;

//    bool turning = false;
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        speed = Random.Range(0.5f, 1);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.tanksize)
//        {
//            turning = true;
//        }
//        else
//        {
//            turning = false;
//        }
//        if (turning)
//        {
//            Vector3 direction = Vector3.zero - transform.position;
//            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
//            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
//            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);

//            speed = Random.Range(0.5f, 1);
//        }
//        else
//        {
//            if (Random.Range(0, 5) < 1) ApplyRules();
//        }
//        //transform.Translate(0, Time.deltaTime * speed, 0); 
//        transform.Translate(Vector3.up * Time.deltaTime * speed);

//    }

//    void ApplyRules()
//    {
//        GameObject[] gos;
//        gos = GlobalFlock.allFish;

//        Vector3 vcentre = Vector3.zero;
//        Vector3 vavoid = Vector3.zero;
//        float gSpeed = 0.1f;

//        Vector3 goalPos = GlobalFlock.goalPos;
//        float dist;

//        int groupSize = 0;
//        foreach(GameObject go in gos)
//        {
//            if(go != this.gameObject)
//            {
//                dist = Vector3.Distance(go.transform.position, this.transform.position);
//                if(dist <= neighbourDistance)
//                {
//                    vcentre += go.transform.position;
//                    groupSize++;

//                    if(dist < 1.0f)
//                    {
//                        vavoid = vavoid + (this.transform.position - go.transform.position);
//                    }

//                    Flock anotherFlock = go.GetComponent<Flock>();
//                    gSpeed = gSpeed + anotherFlock.speed;
//                }
//            }
//        }

//        if(groupSize > 0)
//        {
//            vcentre = vcentre / groupSize + (goalPos - this.transform.position);
//            speed = gSpeed / groupSize;

//            Vector3 direction = (vcentre + vavoid) - transform.position;
//            if(direction != Vector3.zero)
//            {
//                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
//                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
//                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);

//            }
//        }
//    }
//}




using UnityEngine;

public class Flock : MonoBehaviour
{
    public float speed = 0.5f;
    private float rotationSpeed = 4.0f;
    private float neighbourDistance = 3.0f;

    public float separationWeight = 1.5f; 
    public float cohesionWeight = 1.0f;   
    public float alignmentWeight = 1.0f;  

    private bool turning = false;

    void Start()
    {
        speed = Random.Range(0.5f, 1);
    }

    void Update()
    {
        CheckBounds();

        if (!turning && Random.Range(0, 5) < 1)
        {
            ApplyFlockingRules();
        }

        MoveFish();
    }

    void CheckBounds()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.tanksize)
        {
            turning = true;
            TurnBackToCenter();
        }
        else
        {
            turning = false;
        }
    }

    void TurnBackToCenter()
    {
        Vector3 direction = Vector3.zero - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);
        speed = Random.Range(0.5f, 1);
    }

    void MoveFish()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    void ApplyFlockingRules()
    {
        Vector3 center = CalculateCenter() * cohesionWeight; // Apply cohesion weight
        Vector3 avoidance = CalculateAvoidance() * separationWeight; // Apply separation weight
        float averageSpeed = CalculateSpeed() * alignmentWeight; // Apply alignment weight

        if (center != Vector3.zero)
        {
            Vector3 direction = (center + avoidance) - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);
            }
        }

        speed = averageSpeed;
    }

    Vector3 CalculateCenter()
    {
        Vector3 vcenter = Vector3.zero;
        GameObject[] fishes = GlobalFlock.allFish;
        int groupSize = 0;

        foreach (GameObject go in fishes)
        {
            if (go != this.gameObject)
            {
                float dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;
                }
            }
        }

        if (groupSize > 0)
        {
            vcenter /= groupSize;
            vcenter += (GlobalFlock.goalPos - this.transform.position);
        }

        return vcenter;
    }

    Vector3 CalculateAvoidance()
    {
        Vector3 vavoid = Vector3.zero;
        GameObject[] fishes = GlobalFlock.allFish;

        foreach (GameObject go in fishes)
        {
            if (go != this.gameObject)
            {
                float dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist < 1.0f)
                {
                    vavoid += (this.transform.position - go.transform.position);
                }
            }
        }

        return vavoid;
    }

    float CalculateSpeed()
    {
        float totalSpeed = 0.1f;
        GameObject[] fishes = GlobalFlock.allFish;
        int groupSize = 0;

        foreach (GameObject go in fishes)
        {
            if (go != this.gameObject)
            {
                float dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    Flock anotherFlock = go.GetComponent<Flock>();
                    totalSpeed += anotherFlock.speed;
                    groupSize++;
                }
            }
        }

        if (groupSize > 0)
        {
            return totalSpeed / groupSize;
        }
        return totalSpeed;
    }
}


