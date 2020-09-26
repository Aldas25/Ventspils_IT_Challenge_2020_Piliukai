using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public enum BirdState {
        SearchingNest,
        BuildingNest,
        DamagingTree,
        FlyingRandom
    };

    public SpriteRenderer spriteRenderer;
    
    public Color searchingNestColor;
    public Color damagingTreeColor;

    public float speed = 1.0f;
    public float nestBuildingTime = 2.0f;
    public float damagingTime = 1.0f;

    private GameObject nest;
    private BirdState currentState;

    private Vector2 targetPosition;

    private bool buildingNest = false;
    private bool flownToTargetPos = false;
    private bool startedDamaginTree = false;
    //private bool chosenTargetPos = false;

    void Start () {
        UpdateState (BirdState.SearchingNest);
    }

    void Update () {

        switch(currentState) {
            case BirdState.SearchingNest:
                DoSearchingNest ();
                break;
            case BirdState.BuildingNest:
                DoBuildingNest ();
                break;
            case BirdState.DamagingTree:
                DoDamagingTree ();
                break;
            case BirdState.FlyingRandom:
                DoFlyingRandom ();
                break;
            default:
                Debug.Log("Reached unkown state");
                break;
        }

    }

    private void DoSearchingNest () {
        if (nest == null)
            ChooseNest ();
        FlyTowards (nest.transform.position);
        if (CloseTo (nest.transform.position))
            UpdateState (BirdState.BuildingNest);
    }

    private void DoBuildingNest () {
        if (!buildingNest)
            StartCoroutine (BuildNest ());
    }

    private void DoFlyingRandom () {
        if (CloseTo (targetPosition))
            UpdateTargetPosition();

        FlyTowards (targetPosition);

        
        if (flownToTargetPos && CloseTo (nest.transform.position)) {
            flownToTargetPos = false;
            UpdateState (BirdState.DamagingTree);
        }
    }

    private void DoDamagingTree () {
        if (!startedDamaginTree) 
            StartCoroutine (DamageTree ());
    }

    private IEnumerator DamageTree () {
        startedDamaginTree = true;

        yield return new WaitForSeconds (damagingTime);
        nest.GetComponent<Tree> ().Damage ();

        UpdateState (BirdState.FlyingRandom);

        startedDamaginTree = false;
    }

    private void UpdateTargetPosition () {
       /* Debug.Log ("updating target position");
        if (!chosenTargetPos && CloseTo (nest.transform.position)) {
            targetPosition = GetRandomPoint ();
            flownToTargetPos = false;
            chosenTargetPos = true;
            Debug.Log ("Updated target pos to random: " + targetPosition);
        } else {
            targetPosition = nest.transform.position;
            flownToTargetPos = true;
            Debug.Log ("Updated target pos to nest's");
        }*/
        targetPosition = nest.transform.position;
        flownToTargetPos = true;
        //Debug.Log ("Updated target pos to nest's");
    }

    private IEnumerator BuildNest () {
        buildingNest = true;

        yield return new WaitForSeconds (nestBuildingTime);
        nest.GetComponent<Tree> ().AddNest ();

        UpdateState (BirdState.FlyingRandom);

        buildingNest = false;
    }

    private void FlyTowards (Vector2 target) {
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, target, step);
    }

    private void UpdateState (BirdState newState) {
        if (currentState == newState)
            return;

        //Debug.Log ("Bird state updated to: " + newState);

        currentState = newState;

        switch(newState) {
            case BirdState.SearchingNest:
                SetStateToSearchingNest();
                break;
            case BirdState.BuildingNest:

                break;
            case BirdState.DamagingTree:
                SetStateToDamagingTree();
                break;
            case BirdState.FlyingRandom:
                SetStateToFlyingRandom();
                break;
            default:
                Debug.Log("Reached unkown state");
                break;
        }
    }

    private void SetStateToSearchingNest () {
        //currentState = BirdState.SearchingNest;
        spriteRenderer.color = searchingNestColor;
    }

    private void SetStateToDamagingTree () {
        //currentState = BirdState.DamagingTree;
        spriteRenderer.color = damagingTreeColor;
    }

    private void SetStateToFlyingRandom () {
        targetPosition = GetRandomPoint ();
        flownToTargetPos = false;
    }

    private void ChooseNest () {
        GameObject[] treeObjects = GameObject.FindGameObjectsWithTag("Tree");

        // TODO: What if there are no (healthy) trees left?????

        GameObject chosen = treeObjects[0];
        foreach (GameObject treeObject in treeObjects) {
            float distToChosen = Distance (gameObject.transform.position, chosen.transform.position);
            float distToCurTree = Distance (gameObject.transform.position, treeObject.transform.position);
            if (distToCurTree < distToChosen)
                chosen = treeObject; 
        }

        nest = chosen;
    }


    private Vector2 GetRandomPoint () {
        return new Vector2 (
            Random.Range (-8.0f, 8.0f),
            Random.Range (-4.0f, 4.0f)
        );
    }

    private bool CloseTo (Vector2 pos) {
        return (Distance (transform.position, pos) < 0.001f);
    }

    private float Distance (Vector2 pos1, Vector2 pos2) {
        float dist = (pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y);
        return dist;
    }

}
