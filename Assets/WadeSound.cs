using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WadeSound : MonoBehaviour
{
    // Declare Variables

    [SerializeField] private AudioClip footStep;
    [SerializeField] private AudioClip birdChirp;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayAudio(footStep);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayAudio(birdChirp);
        }
    }

    public void PlayAudio (AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

}
