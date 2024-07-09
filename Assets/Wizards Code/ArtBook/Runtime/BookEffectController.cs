using BookEffectV3;
using System;
using UnityEngine;
using static WizardsCode.ArtBook.ArtworkObjects;

namespace WizardsCode.ArtBook
{
    /// <summary>
    /// A version of the book controller that uses the Book Effect asset. This asset has limited animations, but
    /// the book can be resized to accomodate different page sizes.
    /// </summary>
    public class BookEffectController : MonoBehaviour
    {
        public const string ART_RESOURCES_PATH = "ArtObjects";

        [Header("Book")]
        [SerializeField, Tooltip("The book prefab we are building from.")]
        BookMeshBuilder m_bookPrefab;
        [SerializeField, Tooltip("The texture to use for the title page.")]
        Texture2D m_TitlePage = null;
        [SerializeField, Tooltip("The texture to use when a blank page is required.")]
        Texture2D m_BlankPage = null;
        
        [Header("Page Turning")]
        [SerializeField, Tooltip("The time between page turning."), Range(1, 60)]
        public float timeBetweenPageTurns = 5f;
        [SerializeField, Tooltip("The time it takes to tun a page."), Range(1, 10)]
        float m_durationOfPageTurn = 2f;

        [Header("User Controls")]
        [SerializeField, Tooltip("Allow the user to control the book manually using shortcut keys.")]
        bool m_AllowShortcutKeys = true;

        BookMeshBuilder book;
        bool autoTurnPage = true;
        float timeOfNextTurn;
        Boolean flipForward = true;

        public float TimeBetweenPageTurns
        {
            get { return timeBetweenPageTurns; }
            set { 
                if (timeBetweenPageTurns == value)
                {
                    return;
                }

                timeOfNextTurn = Mathf.Min(timeOfNextTurn, Time.timeSinceLevelLoad + value);
                timeBetweenPageTurns = Mathf.Max(value, m_durationOfPageTurn * 2f);
            }
        }

        void Start()
        {
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
                AddSection(artObjects, classification);
            }

            book.BuildDetails.Pages.Add(m_BlankPage);
            if (book.BuildDetails.Pages.Count % 2 != 0)
            {
                book.BuildDetails.Pages.Add(m_BlankPage);
            }
        }

        private void AddSection(ArtworkObjects[] artObjects, SubjectClassification classification)
        {
            foreach (ArtworkObjects artObject in artObjects)
            {
                if (artObject.Classification != classification) continue;
                AddPages(artObject);
            }
        }

        private void AddPages(ArtworkObjects artObject)
        {
            foreach (Texture2D image in artObject.ConceptArt)
            {
                if (image == null) continue;

                book.BuildDetails.Pages.Add(image);
            }
        }

        bool isFlippingForward = true;
        private void Update()
        {
            KeysInput();
            AutoPageTurnUpdate();
        }

        private void KeysInput()
        {
            if (m_AllowShortcutKeys)
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    OpenBook();
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    CloseBook();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    TurnPageBackward();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    TurnPageForward();
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    ToggleAutoPageTurn();
                }
            }
        }

        public void ToggleAutoPageTurn()
        {
            autoTurnPage = !autoTurnPage;
        }

        private void AutoPageTurnUpdate()
        {
            if (!autoTurnPage || timeOfNextTurn > Time.timeSinceLevelLoad)
            {
                return;
            }
            book.SetSpeed(m_durationOfPageTurn);

            if (book.IsBookOpen == false)
            {
                OpenBook();
            }
            else if (isFlippingForward && book.CanTurnPageForward)
            {
                TurnPageForward();
            }
            else if (book.CanTurnPageBackWard)
            {
                isFlippingForward = false;
                TurnPageBackward();
            }
            else
            {
                isFlippingForward = true;
                CloseBook();
            }

            timeOfNextTurn = Time.timeSinceLevelLoad + TimeBetweenPageTurns;
        }

        public void OpenBook()
        {
            book.OpenBook();
            timeOfNextTurn = Time.timeSinceLevelLoad + TimeBetweenPageTurns;
        }

        public void CloseBook()
        {
            book.CloseBook();
            timeOfNextTurn = Time.timeSinceLevelLoad + TimeBetweenPageTurns;
        }

        public void TurnPageBackward()
        {
            book.TurnPageBack();
            timeOfNextTurn = Time.timeSinceLevelLoad + TimeBetweenPageTurns;
        }

        public void TurnPageForward()
        {
            book.TurnPage();
            timeOfNextTurn = Time.timeSinceLevelLoad + TimeBetweenPageTurns;
        }
    }
}