using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameSettings
{
    namespace editor
    {

        [CustomEditor(typeof(GameConfigurations))]
        public class GameConfigurationsEditor : Editor
        {
            public void DrawCustomCommand(Inputs.Control control)
            {
                if (control.UseAxis)
                {
                    control.JoystickAxisName =
                        EditorGUILayout.TextField("Joystick Axis Name", control.JoystickAxisName);
                }
            }
            public override void OnInspectorGUI()
            {
                GameConfigurations Target = (GameConfigurations)target;
                base.OnInspectorGUI();


                if (Target.ThisIsMainSettings == true)
                {
                    GameConfigurations.MainSettings = Target;
                }
            }
        }
    }
}
