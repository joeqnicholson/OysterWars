using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvenMachine : MonoBehaviour
{
    [SerializeField] SpriteAnimation SvenIdle;
    [SerializeField] SpriteAnimationController sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        sprite.Play(SvenIdle);
    }
}
