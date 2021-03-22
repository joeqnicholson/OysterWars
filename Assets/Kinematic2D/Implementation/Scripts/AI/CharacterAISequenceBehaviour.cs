using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Implementation
{

[CreateAssetMenu( menuName = "Kinematic 2D/Implementation/AI/Sequence Behaviour" )]
public class CharacterAISequenceBehaviour : ScriptableObject
{
	
	[SerializeField] List<CharacterAIAction> actionSequence = new List<CharacterAIAction>();
	public List<CharacterAIAction> ActionSequence
	{
		get
		{
			return actionSequence;
		}
	}

	

	
}


}
