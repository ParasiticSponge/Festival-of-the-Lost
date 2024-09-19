using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public string charName;
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
        
    }

    void GetName(string name)
    {
        charName = name;
        //only get first input, then stop listening
        Actions.Input -= GetName;
    }
}
