using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Components;
using K_LanUtilities;
using Services;
using UnityEngine;

namespace Services
{
	//creates a Smash Bros like camera by allowing other components to alter the camera weighting of targeted gameObjects
	public class GroupTargetBrokerSmashBrosCamera : AbstractClasses.Service
	{
		private CinemachineTargetGroup cameraTargetGroup;							//the obj that holds the list of camera targets

		void OnEnable()
		{
			cameraTargetGroup = GetComponent<CinemachineTargetGroup>();
		}
		
		private void Start()
		{
			SetUpCameraTargets();
		}

		public void AddTargetToCam(Transform bouncer)
		{
			cameraTargetGroup.AddMember(bouncer, 0f, .01f);
			// print("bouncer added to camera");
		}
		public void RemoveBouncerToTarget(Transform bouncer)
		{
			cameraTargetGroup.RemoveMember(bouncer);
			// print("bouncer removed from camera");
		}

		public IEnumerator ChangeCamTargetWeight(Transform bouncer, float weight)
		{
			float currentWeight = cameraTargetGroup.m_Targets[cameraTargetGroup.FindMember(bouncer)].weight;
			bool high = false;

			if(weight > currentWeight)
			{
				high = true;
				
				//lerping up
				while(currentWeight < .99f)
				{
					currentWeight = Mathf.Lerp(currentWeight, weight, Time.deltaTime);

					cameraTargetGroup.m_Targets[cameraTargetGroup.FindMember(bouncer)].weight = currentWeight;		

					// print("changing cam weight running (up): " + currentWeight);
					yield return null;	
				}			
			}

			else if(weight < currentWeight)
			{
				high = false;
				
				//lerping down
				while(currentWeight > .01f)
				{
					currentWeight = Mathf.Lerp(currentWeight, weight, Time.deltaTime);

					cameraTargetGroup.m_Targets[cameraTargetGroup.FindMember(bouncer)].weight = currentWeight;
					
					yield return null;	
				}			
			}

			if(high)
			{
				cameraTargetGroup.m_Targets[cameraTargetGroup.FindMember(bouncer)].weight = 1;
			}
			if(!high)
			{
				cameraTargetGroup.m_Targets[cameraTargetGroup.FindMember(bouncer)].weight = 0;
			}

		}

		private void SetUpCameraTargets()
		{
			var interfaces = FindObjectsOfType<MonoBehaviour>().OfType<ICameraTargetable>();
			
			foreach (var I in interfaces)
			{
				CheckCamGroupForTarget(I);
			}
		}

		public void ResetWithoutLerp(Transform targetToReset)
		{
			cameraTargetGroup.m_Targets[cameraTargetGroup.FindMember(targetToReset)].weight = 0;
		}

		private void CheckCamGroupForTarget(ICameraTargetable target)
		{
			Transform t = target.TransformToAddToCameraTargetGroup();

			if (cameraTargetGroup.FindMember(t) == -1) //checks if member is already being tracked by cam
			{
				AddTargetToCam(target.TransformToAddToCameraTargetGroup());
			}
		}
	}
}

public interface ICameraTargetable
{
	Transform TransformToAddToCameraTargetGroup();
}
