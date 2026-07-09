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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            bicycle = GetComponent<BicycleVehicle>();
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
