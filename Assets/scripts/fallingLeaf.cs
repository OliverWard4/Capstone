using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingLeaf : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float horizontalSpeed = 2.0f;
    [SerializeField] private float fallSpeed = 4.0f;
    [SerializeField] private float phaseOffset = 0f;
    [SerializeField] private float lifeSpan = 10f;

    [Header("Shrink Settings")]
    [SerializeField] private float shrinkRate = 0.1f;     // scale shrink per second
    [SerializeField] private float fallShrinkRate = 0.001f; // how fast fall speed shrinks

    private float startX;
    private float time;
    private Vector3 originalScale;

    void Start()
    {
        startX = transform.position.x;
        originalScale = transform.localScale;

        if (phaseOffset == 0f)
        {
            phaseOffset = Random.Range(-5f, 5f);
        }

        Invoke("Leafdie", lifeSpan);
    }

    void Update()
    {
        time += Time.deltaTime;

        // shrinking size
        transform.localScale -= Vector3.one * shrinkRate * Time.deltaTime;
        transform.localScale = Vector3.Max(transform.localScale, Vector3.zero); // avoid negatives

        // shrinking fall speed
        fallSpeed = Mathf.Max(.3f, fallSpeed - fallShrinkRate * Time.deltaTime);

        // motion
        float x = startX + amplitude * Mathf.Sin(time * horizontalSpeed + phaseOffset);
        float y = transform.position.y - fallSpeed * Time.deltaTime;

        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void Leafdie()
    {
        Destroy(gameObject);
    }
}

