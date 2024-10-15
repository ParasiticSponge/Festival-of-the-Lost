using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    private void Star()
    {
        StartCoroutine(Play_Menu_Sounds.PlayClip(0, MenuManager_2.sfxVol));
    }
}
