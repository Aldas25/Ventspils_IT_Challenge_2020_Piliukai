using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public enum TreeState {
        Healthy,
        Damaged,
        Dead
    };

    public SpriteRenderer spriteRenderer;
    public GameObject nestPrefab;

    public float initialHealth = 100.0f;
    public Sprite heathyTreeSprite;
    public Sprite damagedTreeSprite;
    public Sprite deadTreeSprite;

    private float health = 100.0f; 
    public TreeState currentState;
    private bool hasNest = false;
    public bool willHaveNest = false;


    void Start () {
        health = initialHealth;
        UpdateState (TreeState.Healthy);
    }

    public void Damage (float damage = 18.0f) {
        health -= damage;

        if (health <= 0.0f)
            UpdateState (TreeState.Dead);
        else if (health <= 40.0f)
            UpdateState (TreeState.Damaged);

    }

    private void UpdateState (TreeState newState) {
        if (currentState == newState)
            return;

        switch(newState) {
            case TreeState.Healthy:
                SetStateToHealthy ();
                break;
            case TreeState.Damaged:
                SetStateToDamaged ();
                break;
            case TreeState.Dead:
                SetStateToDead ();
                break;
            default:
                Debug.Log("Reached unkown state");
                break;
        }
    }

    private void SetStateToHealthy () {
        currentState = TreeState.Healthy;
        spriteRenderer.sprite = heathyTreeSprite;
    }

    private void SetStateToDamaged () {
        currentState = TreeState.Damaged;
        spriteRenderer.sprite = damagedTreeSprite;
    }

    private void SetStateToDead () {
        currentState = TreeState.Dead;
        spriteRenderer.sprite = deadTreeSprite;
    }

    public void AddNest () {
        if (hasNest)
            return;

        hasNest = true;
        Instantiate (nestPrefab, transform.position, Quaternion.identity, transform);  
    }
}
