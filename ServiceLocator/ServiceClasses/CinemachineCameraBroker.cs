
using AbstractClasses;
using Cinemachine;
using K_LanUtilities;

namespace Services
{
    //controller service for the cinemachine virtual camera
    //couldn't decide whether or not to directly inject this as a dependency, 
    //or just to expose itself through the service locator
    public class CinemachineCameraBroker : Service
    {
        private CinemachineVirtualCamera cinemachineCam;
        private ICinemachineCameraReceivable[] cinemachineCameraComponents;

        void OnEnable()
        {
            cinemachineCam = GetComponent<CinemachineVirtualCamera>();
            cinemachineCameraComponents = StaticUtilities.ReturnInterfaceImplementationsFromScene<ICinemachineCameraReceivable>();
        }

        void Start()
        {
            InjectCinemachineVirtualCamera();
        }

        public CinemachineVirtualCamera GetCinemachineVirtualCamera()
        {
            return cinemachineCam;
        }

        private void InjectCinemachineVirtualCamera()
        {
            for (int i = 0; i < cinemachineCameraComponents.Length; i++)
            {
                cinemachineCameraComponents[i].CinemachineCamera(cinemachineCam);
            }
        }
    }

    public interface ICinemachineCameraReceivable
    {
        void CinemachineCamera(CinemachineVirtualCamera cinemachineCam);
    }
}