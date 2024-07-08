using BookEffectV3;
using System;
using UnityEngine;
using static WizardsCode.ArtworkObjects;

namespace WizardsCode
{
    /// <summary>
    /// A version of the book controller that uses the Book Effect asset. This asset has limited animations, but
    /// the book can be resized to accomodate different page sizes.
    /// </summary>
    public class BookEffectController : MonoBehaviour
    {
        public const string ART_RESOURCES_PATH = "ArtObjects";

        [SerializeField, Tooltip("The book prefab we are building from.")]
        BookMeshBuilder m_bookPrefab;

        [SerializeField, Tooltip("The texture to use for the title page.")]
        Texture2D m_TitlePage = null;

        [SerializeField, Tooltip("The texture to use when a blank page is required.")]
        Texture2D m_BlankPage = null;
        
        [Header("Page Turning")]
        [SerializeField, Tooltip("The time between page turning.")]
        float m_timeBetweenPageTurns = 5f;
        [SerializeField, Tooltip("The time it takes to tun a page.")]
        float m_durationOfPageTurn = 1f;

        BookMeshBuilder book;
        float timeOfNextTurn;
        Boolean flipForward = true;

        void Start()
        {
            // Create a book instance
            book = Instantiate(m_bookPrefab);
            
            // Load all WorldObjects from resources folder
            ArtworkObjects[] artObjects = Resources.LoadAll<ArtworkObjects>(ART_RESOURCES_PATH);
            if (artObjects == null)
            {
                Debug.LogError("No Artwork Objects Found.");
                return;
            }

            book.BuildDetails.Pages.Add(m_BlankPage);
            book.BuildDetails.Pages.Add(m_TitlePage);

            foreach (SubjectClassification classification in Enum.GetValues(typeof(SubjectClassification)))
            {
                // iterate over the images creating a page in the book for each one
                foreach (ArtworkObjects artObject in artObjects)
                {
                    if (artObject.Classification != classification) continue; 
                        
                    foreach (Texture2D image in artObject.ConceptArt)
                    {
                        if (image == null) continue;

                        book.BuildDetails.Pages.Add(image);
                    }
                }
            }

            book.BuildDetails.Pages.Add(m_BlankPage);
            if (book.BuildDetails.Pages.Count % 2 != 0)
            {
                book.BuildDetails.Pages.Add(m_BlankPage);
            }
        }

        bool isFlippingForward = true;
        private void Update()
        {
            if (timeOfNextTurn > Time.timeSinceLevelLoad)
            {
                return;
            }

            if (book.IsBookOpen == false)
            {
                book.OpenBook();
            }
            else if (isFlippingForward && book.CanTurnPageForward)
            {
                book.TurnPage();
            } else if (book.CanTurnPageBackWard)
            {
                isFlippingForward = false;
                book.TurnPageBack();
            } else
            {
                book.CloseBook();
            }

            timeOfNextTurn = Time.timeSinceLevelLoad + m_timeBetweenPageTurns;
        }
    }
}