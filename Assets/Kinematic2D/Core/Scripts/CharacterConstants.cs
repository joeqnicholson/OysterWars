using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Core
{

public static class CharacterConstants
{
	
	public const float MinimumMovement = 0.001f;

	/// <summary>
	/// The skin width is used to prevent the character from getting stuck with other collider. If you are experiencing some problems try to increase this value.
	/// </summary>
	public const float SkinWidth = 0.01f;

	/// <summary>
	/// The thickness of the box shape used by the Boxcast methods.
	/// </summary>
	public const float DepenetrationBoxThickness = 0.01f;
	

	// public const float BoxContactOffsetX = 0.0045f;
	// public const float BoxContactOffsetY = 0.0045f;
	
}

}
