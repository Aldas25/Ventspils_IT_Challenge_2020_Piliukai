using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public enum TreeState {
        Healthy,
        Damaged
    };

    public SpriteRenderer spriteRenderer;
    public GameObject nestPrefab;

    public float initialHealth = 100.0f;
    public Color heathyTreeColor;
    public Color damagedTreeColor;

    private float health = 100.0f; 
    public TreeState currentState;
    private bool hasNest = false;
    public bool willHaveNest = false;


    void Start () {
        health = initialHealth;
        UpdateState(TreeState.Healthy);
    }

    public void Damage (float damage = 10.0f) {
        health -= damage;

        if (health < 25.0f)
            UpdateState (TreeState.Damaged);

    }

    private void UpdateState (TreeState newState) {
        if (currentState == newState)
            return;

        switch(newState) {
            case TreeState.Healthy:
                SetStateToHealthy();
                break;
            case TreeState.Damaged:
                SetStateToDamaged();
                break;
            default:
                Debug.Log("Reached unkown state");
                break;
        }
    }

    private void SetStateToHealthy () {
        currentState = TreeState.Healthy;
        spriteRenderer.color = heathyTreeColor;
    }

    private void SetStateToDamaged () {
        currentState = TreeState.Damaged;
        spriteRenderer.color = damagedTreeColor;
    }

    public void AddNest () {
        if (hasNest)
            return;

        hasNest = true;
        Instantiate (nestPrefab, transform.position, Quaternion.identity, transform);  
    }
}
