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

    private bool simulationStarted = false;

    private UIManager uiManager;

    void Awake () {
        uiManager = gameObject.GetComponent<UIManager> ();
    }

    void Start () {
        GenerateField ();
        StopSimulation ();
    }
    
    /*void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            ToggleSimulationState ();
        }
    }*/

    public void GenerateField () {
        GenerateTrees ();
        GenerateBirds ();
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

    public void ChangeBirdCount (int newBirdCount) {
        birdCount = newBirdCount;
        GenerateField ();
        StopSimulation ();
    }

    public void ChangeTreeCount (int newTreeCount) {
        treeCount = newTreeCount;
        GenerateField ();
        StopSimulation ();
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
            bird.timeToBirth = newVal;
        }
   }

   public void ChangeBabyTime (float newVal) {
       foreach (Transform birdTransform in birdInstantiateObject) {
            Bird bird = birdTransform.gameObject.GetComponent<Bird> ();
            bird.timeBeingBaby = newVal;
        }
   }

    public void ToggleSimulationState () {
        if (simulationStarted)
            StopSimulation ();
        else
            StartSimulation ();
    }

    private void StartSimulation () {
        simulationStarted = true;
        uiManager.ChangeSimTextToStop ();
        foreach (Transform bird in birdInstantiateObject) {
            bird.GetComponent<Bird> ().active = true;
        }
    }

    private void StopSimulation () {
        simulationStarted = false;
        uiManager.ChangeSimTextToStart ();
        foreach (Transform bird in birdInstantiateObject) {
            bird.GetComponent<Bird> ().active = false;
        }
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
