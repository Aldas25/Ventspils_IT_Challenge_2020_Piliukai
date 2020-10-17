using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    
    public Transform dotInstantiateObject;
    public GameObject dotPrefab;

    public Color dotColor;

    public float maxX = 100.0f;
    public float maxY = 100.0f;

    private float rectWidth;
    private float rectHeight;
    private float moved = 0.0f;

    void Awake () {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
        rectWidth = rectTransform.rect.width;
        rectHeight = rectTransform.rect.height;
    }

    public void AddDot (float x, float y) {
        x -= moved;
        if (x > maxX) {
            MoveDots (rectWidth * (x - maxX)/maxX);
            moved += x-maxX;
            x = maxX;
        }

        GameObject dot = Instantiate (dotPrefab, dotInstantiateObject.position, Quaternion.identity, dotInstantiateObject);
        RectTransform rt = dot.GetComponent<RectTransform> ();
        Image image = dot.GetComponent<Image> ();

        Vector2 pos = rt.localPosition;
        pos.x += rectWidth * x/maxX;
        pos.y += rectHeight * y/maxY;
        rt.localPosition = pos;

        image.color = dotColor;
    }

    public void ClearGraph () {
        moved = 0.0f;
        foreach (Transform child in dotInstantiateObject) {
            Destroy (child.gameObject);
        }
    }

    private void MoveDots (float x) {
        foreach (Transform child in dotInstantiateObject) {
            RectTransform rt = child.gameObject.GetComponent<RectTransform> ();
            Vector2 pos = rt.localPosition;
            pos.x -= x;
            rt.localPosition = pos;

            if (pos.x < 0.0f)
                Destroy (child.gameObject);
        }
    }

}
