using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_Menu_Sounds : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private GameObject audioClipPrefab;

    public static IEnumerator PlayClip(ushort clipId, float clipVol)
    {
        Play_Menu_Sounds currentManager = FindObjectOfType<Play_Menu_Sounds>();
        AudioSource source = Instantiate(currentManager.audioClipPrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = currentManager.audioClips[clipId];
        source.volume = clipVol;
        source.Play();
        yield return new WaitWhile(() => source.isPlaying);

        Destroy(source.gameObject);
    }

    public void PressPlay()
    {
        StartCoroutine(PlayClip(1, 0.5f));
    }
}