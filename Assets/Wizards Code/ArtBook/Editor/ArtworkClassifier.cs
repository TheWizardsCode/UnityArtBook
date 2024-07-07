using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using WizardsCode;

/// <summary>
/// Help the user discover and classify new artwork objects.
/// </summary>
internal static class ArtworkClassifier
{

    [MenuItem("Tools/Wizards Code/Art Book/Classify New Art")]
    public static void FindNewArtObjects()
    {
        List<ArtworkObjects> artObjects = new List<ArtworkObjects>(Resources.LoadAll<ArtworkObjects>(BookController.ART_RESOURCES_PATH));

        Texture2D[] images = Resources.LoadAll<Texture2D>(BookController.ART_RESOURCES_PATH);
        if (images == null)
        {
            EditorUtility.DisplayDialog("No New Images", "No unassigned images found", "OK");
            return;
        }

        List<Texture2D> newImages = new List<Texture2D>();
        foreach (Texture2D candidate in images)
        {
            bool isNew = true;
            foreach (ArtworkObjects obj in artObjects)
            {
                foreach (Texture2D image in obj.ConceptArt)
                {
                    if (image == candidate)
                    {
                        isNew = false;
                        continue;
                    }
                }

                if (!isNew)
                {
                    continue;
                }
            }

            if (isNew)
            {
                newImages.Add(candidate);
            }
        }

        if (newImages.Count > 0)
        {
            string continuation = newImages.Count > 1 ? "s were" : " was";
            EditorUtility.DisplayDialog("New Images Found", $"{newImages.Count} new image{continuation} found. Hit OK to create a Art Object record for them.", "OK");

            for (int i = 0; i < newImages.Count; i++)
            {
                int indexOfConceptFor = newImages[i].name.IndexOf("-");
                int indexOfObjectType = newImages[i].name.IndexOf("-", indexOfConceptFor + 1);
                int indexOfTitle = newImages[i].name.IndexOf("-", indexOfObjectType + 1);
                int indexOfImageType = newImages[i].name.IndexOf("-", indexOfTitle + 1);
                int indexOfImageNumber = newImages[i].name.IndexOf("-", indexOfImageType + 1);

                string imageType = newImages[i].name.Substring(indexOfTitle + 1, indexOfImageType - indexOfTitle - 1).Trim();
                if (imageType == "Img2Img")
                {
                    ArtworkObjects obj = ScriptableObject.CreateInstance<ArtworkObjects>();
                    obj.ConceptArt = new Texture2D[] { newImages[i] };

                    obj.ConceptGroup = newImages[i].name.Substring(0, indexOfConceptFor).Trim();

                    string subjectClassification = newImages[i].name.Substring(indexOfConceptFor + 1, indexOfObjectType - indexOfConceptFor - 1).Trim();
                    obj.Classification = Enum.Parse<ArtworkObjects.SubjectClassification>(subjectClassification);


                    obj.SubjectName = newImages[i].name.Substring(indexOfObjectType + 1, indexOfTitle - indexOfObjectType - 1).Trim();

                    string imageNumber = newImages[i].name.Substring(indexOfImageType + 1, newImages[i].name.Length - indexOfImageType - 1).Trim();
                    obj.IDNumber = int.Parse(imageNumber);

                    string folderPath = $"Wizards Code/ArtBook/Resources/{BookController.ART_RESOURCES_PATH}/{obj.ConceptGroup}/{obj.Classification}/";
                    string assetName = $"{obj.SubjectName} - {obj.IDNumber}.asset";

                    string currentPath = "Assets";
                    foreach (string pathSegment in folderPath.Split('/'))
                    {
                        if (string.IsNullOrEmpty(pathSegment))
                        {
                            continue;
                        }

                        if (!AssetDatabase.IsValidFolder($"{currentPath}/{pathSegment}"))
                        {
                            AssetDatabase.CreateFolder(currentPath, pathSegment);
                        }

                        currentPath += $"/{pathSegment}";
                    }

                    AssetDatabase.CreateAsset(obj, $"{currentPath}/{assetName}");
                }
                else
                {
                    Debug.Log($"Skipping image {newImages[i].name} as it is not an image to image art object.");
                }

                EditorUtility.DisplayProgressBar("New Images Found", $"{newImages.Count} new image{continuation} found. Creating Art Object {i} of {newImages.Count}.", i / (float)newImages.Count);

            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return;
        }
        else
        {
            EditorUtility.DisplayDialog("No New Images", "No unassigned images found", "OK");
        }
    }
}