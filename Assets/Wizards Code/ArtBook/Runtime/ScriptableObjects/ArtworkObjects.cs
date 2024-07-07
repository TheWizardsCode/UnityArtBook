using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace WizardsCode
{
    [CreateAssetMenu(fileName = "New World Object", menuName = "Wizards Code/World Object")]
    public class ArtworkObjects : ScriptableObject
    {
        public enum SubjectClassification { Location = 100, Character = 200, TBD = 999 }

        [SerializeField, Tooltip("The concept group this object belongs to. This is used to group like items.")]
        String m_ConceptGroup = "TDB";
        [SerializeField, Tooltip("The type of object this is. This is used to group like items.")]
        SubjectClassification m_classification = SubjectClassification.TBD;
        [SerializeField, Tooltip("The name of the subject of the artwork.")]
        string m_SubjectName = "TBD";
        [SerializeField, Tooltip("One or more images that will be used to display the object in game design documents.")]
        Texture2D[] m_ConceptArt;
        [SerializeField, Tooltip("One or more images that will be used to display the object.")]
        Artwork[] m_Art;
        [SerializeField, Tooltip("A unique number for this image, in conjuction with the concept group and subject name this forms a UID.")]
        int m_idNumber = 0;

        public int IDNumber
        {
            get { return m_idNumber; }
            set { m_idNumber = value; }
        }

        public string ConceptGroup
        {
            get { return m_ConceptGroup; }
            set { m_ConceptGroup = value; }
        }

        public string SubjectName
        {
            get { return m_SubjectName; }
            set { m_SubjectName = value; }
        }
        
        public SubjectClassification Classification
        {
            get { return m_classification; }
            set { m_classification = value; }
        }

        public Artwork[] Artwork
        {
            get { return m_Art; }
            internal  set { m_Art = value; }
        }

        [Obsolete("No longer storing concept art in the object. We instead store the Artwork definition.")]
        public Texture2D[] ConceptArt
        {
            get { return m_ConceptArt; }
            set { m_ConceptArt = value; } 
        }
    }
}
