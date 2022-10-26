using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsGenerator : MonoBehaviour
{
    // Prefab d'un boid
    public GameObject boidPrefab;

    // Vitesses minimum & maximum d'un boid
    public float speedMin, speedMax;

    // Nombre & liste de boids
    public int boidsNumber;
    public List<GameObject> boids;

    // Coordonn�es extr�mes des positions des boids
    public Vector3 minimumPosition;
    public Vector3 maximumPosition;


    // Start is called before the first frame update
    void Start()
    {
        generateRandomBoids();
    }

    // Update is called once per frame
    void Update()
    {
        updateBoidsBehaviour();
    }

    /*
     * Modification des comportements des boids
     */ 
    void updateBoidsBehaviour()
    {
        // V�rification des distances
        foreach(GameObject boid in boids)
        {
            // On passe au boid suivant si celui-ci ne peut �tre attir�
            if (!boid.GetComponent<Boid>().canBeAttracted)
                continue;

            List<GameObject> closestBoids = new List<GameObject>();
            foreach (GameObject otherBoid in boids)
            {
                if (otherBoid == boid)
                    continue;

                float singleDistance = boid.GetComponent<Boid>().distance(otherBoid.transform);
                if(singleDistance <= boid.GetComponent<Boid>().attractDistance)
                    closestBoids.Add(otherBoid);
            }

            // Changement du comportement
            boid.GetComponent<Boid>().adaptMovement(ref closestBoids);
        }
    }

    /*
     * Positionnement al�atoire des boids
     */
    void generateRandomBoids()
    {
        for(int i = 0; i < boidsNumber; i++)
        {
            // Cr�ation
            GameObject boid = Instantiate(boidPrefab, randomizePosition(), Quaternion.identity);
            // Param�trage
            boid.name = "Boid n�" + (i + 1);
            boid.GetComponent<Boid>().defaultSpeed = Random.Range(speedMin, speedMax);
            boid.GetComponent<Boid>().minimumPosition = minimumPosition;
            boid.GetComponent<Boid>().maximumPosition = maximumPosition;
            //boid.GetComponent<Boid>().boidGenerator = this;
            boids.Add(boid);
        }
    }

    /*
     * G�n�rateur al�atoire de positions
     * */

    Vector3 randomizePosition()
    {
        return new Vector3(Random.Range(minimumPosition.x, maximumPosition.x), Random.Range(minimumPosition.y, maximumPosition.y), Random.Range(minimumPosition.z, maximumPosition.z));
    }
}
