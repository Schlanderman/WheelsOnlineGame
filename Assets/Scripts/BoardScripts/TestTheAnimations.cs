using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionRodAnimManager))]
public class TestTheAnimations : Editor
{
    int rodNumber;

    //Indizes für Dropdown-Auswahl
    int spriteIndex = 0;
    int animationIndex = 0;

    //Vordefinierte Listen
    string[] spriteOptions = new string[] { "Arrow", "Bomb", "Book", "Dagger", "Fireball", "Hammer", "Sword" };
    string[] animationOptions = new string[] {
        "PopUp", "ArrowLeftCrown", "ArrowRightCrown", "ArrowLeftBulwark", "ArrowRightBulwark",
        "AttackLeftCrown", "AttackRightCrown", "AttackLeftBulwark", "AttackRightBulwark",
        "FireballLeftHigh", "FireballRightHigh"
    };

    public override void OnInspectorGUI()
    {
        //Basis Inspector anzeigen
        DrawDefaultInspector();

        ActionRodAnimManager actionRodAnimManager = (ActionRodAnimManager)target;

        //Benutzerobefläche zum Testen der Funktion
        rodNumber = EditorGUILayout.IntField("Rod Number", rodNumber);

        //Dropdown für die Sprites
        spriteIndex = EditorGUILayout.Popup("Sprite Name", spriteIndex, spriteOptions);
        string selectedSprite = spriteOptions[spriteIndex];

        //Dropdown für die Animationen
        animationIndex = EditorGUILayout.Popup("Animation Name", animationIndex, animationOptions);
        string selectedAnimation = animationOptions[animationIndex];

        //Button, um die Animation zu aktivieren
        if (GUILayout.Button("Activate Animation"))
        {
            actionRodAnimManager.TestActivateRodAnimation(rodNumber, selectedSprite, selectedAnimation);
        }
    }
}
