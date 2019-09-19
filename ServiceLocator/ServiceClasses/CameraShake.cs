using System.Collections;
using AbstractClasses;
using Cinemachine;
using UnityEngine;

//gotta get rid of inspector variables
namespace Services
{
	public class CameraShake : Service, IGM_ServiceLocator_Recievable
	{
		//Dependencies
		private GM_ServiceLocator gm;

		private CinemachineVirtualCamera virtualCamera;							

		//working variables
		[SerializeField]
		private CinemachineBasicMultiChannelPerlin virtualCameraNoise;			//the noise module of cinemachine's camera

		void Start() 
		{
			virtualCamera = gm.GetService<CinemachineCameraBroker>().GetCinemachineVirtualCamera();

			if (virtualCamera != null)
			{
				virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();	
			} 
		}

		public IEnumerator ShakingCamera(float time, float amplitude, float frequency)
		{
			// Debug.Log("time: " + time + " || amplitude: " + amplitude + " || frequency: " + frequency);

			virtualCameraNoise.m_AmplitudeGain = amplitude;
			virtualCameraNoise.m_FrequencyGain = frequency;

			while (amplitude >= 0)
			{
				virtualCameraNoise.m_AmplitudeGain = amplitude;
				amplitude -= (Time.deltaTime)*10;

				// print("amplitude = " + amplitude);
				yield return null;
			}

			virtualCameraNoise.m_AmplitudeGain = 0;
			virtualCameraNoise.m_FrequencyGain = 0;
		}

		public void AssignGM(GM_ServiceLocator serviceLocator)
		{
			gm = serviceLocator;
		}
	}
}

