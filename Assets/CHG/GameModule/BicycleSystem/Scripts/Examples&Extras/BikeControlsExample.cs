using CHG.Bike;
using CHG.Scripts;
using rayzngames;
using UnityEngine;

namespace rayzngames
{
    public class BikeControlsExample : MonoBehaviour
    {
        BicycleVehicle bicycle;
        public bool controllingBike;
        [SerializeField] private PlayerInputSO playerInput;

        // 있으면 조종/회전 권한을 이 모듈에 위임한다(넉백 연출과 충돌 방지). 없으면 기존 방식 사용.
        private BikeControlModule controlModule;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            bicycle = GetComponent<BicycleVehicle>();
            controlModule = GetComponent<BikeControlModule>();
            playerInput.OnMoved += HandleMoved;
            playerInput.OnMovedEnded += HandleMovedEnd;
            playerInput.OnStopStated += HandleStopStated;
            playerInput.OnStopEnded += HandleStopEnded;
        }

        private void HandleMovedEnd()
        {
            bicycle.VerticalInput = 0;
            bicycle.HorizontalInput = 0;
        }


        private void HandleMoved(Vector2 evt)
        {
            bicycle.VerticalInput = evt.y;
            bicycle.HorizontalInput = evt.x;
        }

        private void HandleStopStated()
        {
            bicycle.Braking = true;
        }

        private void HandleStopEnded()
        {
            bicycle.Braking = false;
        }
        // Update is called once per frame
        void Update()
        {
            //bicycle.VerticalInput = Input.GetAxis("Vertical");
            //bicycle.HorizontalInput = Input.GetAxis("Horizontal");
            BrakingInput();

            if (controlModule != null)
            {
                // 권한을 모듈에 위임: 조종 의사만 전달하고, 실제 InControl/회전 제어는 모듈이 결정한다.
                controlModule.PlayerWantsControl = controllingBike;
            }
            else
            {
                //Extending functionality
                bicycle.InControl(controllingBike);

                if (controllingBike)
                {
                    //Constrains the Z rotation of the bike, when onground, and releases it when airborne.
                    bicycle.ConstrainRotation(bicycle.OnGround());
                }
                else
                {
                    bicycle.ConstrainRotation(false);
                }
            }

            /*
            //Detach controls
            if (bicycle.OnGround() == false) { controllingBike = false; }

            //Landing Controls (Land Pressing E)
            if (Input.GetKey(KeyCode.E)) { controllingBike = true; }
            bicycle.InControl(controllingBike);   
            */
        }
        void BrakingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Braking");
                
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                
            }

        }
    }
}
