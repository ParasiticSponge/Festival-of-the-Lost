using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_Menu_Sounds : MonoBehaviour
{
    public List<AudioClip> musicClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private GameObject audioClipPrefab;
    private void OnEnable()
    {
        Actions.MenuBeginSound += PressPlay;
    }
    private void OnDisable()
    {
        Actions.MenuBeginSound -= PressPlay;
    }
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
    public static AudioSource CreateClipReturn(ushort clipId, float clipVol)
    {
        Play_Menu_Sounds currentManager = FindObjectOfType<Play_Menu_Sounds>();
        AudioSource source = Instantiate(currentManager.audioClipPrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = currentManager.audioClips[clipId];
        source.volume = clipVol;
        source.Play();

        return source;
    }
    public static void ChangeClip(AudioSource source, AudioClip clip, float clipVol)
    {
        source.clip = clip;
        source.volume = clipVol;
        source.Play();
    }

    public void PressPlay()
    {
        StartCoroutine(PlayClip(1, MenuManager_2.sfxVol));
    }
}