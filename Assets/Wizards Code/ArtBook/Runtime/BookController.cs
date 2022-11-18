using echo17.EndlessBook;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static WizardsCode.WorldObject;

namespace WizardsCode
{
    public class BookController : MonoBehaviour
    {
        private const string ResourcesPath = "World Objects";
        [SerializeField, Tooltip("The book prefab we are building from.")]
        EndlessBook m_bookPrefab;
        [SerializeField, Tooltip("The base matierla to use for pages. When creating pages this material will be used, with the main material replaced with the appropriate image.")]
        Material m_basePageMaterial;

        [Header("Page Turning")]
        [SerializeField, Tooltip("The time between page turning.")]
        float m_timeBetweenPageTurns = 5f;
        [SerializeField, Tooltip("The time it takes to tun a page.")]
        float m_durationOfPageTurn = 1f;

        EndlessBook book;
        float timeOfNextTurn;
        Boolean flipForward = true;

        void Start()
        {
            // Create a book instance
            book = Instantiate(m_bookPrefab);
            int pageNumber = 0;

            // Load all WorldObjects from resources folder
            WorldObject[] worldObjects = Resources.LoadAll<WorldObject>(ResourcesPath);
            if (worldObjects == null)
            {
                Debug.LogError("No WorldObjects Found.");
                return;
            }

            foreach (ObjectRace race in Enum.GetValues(typeof(ObjectRace))) {
                //TODO: Add a Race Section Header
                foreach (ObjectClass objClass in Enum.GetValues(typeof(ObjectClass)))
                {
                    // iterate over the images creating a page in the book for each one
                    foreach (WorldObject worldObject in worldObjects)
                    {
                        if (worldObject.Race != race || worldObject.Class != objClass) continue; 
                        
                        foreach (Texture2D image in worldObject.ConceptArt)
                        {
                            if (image == null) continue;

                            pageNumber++;
                            Material material = new Material(m_basePageMaterial);
                            material.mainTexture = image;

                            PageData page = book.AddPageData();
                            page.material = material;

                            book.SetPageNumber(pageNumber);
                        }
                    }
                }
            }

            // open the book
            book.SetPageNumber(1);
            book.SetState(EndlessBook.StateEnum.ClosedFront);
        }

        private void Update()
        {
            // if it is time to turn the page, do so
            if (timeOfNextTurn > Time.timeSinceLevelLoad)
            {
                return;
            }

            switch (book.CurrentState)
            {
                case EndlessBook.StateEnum.ClosedFront:
                    flipForward = true;
                    book.SetState(EndlessBook.StateEnum.OpenFront);
                    break;
                case EndlessBook.StateEnum.OpenFront:
                    if (flipForward)
                    {
                        book.SetState(EndlessBook.StateEnum.OpenMiddle);
                    }
                    else
                    {
                        book.SetState(EndlessBook.StateEnum.ClosedFront);
                    }
                    break;
                case EndlessBook.StateEnum.OpenMiddle:
                    if (flipForward)
                    {
                        if (book.CurrentPageNumber + 1 < book.LastPageNumber)
                        {
                            book.TurnForward(m_durationOfPageTurn);
                        } else
                        {
                            book.SetState(EndlessBook.StateEnum.OpenBack);
                        }
                    }
                    else
                    {
                        if (book.CurrentPageNumber > 1)
                        {
                            book.TurnBackward(m_durationOfPageTurn);
                        }
                        else
                        {
                            book.SetState(EndlessBook.StateEnum.OpenFront);
                        }
                    }
                    break;
                case EndlessBook.StateEnum.ClosedBack:
                    flipForward = false;
                    book.SetState(EndlessBook.StateEnum.OpenBack);
                    break;
                case EndlessBook.StateEnum.OpenBack:
                    if (flipForward)
                    {
                        book.SetState(EndlessBook.StateEnum.ClosedBack);
                    }
                    else
                    {
                        book.SetState(EndlessBook.StateEnum.OpenMiddle);
                    }
                    break;
            }
            
            timeOfNextTurn = Time.timeSinceLevelLoad + m_timeBetweenPageTurns;
        }

        [MenuItem("Tools/Wizards Code/TCG/Find New World Objects")]
        public static void FindNewWorldObjects()
        {
            List<WorldObject> worldObjects = new List<WorldObject>(Resources.LoadAll<WorldObject>(ResourcesPath));
            
            Texture2D[] images = Resources.LoadAll<Texture2D>(ResourcesPath);
            if (images == null)
            {
                EditorUtility.DisplayDialog("No New Images", "No unassigned images found", "OK");
                return;
            }

            List<Texture2D> newImages = new List<Texture2D>();
            foreach (Texture2D candidate in images)
            {
                bool isNew = true;
                foreach (WorldObject obj in worldObjects)
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
                EditorUtility.DisplayDialog("New Images Found", $"{newImages.Count} new image{continuation} found. Hit OK to create a WorldObject record for them.", "OK");

                for (int i = 0; i < newImages.Count; i++)
                {
                    WorldObject obj = ScriptableObject.CreateInstance<WorldObject>();
                    obj.ConceptArt = new Texture2D[] { newImages[i] };
                    EditorUtility.DisplayProgressBar("New Images Found", $"{newImages.Count} new image{continuation} found. Creating WorldObject {i} of {newImages.Count}.", i / (float)newImages.Count);

                    AssetDatabase.CreateAsset(obj, $"Assets/Wizards Code/Resources/{ResourcesPath}/TBD_{newImages[i].name}.asset");
                }
                EditorUtility.ClearProgressBar();
                return;
            } else
            {
                EditorUtility.DisplayDialog("No New Images", "No unassigned images found", "OK");
            }
        }
    }
}