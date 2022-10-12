using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // Distances permettant d'établir la zone dans laquelle se trouve un voisin
    public float alignDistance, repulseDistance; // attractDistance n'est peut être pas nécessaire à cause du collider

    /*
     * Paramètres relatifs au mouvement individuel du boïd
     */
    // Vitesses aléatoires
    public float speedMin, speedMax;
    float speed;
    float step = 0;
    // Limites
    float axisX = 0.05f;
    float axisZ = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialisation d'une vitesse aléatoire
        speed = Random.Range(3f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    // Mouvement circulaire du boïd
    void move()
    {
        step = speed * Time.deltaTime;
        //step += speed * Time.deltaTime;

        // Mise à jour des coordonnées
        float x = Mathf.Cos(step) * axisX;
        float z = Mathf.Sin(step) * axisZ;

        // Translation
        //transform.position = new Vector3(x, 0, z);
        //transform.Translate(new Vector3(x, 0, z)); 
        transform.Translate(Vector3.forward * step);
    }

    /*
     * Méthodes correspondant aux 3 mouvements des voisins
     */

    // Attraction
    void moveCloser(GameObject other)
    {
        transform.position = Vector3.MoveTowards(transform.position, other.transform.position, step);
    }

    // Alignement
    void moveWith(GameObject other)
    {

    }

    // Répulsion
    void moveAway(GameObject other)
    {

    }

    // Distance entre ce boid et un autre
    float distance(Transform other)
    {
        return Vector3.Distance(transform.position, other.position);
    }

    // Collision entre ce boid et un autre
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            Debug.Log("Coucou !" + other.gameObject.name + ", je suis " + name);
            Debug.Log("Voyons nos distances : " + distance(other.gameObject.transform));

            // Vérification de la distance, afin de déterminer le comportement à avoir
            if(distance(other.gameObject.transform) <= repulseDistance)
            {
                Debug.Log("Répulsion !");
            }
            else if (distance(other.gameObject.transform) <= alignDistance)
            {
                Debug.Log("Alignons-nous !");
            }
            else
            {
                Debug.Log("Attends moi " + other.gameObject.name);
                moveCloser(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            // Vérification de la distance, afin de déterminer le comportement à avoir
            if (distance(other.gameObject.transform) <= repulseDistance)
            {
                Debug.Log("Répulsion !");
            }
            else if (distance(other.gameObject.transform) <= alignDistance)
            {
                Debug.Log("Alignons-nous !");
            }
            else
            {
                Debug.Log("Attends moi " + other.gameObject.name);
                moveCloser(other.gameObject);
            }
        }
    }
}
