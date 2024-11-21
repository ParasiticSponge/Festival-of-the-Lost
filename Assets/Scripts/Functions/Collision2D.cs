using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision2D : MonoBehaviour
{
    public enum Type
    {
        Circle, // 0
        Rectangle, // 1 
        Banana // 2
    };

    public Type type;

    public float radius;
    public Vector2 offset;
    public float mass = 1.0f;
    public bool useScale;

    void Awake()
    {
        if (useScale) radius = transform.localScale.x / 2;
    }
}
