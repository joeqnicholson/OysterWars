using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public SpriteAnimation ChestAnimation;
    public SpriteAnimation WorldAnimation;
    public SpriteAnimation MenuIcon;
}
