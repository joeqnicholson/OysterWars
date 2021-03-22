#if UnityEditor

using UnityEngine;
using UnityEditor;

namespace Lightbug.Kinematic2D.Core
{

[CustomPropertyDrawer(typeof(Range_NoSliderAttribute))]
public class Range_NoSliderAttributeEditor : PropertyDrawer
{
	Range_NoSliderAttribute target;
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{		
		if( target == null )
			target = attribute as Range_NoSliderAttribute;

		if( property.propertyType == SerializedPropertyType.Float )
		{
			property.floatValue = Mathf.Clamp( property.floatValue , target.minFloat , target.maxFloat );
		} 
		else if( property.propertyType == SerializedPropertyType.Integer )
		{
			property.intValue = Mathf.Clamp( property.intValue , target.minInteger , target.maxInteger );
		}
		else
		{
			GUI.Label( position , "This attribute doesn't work properly with the chosen field type." );
			return;
		}

		EditorGUI.PropertyField( position , property );
	}
}

}


#endif