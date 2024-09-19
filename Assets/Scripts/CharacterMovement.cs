using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 1;
    public string charName;
    public int canMove = 1;

    private void OnEnable()
    {
        Actions.Input += GetName;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(horizontal, vertical, 0) * Time.deltaTime * movementSpeed * canMove;
    }

    void GetName(string name)
    {
        charName = name;
        //only get first input, then stop listening
        Actions.Input -= GetName;
    }
}
