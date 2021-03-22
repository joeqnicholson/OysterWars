using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

public enum AIBehaviourType
{
	Sequence ,
	Follow	
}

[System.Serializable]
public class CharacterAIAction
{
	[Range_NoSlider( true )]
	public float duration = 1;
	
	[ CustomClassDrawer ]
	public CharacterActions action = new CharacterActions();
}

	

}