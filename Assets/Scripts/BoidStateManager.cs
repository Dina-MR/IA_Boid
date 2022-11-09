using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidStateManager : MonoBehaviour
{
    // Etat du boid
    State state = State.ROAMING;
    // Entit� par laquelle le boid est attir�/repouss� (autre qu'un boid)
    GameObject otherEntity;
    // Liste des tags des objets les plus int�ressants
    List<string> tagList = new List<string>() { "Player", "DeadTree", "Cloud" };
    // Temps pour le d�placement
    float time = 0;
    // Param�tres de distance pour le d�placement
    float radius;
    float height;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        adaptBehaviour();
    }

    // Modification de l'�tat
    void setState(State _state)
    {
        state = _state;
    }

    // Modification de l'entit� "cible" courante
    void setOtherEntity(GameObject _otherEntity)
    {
        otherEntity = _otherEntity;
    }

    // Mise � jour du comportement du boid, selon son �tat
    void adaptBehaviour()
    {
        switch(state)
        {
            case State.EVADING:
                break;
            case State.FOLLOWING_PLAYER:
                diveTowardTarget(otherEntity);
                rotateTowardTarget(otherEntity);
                break;
            case State.FOLLOWING_TREE:
                gameObject.GetComponent<Boid>().immuneToOthers = true;
                diveTowardTarget(otherEntity);
                setState(State.STUCK_ON_TREE);
                break;
            case State.STUCK_ON_TREE:
                rotateTowardTarget(otherEntity);
                break;
            default:
                break;
        }
    }

    // Mise � jour des informations sur l'entit� cible et les param�tres de positionnement
    void updateData(GameObject _otherEntity, State newState, bool coverAllEntity)
    {
        setState(newState);
        setOtherEntity(_otherEntity);
        height = otherEntity.GetComponent<CapsuleCollider>().height;
        // Dans le cas de l'arbre, la hauteure minimale du boid est la moiti� de sa hauteur
        if(!coverAllEntity)
            height = Random.Range(height / 2, height);
        // Dans le cas du joueur, le boid peut avoir une hauteur allant du bas jusqu'au haut de ce dernier
        else
            height = Random.Range(0, height);
        radius = otherEntity.GetComponent<CapsuleCollider>().radius;

    }

    // Mise � jour de l'�tat, en fonction des entit�s � proximit�s
    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.gameObject.tag;
        if (tagList.Contains(otherTag))
        {
            switch (otherTag)
            {
                // Collision avec le joueur
                case "Player":
                    Debug.Log("Joueur detect�");
                    if(state != State.STUCK_ON_TREE)
                        updateData(other.gameObject, State.FOLLOWING_PLAYER, true);
                    break;
                // Collision avec un arbre sans feuilles
                case "DeadTree":
                    Debug.Log("Arbre sans feuille d�tect�");
                    if (state == State.FOLLOWING_PLAYER)
                        updateData(other.gameObject, State.FOLLOWING_TREE, false);
                    break;
                // Collision avec un nuage
                default:
                    Debug.Log("Nuage d�tect�");
                    setState(State.EVADING);
                    setOtherEntity(other.gameObject);
                    avoidTarget(otherEntity);
                    break;
            }
        }
    }

    /*
     * ENSEMBLES DES COMPORTEMENTS DU BOID SELON SON ETAT
     */

    // Rotation circulaire autour d'une cible
    void rotateTowardTarget(GameObject target)
    {
        Vector3 targetCenter = target.transform.position;
        float newX = targetCenter.x + Mathf.Cos(time) * radius;
        float newZ = targetCenter.z + Mathf.Sin(time) * radius;
        transform.position = new Vector3(newX, height, newZ);
    }

    // Plong�e vers une cible
    void diveTowardTarget(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 newPosition = new Vector3(targetPosition.x + radius, targetPosition.y + radius, targetPosition.z + radius);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, time);
    }

    // Esquive d'une cible
    void avoidTarget(GameObject target)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 20f, 10))
        {
            Vector3 normale = hit.normal;
            normale.z = 0f; // On fige temporairement la position sur l'axe z
            Vector3 targetPosition = transform.position + normale * 2f;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, time);
        }
    }

    /*
     * LISTE DES ETATS DU BOID :
     * - ROAMING (�tat initial) : il fait sa vie,
     * - EVADING : il fuit l'obstacle,
     * - FOLLOWING_PLAYER : il est attir� par le jeu,
     * - FOLLOWING_TREE : il se dirige vers un arbre,
     * - STUCK_ON_TREE : il reste sur un arbre.
     */
    enum State
    {
        ROAMING,
        EVADING,
        FOLLOWING_PLAYER,
        FOLLOWING_TREE,
        STUCK_ON_TREE,
    }
}
