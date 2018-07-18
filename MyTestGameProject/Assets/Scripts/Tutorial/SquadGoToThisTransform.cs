using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadGoToThisTransform : MonoBehaviour 
{
	[SerializeField] Squad squad;
	
	void Start()
	{
		squad.Controller.MoveToPoint(transform.position);
        Debug.Log("SquadGoToThisTransform");
	}
}
