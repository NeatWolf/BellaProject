﻿using UnityEngine;
using System.Collections;

public class PlayerControlledComponent : MonoBehaviour 
{

	public AudioClip horseSound;
	public AudioClip sparkleSound;


	public Ray ray;
	public RaycastHit rayHit;
	public NavMeshAgent navMeshAgent;
	public float MaxSpeed = 5;

	[SerializeField]
	private float speedPercent = 100;

	[SerializeField]
	private float speedMod = 0;

	[SerializeField]
	private GameObject BreathInput;

	private PEPTestDataInput testData;
	private PepMeasurementsComponent measurements;

	private BackpackComponent backpack;

	Animator horseAnim;


	void Start()
	{
		navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		backpack = gameObject.GetComponent<BackpackComponent>();
		horseAnim = gameObject.GetComponentInChildren<Animator>();
		if(BreathInput != null)
		{
			testData = BreathInput.GetComponent<PEPTestDataInput>();
			measurements = BreathInput.GetComponent<PepMeasurementsComponent>();
		}
		else
		{
			Debug.LogError("There is no breath input in the scene. This is bad");
		}
	}


	void Update()
	{
		GetTouchInput();
		SpeedPercent = SetSpeedNumber() * (SpeedMod / 100f);
		//Debug.Log("This is the speed percentage" + SpeedPercent);
		navMeshAgent.speed = MaxSpeed * ((float)speedPercent / 100);
		IncreaseSpeedMod();
		CheckForFailedRound();
		horseAnim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
	}

	private void IncreaseSpeedMod ()
	{
		if(measurements.ConstantBreath == true)
		{
			SpeedMod += 0.25f;
		}
	}

	void CheckForFailedRound ()
	{
		if(measurements.ConstantBreath == false && backpack.RingsHeld > 0)
		{
			AudioSource.PlayClipAtPoint(horseSound, gameObject.transform.position);
//			horseS
			SpeedMod = 20f;
			// Now throw off all rings in a magical way.
			int ringsToThrow = backpack.RingsHeld;
			backpack.timeToPickUp = 4f;
			for(int i = 0; i < ringsToThrow; i++)
			{
				GameObject newObj = Instantiate(Resources.Load("Ring"), gameObject.transform.position + Vector3.up * 2f + new Vector3(Random.Range(-1, 1f), 0f, Random.Range(-1,1f)), Quaternion.identity) as GameObject;
				newObj.rigidbody.AddForce(gameObject.transform.up * 5 + new Vector3(Random.Range(-1, 1f), 0f, Random.Range(-1,1f)) * 5);
			}
			backpack.RingsHeld = 0;
		}
	}

	private void GetTouchInput()
	{
		if(Input.GetButton("MouseTouch1"))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out rayHit, 200f);
			if(rayHit.collider != null)
			{
				navMeshAgent.destination = rayHit.point;
			}
		}
	}

	private int tempSpeed = 100; // Not kept in the function for Garbage Collection
	
	private int SetSpeedNumber()
	{
		switch (testData.Status)
		{
		case (BreathStatus.Inhale):
			tempSpeed = Inhale();
			break;

		case (BreathStatus.Exhale):
			tempSpeed = Exhale();
			break;

		case (BreathStatus.Rest):
			tempSpeed = Rest();
			break;
			
		case (BreathStatus.Finished):
			tempSpeed = Finished();
			break;
		}

		return tempSpeed;
	}

	private int Inhale ()
	{
		// This assumes the person is breathing outside of the mask
		return 100;
	}

	// All the information for the Exhale speed portion

	private int tempExhaleSpeed = 100;

	private int Exhale ()
	{
		if(testData.BreathStrength < testData.PressureMaximum && testData.BreathStrength > testData.PressureMinimum)
		{
			tempExhaleSpeed = 100; // returns percentage of speed
		}
		else if(testData.BreathStrength > testData.PressureMaximum)
		{
			tempExhaleSpeed = 20;
		}
		else if(testData.BreathStrength < testData.PressureMinimum)
		{
			tempExhaleSpeed =  Mathf.RoundToInt((testData.BreathStrength / (float)testData.PressureMinimum) * 100f);

		}
		return tempExhaleSpeed;
	}

	private int Rest ()
	{
		// This assumes the person is breathing outside of the mask
		return 100;
	}

	private int Finished ()
	{
		return 100;
	}



	
	public float SpeedPercent
	{
		get { return speedPercent; }
		set 
		{
			if(value < 0)
			{
				speedPercent = 0;
			}
			else if (value > 100)
			{
				speedPercent = 100;
			}
			else
			{
				speedPercent = value;
			}
		}
	}

	public float SpeedMod
	{
		get { return speedMod; }
		set 
		{
			if(value < 0)
			{
				speedMod = 0;
			}
			else if (value > 100)
			{
				speedMod = 100;
			}
			else
			{
				speedMod = value;
			}
		}
	}

}
















