using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GM_Jump))]
public class GM_JumpEditor : Editor
{
    void DrawCharHeight(GM_Jump Target)
    {
        Handles.color = Color.cyan;
        Handles.DrawLine(Target.transform.position, Target.transform.position + (Vector3.up * Target.CharacterHeight));
        Handles.DrawWireDisc(Target.transform.position + (Vector3.up * Target.CharacterHeight), Vector3.up, 0.5f);
    }
    void DrawMaxHeight(GM_Jump Target)
    {
        Handles.color = Color.cyan;
        Vector3 fwd = Target.transform.position + (Target.transform.forward * Target.MaxClimbDistance);

        Handles.DrawLine(fwd, fwd + (Vector3.up * Target.MaxClimbHeight));
    }
    void OnSceneGUI()
    {
        DrawCharHeight((GM_Jump)target);
        DrawMaxHeight((GM_Jump)target);
    }
}
