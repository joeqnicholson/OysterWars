using UnityEngine;
using Lightbug.Kinematic2D.Implementation;

public class BrainSwitcher : MonoBehaviour
{
    public CharacterHybridBrain brain = null;
  
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.B) )
            brain.SetBrainType( !brain.IsAI );
    }
}
