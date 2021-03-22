using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation", menuName = "SpriteAnimation")]
public class SpriteAnimation : ScriptableObject
{
    [SerializeField] Sprite[] keyframes = new Sprite[1];
    public int imageIndex = 0;
    [SerializeField] private float framesPerSecond;
    [SerializeField] public bool loop = true;
    public float totalFrames;
   
    
    private float frameTimer;

    void OnEnable()
    {
        totalFrames = keyframes.Length;
    }

    public Sprite Keyframe(int index)
    {
        return keyframes[index];
    }

    public float FPS()
    {
        return framesPerSecond;
    }
}
