using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject birdPrefab;
    public GameObject treePrefab;
    public Transform birdInstantiateObject;
    public Transform treeInstantiateObject;

    public Vector2 xBounds;
    public Vector2 yBounds;
    public float offset;

    public int treeCount;
    public int birdCount;
    public int birdLimit = 200;

    public Graph healthyTreeGraph;
    public Graph damagedTreeGraph;
    public Graph deadTreeGraph;
    public Graph birdBabyGraph;
    public Graph birdAdultGraph;
    public float graphPlottingTime = 0.1f;
    private float simulationTime = 0.0f;
    private float leftToPlot = 0.0f;

    private bool simulationStarted = false;

    private UIManager uiManager;

    private float smallOfset = 0.1f;

    public float maxSimTime = 60.0f;

    public GameObject playButton;
    public GameObject pauseButton;
    public GameObject restartButton;

    void Awake () {
        uiManager = gameObject.GetComponent<UIManager> ();
    }

    void Start () {
        GenerateField ();
        RestartSimulation ();

        birdAdultGraph.maxY = birdLimit;
        birdBabyGraph.maxY = birdLimit;

        /*playButton.SetActive (true);
        pauseButton.SetActive (false);
        restartButton.SetActive (false);*/
    }

    void Update () {
        if (simulationStarted) {
            simulationTime += Time.deltaTime;
            uiManager.UpdateTimeText (simulationTime);
            leftToPlot -= Time.deltaTime;
            if (leftToPlot <= 0.0f) {
                leftToPlot = graphPlottingTime;
                healthyTreeGraph.AddDot (simulationTime, (float)CountHealthyTrees ());
                damagedTreeGraph.AddDot (simulationTime, (float)CountDamagedTrees ());
                deadTreeGraph.AddDot (simulationTime, (float)CountDeadTrees ());
                birdBabyGraph.AddDot (simulationTime, (float)CountBabyBirds ());
                birdAdultGraph.AddDot (simulationTime, (float)CountAdultBirds ());
            }

            if (simulationTime >= maxSimTime) {
                //StopSimulation ();
                //uiManager.ChangeSimTextToStart ();
                //playButton.SetActive (false);
                //pauseButton.SetActive (false);
                //restartButton.SetActive (true);
                StopSimulation ();
                uiManager.ChangeSimTextToRestart ();
            }
        }
    }

    public bool isSimulationStarted () {
        return simulationStarted;
    }

    public void GenerateField () {
        GenerateTrees ();
        GenerateBirds ();

        simulationTime = 0.0f;
        uiManager.UpdateTimeText (simulationTime);
        healthyTreeGraph.ClearGraph ();
        damagedTreeGraph.ClearGraph ();
        deadTreeGraph.ClearGraph ();
        birdBabyGraph.ClearGraph ();
        birdAdultGraph.ClearGraph ();
    }

    private void GenerateTrees () {
        foreach (Transform child in treeInstantiateObject) {
            GameObject.Destroy(child.gameObject);
        }
        
        List<Vector2> treeCoordinates = GenerateCoordinates (treeCount);
        
        foreach (Vector2 pos in treeCoordinates) {
            Instantiate (treePrefab, pos, Quaternion.identity, treeInstantiateObject);
        }
    }

    private void GenerateBirds () {
        foreach (Transform child in birdInstantiateObject) {
            GameObject.Destroy(child.gameObject);
        }

        List<Vector2> birdCoordinates = GenerateCoordinates (birdCount);

        foreach (Vector2 pos in birdCoordinates) {
            GameObject bird = Instantiate (birdPrefab, pos, Quaternion.identity, birdInstantiateObject);
            bird.GetComponent<Bird> ().active = false;
        }
    }

    public bool CanInstantiate () {
        return (CountBirds () < birdLimit);
    }

    public bool leftNonDeadTrees () {
        return CountTrees () != CountDeadTrees ();
    }

    private int CountTrees () {
        return treeInstantiateObject.childCount;
    }

    private int CountHealthyTrees () {
        int cnt = 0;
        foreach (Transform child in treeInstantiateObject) {
            Tree tree = child.gameObject.GetComponent<Tree> ();
            if (tree.currentState == Tree.TreeState.Healthy)
                cnt++;
        }
        return cnt;
    }

    private int CountDamagedTrees () {
        int cnt = 0;
        foreach (Transform child in treeInstantiateObject) {
            Tree tree = child.gameObject.GetComponent<Tree> ();
            if (tree.currentState == Tree.TreeState.Damaged)
                cnt++;
        }
        return cnt;
    }

    private int CountDeadTrees () {
        int cnt = 0;
        foreach (Transform child in treeInstantiateObject) {
            Tree tree = child.gameObject.GetComponent<Tree> ();
            if (tree.currentState == Tree.TreeState.Dead)
                cnt++;
        }
        return cnt;
    }

    private int CountAdultBirds () {
        return CountBirds () - CountBabyBirds ();
    }

    private int CountBabyBirds () {
        int cnt = 0;
        foreach (Transform child in birdInstantiateObject) {
            Bird bird = child.gameObject.GetComponent<Bird> ();
            if (bird.currentState == Bird.BirdState.Baby)
                cnt++;
        }
        return cnt;
    }

    private int CountBirds () {
        return birdInstantiateObject.childCount;
    }

    public void ChangeBirdCount (int newBirdCount) {
        birdCount = newBirdCount;
        GenerateField ();
        //if (simulationStarted)
            StopSimulation ();
        uiManager.ChangeSimTextToStart ();
    }

    public void ChangeTreeCount (int newTreeCount) {
        treeCount = newTreeCount;
        healthyTreeGraph.maxY = treeCount;
        damagedTreeGraph.maxY = treeCount;
        deadTreeGraph.maxY = treeCount;
        GenerateField ();
        //if (simulationStarted)
            StopSimulation ();
        uiManager.ChangeSimTextToStart ();
    }

   public void ChangeChildrenPerBirdNum (int newVal) {
       foreach (Transform birdTransform in birdInstantiateObject) {
            Bird bird = birdTransform.gameObject.GetComponent<Bird> ();
            bird.UpdateChildrenPerParNum (newVal);
        }
   }

   public void ChangeBirthTime (float newVal) {
       foreach (Transform birdTransform in birdInstantiateObject) {
            Bird bird = birdTransform.gameObject.GetComponent<Bird> ();
            bird.timeToBirth = newVal + Random.Range(-smallOfset, smallOfset);
        }
   }

   public void ChangeBabyTime (float newVal) {
       foreach (Transform birdTransform in birdInstantiateObject) {
            Bird bird = birdTransform.gameObject.GetComponent<Bird> ();
            bird.timeBeingBaby = newVal + Random.Range(-smallOfset, smallOfset);
        }
   }

    public void ToggleSimulationState () {
        if (simulationStarted)
            StopSimulation ();
        else
            StartSimulation ();
    }

    public void StartSimulation () {
        //Time.timeScale = 1.0f;
        if (simulationTime >= maxSimTime) {
            GenerateField ();
        }
        simulationStarted = true;
        uiManager.ChangeSimTextToStop ();
        foreach (Transform bird in birdInstantiateObject) {
            bird.GetComponent<Bird> ().active = true;
        }

        /*playButton.SetActive (false);
        pauseButton.SetActive (true);
        restartButton.SetActive (true);*/
    }

    public void StopSimulation () {
        //Time.timeScale = 0.0f;
        simulationStarted = false;
        uiManager.ChangeSimTextToContinue ();
        foreach (Transform bird in birdInstantiateObject) {
            bird.GetComponent<Bird> ().active = false;
        }

        /*playButton.SetActive (true);
        pauseButton.SetActive (false);
        restartButton.SetActive (true);*/
    }

    public void RestartSimulation () {
        StopSimulation ();
        GenerateField ();
        uiManager.ChangeSimTextToStart ();
        //Debug.Log (" end of restart function");

        /*playButton.SetActive (true);
        pauseButton.SetActive (false);
        restartButton.SetActive (false);*/
    }

    List<Vector2> GenerateCoordinates (int count) {
        List<Vector2> coordinates = new List<Vector2> ();
        for (int i = 0; i < count; i++) {
            Vector2 newCoord;
            bool ok = true;
            int cnt = 0;
            do {
                newCoord = GetRandomPoint();
                ok = true;
                for (int j = 0; j < i; j++) {
                    if (Distance (newCoord, coordinates[j]) <= offset*offset) {
                        ok = false;
                        break;
                    }
                }
                cnt++;
            } while (!ok && cnt < 100);
            coordinates.Add (newCoord);
        }
        return coordinates;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0, 0, 1, 0.1f);
        Vector2 center = new Vector2 (
            (xBounds.x + xBounds.y)/2, 
            (yBounds.x + yBounds.y)/2
        );
        Vector2 size = new Vector2 (
            xBounds.y - xBounds.x,
            yBounds.y - yBounds.x
        );
        Gizmos.DrawCube(center, size);

        Gizmos.color = new Color(1, 0, 0, 0.1f);
        size.x -= 2*offset;
        size.y -= 2*offset;
        Gizmos.DrawCube(center, size);

        Gizmos.color = new Color(0, 1, 0, 0.2f);
        foreach (Transform bird in birdInstantiateObject) {
            Gizmos.DrawSphere(bird.position, offset);
        }
        foreach (Transform tree in treeInstantiateObject) {
            Gizmos.DrawSphere(tree.position, offset);
        }
    }

    private Vector2 GetRandomPoint () {
        float minBoundsX = xBounds.x + offset;
        float maxBoundsX = xBounds.y - offset;
        float minBoundsY = yBounds.x + offset;
        float maxBoundsY = yBounds.y - offset;
        return new Vector2 (
            Random.Range (minBoundsX, maxBoundsX),
            Random.Range (minBoundsY, maxBoundsY)
        );
    }

    private float Distance (Vector2 pos1, Vector2 pos2) {
        float dist = (pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y);
        return dist;
    }

}
