using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lightbug.Kinematic2D.Extras
{

public class PrefabInstantiator : MonoBehaviour {

	[SerializeField] float instantiationTime = 1;
	[SerializeField] GameObject prefab;
	[SerializeField] Text outputText = null;


	[SerializeField] bool limitNumber = false;
	[SerializeField] int maxNumber = 400;
	
	public void SetMaxNumber(int maxNumber)
	{
		this.maxNumber = maxNumber;
	}

	public void EnableLimit(bool enable)
	{
		limitNumber = enable;
	}

	public void SetPrefab(GameObject prefab)
	{
		this.prefab = prefab;
	}

	int currentNumber = 0;
	public int CurrentNumber
	{
		get
		{
			return currentNumber;
		}
	}

	public bool startCoroutineOnStart = true;

	Coroutine coroutine;

	void Start ()
	{	
		if( startCoroutineOnStart )			
			coroutine = StartCoroutine(Coroutine());
	}
	
	public void StartProcess()
	{
		coroutine = StartCoroutine(Coroutine());
	}

	public void StopProcess()
	{
		StopCoroutine(coroutine);
	}

	IEnumerator Coroutine () 
	{
		bool condition = limitNumber ? currentNumber < maxNumber : true;
		while( condition )
		{
			if(prefab != null)
			{
				Instantiate( prefab,transform.position , transform.rotation );				
				currentNumber++;
			}

			if(outputText != null)
				outputText.text = "Number of AI characters instantiated : " + currentNumber.ToString();

			yield return new WaitForSeconds(instantiationTime);

			condition = limitNumber ? currentNumber < maxNumber : true;
		}
	}
}

}
