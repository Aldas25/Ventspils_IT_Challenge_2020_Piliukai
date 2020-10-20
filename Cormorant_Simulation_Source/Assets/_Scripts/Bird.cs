using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public enum BirdState {
        Baby,
        SearchingMate,
        SearchingNest,
        BuildingNest,
        DamagingTree,
        FlyingRandom
    };

    //public SpriteRenderer spriteRenderer;

    public Animator animator;

    public Vector2 speedRange;
    private float speed = 1.0f;
    public float nestBuildingTime = 2.0f;
    public float damagingTime = 1.0f;

    public GameObject birdBabyPrefab;
    public float timeToBirth;
    public float timeBeingBaby;
    public float timeLeftBeingBaby;
    
    private float timeLeftToBirth;
    public int childrenPerParentNum;
    private int leftChildren;
    private bool startedGivingBirth = false;

    private GameObject nest;
    public BirdState currentState;

    private Vector2 targetPosition;

    private bool buildingNest = false;
    private bool flownToTargetPos = false;
    private bool startedDamaginTree = false;

    private bool hasNest = true;
    //private bool chosenTargetPos = false;

    public bool hasMate = false;
    public bool givingBirth = false;
    private GameObject mate;

    public bool tempIfHasPar = false;

    private SimulationManager simulationManager;

    public bool active = true;

    private float smallOfset = 0.1f;

    void Awake () {
        simulationManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<SimulationManager> ();
    }

    void Start () {
        speed = Random.Range (speedRange.x, speedRange.y);
        damagingTime += Random.Range (-smallOfset, smallOfset);

        if (!tempIfHasPar)
            UpdateState (BirdState.SearchingMate);
        
        
    }

    void Update () {

        if (!active)
            return;

        switch(currentState) {
            case BirdState.Baby:
                DoBeingBaby (Time.deltaTime);
                break;
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
            case BirdState.SearchingMate:
                DoSearchingMate ();
                break;
            default:
                Debug.Log("Reached unkown state");
                break;
        }

        if (hasNest && hasMate && givingBirth && leftChildren > 0 && (simulationManager.leftNonDeadTrees ())) {
            timeLeftToBirth -= Time.deltaTime;
            if (timeLeftToBirth <= 0.0f) {
                startedGivingBirth = true;
                leftChildren--;
                timeLeftToBirth = timeToBirth;
                if (simulationManager.CanInstantiate ()) {
                    GameObject birdBaby = Instantiate (birdBabyPrefab, nest.transform.position, Quaternion.identity, simulationManager.birdInstantiateObject);
                    Bird birdComponent = birdBaby.GetComponent<Bird> ();
                    birdComponent.UpdateState (BirdState.Baby);
                    birdComponent.hasMate = false;
                    birdComponent.tempIfHasPar = true;
                    birdComponent.timeLeftBeingBaby = timeBeingBaby;
                }
            }
        }

    }

    public void UpdateChildrenPerParNum (int newVal) {
        childrenPerParentNum = newVal;
        if (!startedGivingBirth)
            leftChildren = childrenPerParentNum;
    }

    private void DoBeingBaby (float timePast) {
        animator.SetBool ("Fly", false);

        timeLeftBeingBaby -= timePast;
        if (timeLeftBeingBaby <= 0.0f)
            UpdateState (BirdState.SearchingMate);
    }

    private void DoSearchingNest () {
        if (nest == null)
            ChooseNest ();

        if (!hasNest) {
            UpdateState (BirdState.FlyingRandom);
            return;
        }

        FlyTowards (nest.transform.position);
        if (CloseTo (nest.transform.position))
            UpdateState (BirdState.BuildingNest);
    }

    private void DoBuildingNest () {
        if (!buildingNest)
            StartCoroutine (BuildNest ());
    }

    private void DoFlyingRandom () {
        animator.SetBool ("Fly", true);

        if (CloseTo (targetPosition))
            UpdateTargetPosition();

        FlyTowards (targetPosition);

        
        if (hasNest && flownToTargetPos && CloseTo (nest.transform.position)) {
            flownToTargetPos = false;
            UpdateState (BirdState.DamagingTree);
        }
    }

    private void DoDamagingTree () {
        if (!startedDamaginTree) 
            StartCoroutine (DamageTree ());
    }

    private void DoSearchingMate () {
        if (!hasMate) {
            FindMate ();
            if (!hasMate) {
               // targetPosition = transform.position;
                hasNest = false;
                UpdateState (BirdState.FlyingRandom);
                return;
            }
        }

    }

    private void FindMate () {
        GameObject[] birdObjects = GameObject.FindGameObjectsWithTag("Bird");

        List<GameObject> candidates = new List<GameObject> ();

        foreach (GameObject birdObject in birdObjects) {
            Bird bird = birdObject.GetComponent<Bird> ();
            if (bird != this && !bird.hasMate)
                candidates.Add(birdObject);
        }

        if (candidates.Count == 0) 
            return;

        GameObject newMate = candidates[Random.Range(0, candidates.Count)];
        
        UpdateMate (newMate);
        newMate.GetComponent<Bird> ().UpdateMate (this.gameObject, nest);
        // if not hasNest, update both birds HasNest to false

        //Debug.Log ("Mated: " + this.gameObject + " with " + newMate);
    }

    public void UpdateMate (GameObject newMate) {
        timeLeftToBirth = timeToBirth;
        leftChildren = childrenPerParentNum;
        hasMate = true;
        givingBirth = true;
        mate = newMate;
        ChooseNest ();

        if (!hasNest) {
            hasNest = false;
            UpdateState (BirdState.FlyingRandom);
            return;
        }

        UpdateState (BirdState.SearchingNest);
    }

    public void UpdateMate (GameObject newMate, GameObject newNest) {
        timeLeftToBirth = timeToBirth;
        leftChildren = childrenPerParentNum;
        hasMate = true;
        mate = newMate;
        nest = newNest;

        if (newNest == null) {
            hasNest = false;
            UpdateState (BirdState.FlyingRandom);
            return;
        }

        UpdateState (BirdState.SearchingNest);
    }

    private IEnumerator DamageTree () {
        animator.SetBool ("Fly", false);

        startedDamaginTree = true;

        do {
            yield return new WaitForSeconds (damagingTime);
        } while (!active);
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

        if (!hasNest) {
            targetPosition = GetRandomPoint ();
            return;
        }

        targetPosition = nest.transform.position;
        flownToTargetPos = true;
        //Debug.Log ("Updated target pos to nest's");
    }

    private IEnumerator BuildNest () {
        animator.SetBool ("Fly", false);

        buildingNest = true;

        do{
            yield return new WaitForSeconds (nestBuildingTime);
        } while (!active);
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

       // Debug.Log(" Updating state from " + currentState + " to " + newState);

        //Debug.Log ("Bird state updated to: " + newState);

        currentState = newState;

        switch(newState) {
            case BirdState.Baby:

                break;
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
            case BirdState.SearchingMate:
                
                break;
            default:
                Debug.Log("Reached unkown state");
                break;
        }
    }

    private void SetStateToSearchingNest () {
        //currentState = BirdState.SearchingNest;
        //spriteRenderer.color = searchingNestColor;
    }

    private void SetStateToDamagingTree () {
        //currentState = BirdState.DamagingTree;
        //spriteRenderer.color = damagingTreeColor;
    }

    private void SetStateToFlyingRandom () {
        targetPosition = GetRandomPoint ();
        flownToTargetPos = false;
    }

    private void ChooseNest () {
        GameObject[] treeObjects = GameObject.FindGameObjectsWithTag("Tree");

        if (treeObjects.Length == 0)
            return;

        GameObject chosen = null;
        foreach (GameObject treeObject in treeObjects) {
            //Debug.Log (gameObject.name + " is choosing nest. " + treeObject.name + " has nest: " + treeObject.GetComponent<Tree> ().hasNest);
            if (!treeObject.GetComponent<Tree> ().willHaveNest) {
                if (chosen == null)
                    chosen = treeObject;
                else {
                    float distToChosen = Distance (gameObject.transform.position, chosen.transform.position);
                    float distToCurTree = Distance (gameObject.transform.position, treeObject.transform.position);
                    if (distToCurTree < distToChosen)
                        chosen = treeObject; 
                }
            }
        }

        if (chosen == null) {
            hasNest = false;
            return;
        }

        nest = chosen;
        nest.GetComponent<Tree> ().willHaveNest = true;
        //Debug.Log (gameObject.name + " chose nest: " + nest.name);
    }


    private Vector2 GetRandomPoint () {
        float minBoundsX = simulationManager.xBounds.x;
        float maxBoundsX = simulationManager.xBounds.y;
        float minBoundsY = simulationManager.yBounds.x;
        float maxBoundsY = simulationManager.yBounds.y;
        return new Vector2 (
            Random.Range (minBoundsX, maxBoundsX),
            Random.Range (minBoundsY, maxBoundsY)
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
