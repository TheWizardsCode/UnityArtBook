using echo17.EndlessBook;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static WizardsCode.ArtworkObjects;

namespace WizardsCode
{
    public class BookController : MonoBehaviour
    {
        public const string ART_RESOURCES_PATH = "ArtObjects";

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
            ArtworkObjects[] artObjects = Resources.LoadAll<ArtworkObjects>(ART_RESOURCES_PATH);
            if (artObjects == null)
            {
                Debug.LogError("No Artwork Objects Found.");
                return;
            }

            foreach (SubjectClassification classification in Enum.GetValues(typeof(SubjectClassification)))
            {
                // iterate over the images creating a page in the book for each one
                foreach (ArtworkObjects artObject in artObjects)
                {
                    if (artObject.Classification != classification) continue; 
                        
                    foreach (Texture2D image in artObject.ConceptArt)
                    {
                        if (image == null) continue;

                        pageNumber++;
                        Material material = new Material(m_basePageMaterial);
                        material.mainTexture = image;

                        PageData page = book.AddPageData();
                        page.material = material;
                        page.

                        book.SetPageNumber(pageNumber);
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
    }
}