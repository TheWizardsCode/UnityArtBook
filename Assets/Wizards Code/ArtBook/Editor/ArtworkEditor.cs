using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WizardsCode
{
    [CustomEditor(typeof(Artwork))]
    public class ArtworkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Artwork artwork = (Artwork)target;

            string prompt = artwork.Prompt;
            EditorGUILayout.LabelField("", prompt, Styles.Prompt);
            if (GUILayout.Button("Copy"))
            {
                GUIUtility.systemCopyBuffer = prompt;
            }

            EditorGUILayout.Space();

            artwork.SpecifySeed = EditorGUILayout.ToggleLeft("Specify Seed", artwork.SpecifySeed);
            if (artwork.SpecifySeed)
            {
                EditorGUILayout.BeginHorizontal();
                artwork.Seed = EditorGUILayout.IntField("Seed", artwork.Seed);
                if (GUILayout.Button("Randomize"))
                {
                    artwork.Seed = 0;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            artwork.ImagePrompt = EditorGUILayout.TextField("Image Prompt", artwork.ImagePrompt);

            EditorGUILayout.BeginHorizontal();
            artwork.Location = EditorGUILayout.TextField("Location", artwork.Location);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            artwork.Subject = EditorGUILayout.TextField("Subject", artwork.Subject);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            artwork.Action = EditorGUILayout.TextField("Action", artwork.Action);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            artwork.Lighting = EditorGUILayout.TextField("Lighting", artwork.Lighting);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            artwork.ShotFraming = (Artwork.Framing)EditorGUILayout.EnumPopup("Shot Framing", artwork.ShotFraming);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            artwork.Form = (Artwork.Artform)EditorGUILayout.EnumPopup("Art Form", artwork.Form);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }
}
