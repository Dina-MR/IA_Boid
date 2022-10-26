using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // Distances permettant d'�tablir la zone dans laquelle se trouve un voisin
    public float attractDistance, alignDistance, repulseDistance;
    // Etat du boid
    public bool canBeAttracted = true;
    public bool canBeRepulsed = true;

    /*
     * Param�tres relatifs au mouvement individuel du bo�d
     */
    // Vitesses al�atoires
    public float defaultSpeed; // vitesse par d�faut, invariable
    public float speed; // vitesse variable
    float step = 0;
    // Limites
    float axisX = 0.05f;
    float axisZ = 0.05f;
    // Positions extr�mes, r�cup�r�es � partir du parent
    public Vector3 minimumPosition;
    public Vector3 maximumPosition;
    // Translation, pour les demi-tours
    Vector3 translation = Vector3.forward;

    // Acc�s au contr�leur
    //public BoidsGenerator boidGenerator;

    // Suivi ou non du joueur
    bool isFollowingPlayer = false;

    /*
     * Positions extr�mes, pour le mouvement
     */

    // Etat du boid

    // Start is called before the first frame update
    void Start()
    {
        // Initialisation d'une vitesse al�atoire
        speed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //detectDistances(boidGenerator);
        move();
    }

    // Mouvement circulaire du bo�d
    void move()
    {
        step = speed * Time.deltaTime;
        //step += speed * Time.deltaTime;

        // Mise � jour des coordonn�es
        float x = Mathf.Cos(step) * axisX;
        float z = Mathf.Sin(step) * axisZ;

        // Translation
        //transform.position = new Vector3(x, 0, z);
        //transform.Translate(new Vector3(x, 0, z));
        // Demi-tour lorsque le boid atteint les limites du terrain
        if(transform.position.z <= minimumPosition.z)
        {
            //transform.RotateAround(transform.position, Vector3.forward, step);
            translation = Vector3.forward;
            transform.Rotate(new Vector3(0, 10, 0) * Time.deltaTime);
        }
        if(transform.position.z >= maximumPosition.z)
        {
            //transform.RotateAround(transform.position, Vector3.back, step);
            translation = Vector3.back;
            transform.Rotate(new Vector3(0, 10, 0) * Time.deltaTime);
        }
        transform.Translate(translation * step);
    }

    /*
     * M�thodes correspondant aux 3 mouvements des voisins
     */

    // Attraction
    void moveCloser(GameObject other)
    {
        transform.position = Vector3.MoveTowards(transform.position, other.transform.position, step);
    }

    void moveCloserAll(List<GameObject> neighbours)
    {
        int neighboursCount = neighbours.Count;
        if (neighboursCount > 0)
        {
            // Calcul de la vitesse moyenne des voisins
            float averageSpeed = 0;
            foreach (GameObject neighbour in neighbours)
            {
                transform.position = Vector3.MoveTowards(transform.position, neighbour.transform.position, step);
            }
        }
    }

    // Alignement
    void moveWith(GameObject other)
    {
        //Debug.Log("SPEED" + other.GetComponent<Boid>().speed);
        //speed = other.GetComponent<Boid>().speed;
    }

    void moveWithAll(List<GameObject> neighbours, float speedLimiter)
    {
        int neighboursCount = neighbours.Count;
        if (neighboursCount > 0)
        {
            // Calcul de la vitesse moyenne des voisins
            float averageSpeed = 0;
            foreach(GameObject neighbour in neighbours)
            {
                averageSpeed += neighbour.GetComponent<Boid>().speed;
            }
            averageSpeed /= neighboursCount;
            // Adaptation de notre vitesse
            speed += averageSpeed / speedLimiter;
        }
    }

    // R�pulsion
    void moveAway(GameObject other)
    {

    }

    void moveAwayFromAll(List<GameObject> neighbours, float distanceCoefficient)
    {
        int neighboursCount = neighbours.Count;
        if (neighboursCount > 0)
        {
            //Vector3 newDistance = Vector3.zero;
            Vector3 positionDifference = Vector3.zero;
            foreach (GameObject neighbour in neighbours)
            {
                positionDifference += transform.position - neighbour.transform.position;
                //positionDifference.x = positionDifference.x >= 0 ? Mathf.Sqrt(repulseDistance) - positionDifference.x : -Mathf.Sqrt(repulseDistance) - positionDifference.x;
                //positionDifference.y = positionDifference.y >= 0 ? Mathf.Sqrt(repulseDistance) - positionDifference.y : -Mathf.Sqrt(repulseDistance) - positionDifference.y;
                //positionDifference.z = positionDifference.z >= 0 ? Mathf.Sqrt(repulseDistance) - positionDifference.z : -Mathf.Sqrt(repulseDistance) - positionDifference.z;
                //newDistance = positionDifference;
            }
            positionDifference /= neighboursCount;
            //positionDifference = -positionDifference.normalized;
            transform.Translate(new Vector3(positionDifference.x, 0, positionDifference.z) * 2f);
        }
    }

    // Distance entre ce boid et un autre
    public float distance(Transform other)
    {
        return Vector3.Distance(transform.position, other.position);
    }

    /*
     * 1ERE VERSION - AVEC COLLIDER
     */

    // Collision entre ce boid et le joueur
    private void OnTriggerEnter(Collider other)
    {
        //Collision avec le joueur
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Coucou joueur ! Je suis le bo�d n� " + name);
            moveCloser(other.gameObject);
        }
    }

    /*
     * 2EME VERSION - AVEC LA LISTE DES BOIDS 
     */
    //void detectDistances(BoidsGenerator boidsGenerator)
    //{
    //    foreach(GameObject boid in boidsGenerator.boids)
    //    {
    //        if (boid == this)
    //            continue;

    //        float newDistance = distance(boid.transform);
    //        adaptMovement(newDistance, boid);
    //    }
    //}

    // Adaptation du mouvement selon la distance
    public void adaptMovement(ref List<GameObject> neighbours)
    {
        Debug.Log("Nombre de voisins " + neighbours.Count);

        // Liste des voisins par cat�gorie
        List<GameObject> attractList = neighbours;
        List<GameObject> alignList = new List<GameObject>();
        List<GameObject> repulseList = new List<GameObject>();

        foreach(GameObject neighbour in neighbours)
        {
            float distanceWithNeighbour = distance(neighbour.transform);
            // V�rification de la distance, afin de d�terminer le comportement � avoir
            if (distanceWithNeighbour <= repulseDistance && canBeRepulsed)
            {
                Debug.Log("R�pulsion !");
                repulseList.Add(neighbour);
                //canBeRepulsed = false;
                canBeAttracted = true;
            }
            if (distanceWithNeighbour <= alignDistance)
            {
                Debug.Log("Alignons-nous !");
                alignList.Add(neighbour);
                //moveWith(neighbour);
                canBeRepulsed = true;
                //canBeAttracted = false;
            }
        }

        //Mise � jour des mouvements
        moveCloserAll(attractList); // attraction
        moveWithAll(alignList, 1000); // alignement
        moveAwayFromAll(repulseList, 1000); // r�pulsion
    }
}
