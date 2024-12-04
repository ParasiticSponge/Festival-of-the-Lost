using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.TextCore.Text;

public class PhysicsManager : MonoBehaviour
{
    BoxCollider2D[] boxCollision;
    CircleCollider2D[] circCollision;
    Collision2D[] collision;
    CharacterController2D character;
    float count = 0;
    private void Awake()
    {
        //boxCollision = FindObjectsOfType<BoxCollider2D>();
        circCollision = FindObjectsOfType<CircleCollider2D>();
        collision = FindObjectsOfType<Collision2D>();
        character = FindObjectOfType<CharacterController2D>();
    }
    // Update is called once per frame
    public void UpdateCollision()
    {
        circCollision = FindObjectsOfType<CircleCollider2D>();
        character = FindObjectOfType<CharacterController2D>();
    }
    void Update()
    {
        count = 0;
        foreach (Collision2D this_collision in collision)
        {
            //print(this_collision.gameObject.name);
            foreach (CircleCollider2D other_collision in circCollision)
            {
                //print(other_collision.gameObject.name);
                // Do not detect collision with yourself.
                if (this_collision == other_collision) continue;
                //weird effects happen with arrow animation if NPCs are included
                if (!other_collision.GetComponent<CharacterController2D>()) continue;

                // Circle to Circle collision.
                /*if (this_collision.type == Collision2D.Type.Circle && other_collision.type == Collision2D.Type.Circle)
                {*/
                    float this_radius = this_collision.radius;
                    float other_radius = other_collision.radius;

                    /*Vector2 this_to_other = other_collision.transform.position - this_collision.transform.position;
                    Vector2 other_to_this = this_collision.transform.position - other_collision.transform.position;
                    float distance_this_to_other = this_to_other.magnitude;*/

                    Vector2 thisObj = new Vector2(this_collision.transform.position.x, this_collision.transform.position.y);
                    Vector2 otherObj = new Vector2(other_collision.transform.position.x, other_collision.transform.position.y);

                    Vector2 this_to_other = (otherObj + other_collision.offset) - (thisObj + this_collision.offset);
                    float distance_this_to_other = this_to_other.magnitude;

                    // A negative intersection depth means no intersection. 3D only
                    float intersection_depth = (this_radius + other_radius) - distance_this_to_other;

                    if (intersection_depth > 0.0f)
                    {
                        count++;
                        Actions.isOverDoor.Invoke(this_collision.gameObject, true);
                        character.doorNum = Int32.Parse(this_collision.gameObject.name);
                        character.enter = true;
                        character.choice = 0;
                        /*// AI Resolution: Create a collision event that the gameobjects can respond to

                        float total_mass = this_collision.mass + other_collision.mass;

                        float other_mass_ratio = other_collision.mass / total_mass;
                        float this_mass_ratio = this_collision.mass / total_mass;

                        // Move other using this_to_other vector
                        other_collision.transform.position += this_to_other.normalized * intersection_depth * this_mass_ratio;

                        // Move this using other_to_this vector
                        this_collision.transform.position += other_to_this.normalized * intersection_depth * other_mass_ratio;*/
                    }
                    if (count == 0)
                    {
                        Actions.isOverDoor.Invoke(this_collision.gameObject, false);
                        character.enter = false;
                    }
                //}
            }
        }
    }
}
