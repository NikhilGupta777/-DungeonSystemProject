using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSettings
{
    public class Inputs
    {
        [System.Serializable]
        public enum commandType
        {
            Key,Axis
        }
        [System.Serializable]
        public class Control
        {
            public KeyCode[] Keys;
            public KeyCode[] NegKeys;
            [Space]
            public bool UseAxis;
            [Range(0,5)]
            public float Sensistivity = 1;
            public string JoystickAxisName;
        }
        public static bool GetButton(Control control)
        {
            bool KeyPressed = new bool();
            for (int i = 0; i < control.Keys.Length; i++)
            {
                if (control.NegKeys.Length > 0)
                {
                    for (int x = 0; x < control.NegKeys.Length; x++)
                    {
                        KeyPressed = Input.GetKey(control.Keys[i]) || Input.GetKey(control.NegKeys[x]);
                    }
                } else
                {
                    KeyPressed = Input.GetKey(control.Keys[i]);
                }
            }
            return KeyPressed;
        }
        public static bool GetButtonDown(Control control)
        {
            bool KeyPressed = new bool();
            for (int i = 0; i < control.Keys.Length; i++)
            {
                KeyPressed = Input.GetKeyDown(control.Keys[i]);
            }
            return KeyPressed;
        }
        public static bool GetButtonUp(Control control)
        {
            bool KeyPressed = new bool();
            for (int i = 0; i < control.Keys.Length; i++)
            {
                KeyPressed = Input.GetKeyUp(control.Keys[i]);
            }
            return KeyPressed;
        }
        public static float GetButtonAxis(Control control)
        {
            float KeyPressed = new float();
            for (int i = 0; i < control.Keys.Length; i++)
            {
                for (int x = 0; x < control.NegKeys.Length; x++)
                {
                    if (Input.GetKey(control.Keys[i]))
                    {
                        KeyPressed = 1;
                    } else
                    {
                        if (Input.GetKey(control.NegKeys[i]))
                        {
                            KeyPressed = -1;
                        } else
                        {
                            KeyPressed = 0;
                        }
                    }
                }
            }

            if (control.UseAxis)
            {
                KeyPressed = (Input.GetAxis(control.JoystickAxisName) * control.Sensistivity);
            }
            return KeyPressed;
        }
    }

    [CreateAssetMenu(fileName = "GameConfigurations", menuName = "Create Game Settings")]
    public class GameConfigurations : ScriptableObject
    {
        public void OnValidate()
        {
            if (ThisIsMainSettings)
            {
                MainSettings = this;
                Debug.Log("Game Main Settings now is: " + MainSettings.name);
            }
        }

        [SerializeField]
        public static GameConfigurations MainSettings;
        public bool ThisIsMainSettings;

        [System.Serializable]
        public class controls
        {
            [Header("Camera Movement")]
            [SerializeField]
            public Inputs.Control CameraMoveX;
            [SerializeField]
            public Inputs.Control CameraMoveY;

            [Header("Gameplay")]
            [SerializeField]
            public Inputs.Control WalkVertical;
            [SerializeField]
            public Inputs.Control WalkHorizontal;
            [SerializeField]
            public Inputs.Control Run;
            [SerializeField]
            public Inputs.Control Jump;
            [SerializeField]
            public Inputs.Control Aim;
        }
        [SerializeField]
        public controls Controls;
    }
}
