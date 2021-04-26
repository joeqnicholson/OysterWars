using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WadeSound : MonoBehaviour
{
    // Declare Variables

    [SerializeField] private AudioClip footStep;
    [SerializeField] private AudioClip birdChirp;
    [SerializeField] private AudioClip[] gunShots = new AudioClip[10];

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGunShot ()
    {
        audioSource.PlayOneShot(gunShots[Random.Range(0, gunShots.Length - 1)]);
    }

}
