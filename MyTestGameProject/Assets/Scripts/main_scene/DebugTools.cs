using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour 
{
	public void Debug_LoadNextLevel()
	{
		GameManager.Instance.LoadNextLevel();
	}
	
	
}
