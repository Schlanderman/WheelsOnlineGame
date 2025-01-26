using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ProfileData))]
public class ProfileDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ProfileData profileData = (ProfileData)target;

        if (GUILayout.Button("Reset PlayerPrefs"))
        {
            profileData.ResetPlayerPrefs();
        }
    }
}
#endif
