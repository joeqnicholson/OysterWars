using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WadeSound : MonoBehaviour
{
    // Declare Variables

    [SerializeField] private AudioClip footStep;
    [SerializeField] private AudioClip birdChirp;
    [SerializeField] private AudioClip wadeHit;
    [SerializeField] private AudioClip[] gunShots = new AudioClip[10];
    [SerializeField] private AudioClip[] footSteps = new AudioClip[10];
    [SerializeField] private AudioClip jumpUp;
    [SerializeField] private AudioClip jumpLand;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (wadeHit == null)
            Debug.LogError("wadeHit has not been assigned.", this);
        // Notice, that we pass 'this' as a context object so that Unity will highlight this object when clicked.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayRandom(AudioClip[] clips)
    {
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
    }

    public void PlayGunShot ()
    {
        PlayRandom(gunShots);
    }

    public void PlayFootStep()
    {
        PlayRandom(footSteps);
    }

    public void PlayWadeHit()
    {
        audioSource.PlayOneShot(wadeHit);

    }

    public void PlayJumpLand()
    {
        audioSource.PlayOneShot(jumpLand);

    }

    public void PlayJumpUp()
    {
        audioSource.PlayOneShot(jumpUp);

    }

}
