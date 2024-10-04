using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject fish;
    public GameObject goal;
    private GameObject g;
    public static int tanksize = 5;
    static int numFish = 10;
    public static GameObject[] allFish = new GameObject[numFish];
    public static Vector3 goalPos = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tanksize, tanksize),
                                      Random.Range(-tanksize, tanksize), 0);
            allFish[i] = (GameObject) Instantiate(fish, pos, Quaternion.identity);                          
        }
        g = Instantiate(goal, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0, 10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-tanksize, tanksize),
                                  Random.Range(-tanksize, tanksize), 0);
        }

        g.transform.position = goalPos;
    }
}
