using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;

namespace Inputs
{
    public class PlayerInputHandlerMenu : MonoBehaviour
    {
        [SerializeField] PlayerInput playerInput;
        [SerializeField] private float index;
        
        private Controls controls;
        private Controls Controls
        {
        get
            {
                if (controls != null) { return controls; }
                return controls = new Controls();
            }
        }
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        // Update is called once per frame
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            index = playerInput.playerIndex;
            /*var motors = FindObjectsOfType<PlayerController>();
            motor = motors.FirstOrDefault(m => m.GetPlayerIndex() == index);

            var combats = FindObjectsOfType<PlayerCombat>();
            combat = combats.FirstOrDefault(m => m.GetPlayerIndex() == index);*/

        }
        public void OnMove(CallbackContext context)
        {

        }
        public void OnLook(CallbackContext context)
        {

        }
        public void OnAttack(CallbackContext context)
        {

        }
        public void OnDash(CallbackContext context)
        {

        }
        public void OnZone()
        {

        }
        public void OnBigAttack(CallbackContext context)
        {

        }
        public void OnDefense()
        {

        }
    }
}
