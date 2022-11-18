using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace WizardsCode
{
    [CreateAssetMenu(fileName = "New World Object", menuName = "Wizards Code/World Object")]
    public class WorldObject : ScriptableObject
    {
        public enum ObjectRace { Orc, DarkElf, Human, TBD = 999}
        public enum ObjectClass { LocationExterior, LocationInterior, Character, TBD = 999 }
        
        [SerializeField, Tooltip("The type of object this is. This is used to group like items.")]
        [FormerlySerializedAs("m_objectType")]
        ObjectClass m_objectClass;
        [SerializeField, Tooltip("The short name of the object, to be used when UI space is limited.")]
        string m_ShortName = "TBD";
        [SerializeField, Tooltip("The full name of the object, to be used when UI space is not limited.")]
        string m_LongName = "TBD";
        [SerializeField, Tooltip("The race that this object is associated with.")]
        ObjectRace m_Race = ObjectRace.TBD;
        [Obsolete("Use m_Art instead"), SerializeField, Tooltip("One or more images that will be used to display the object in game design documents.")]
        Texture2D[] m_ConceptArt;
        [SerializeField, Tooltip("One or more images that will be used to display the object.")]
        Artwork[] m_Art;

        public ObjectRace Race
        {
            get { return m_Race; }
        }
        
        public ObjectClass Class
        {
            get { return m_objectClass; }
        }

        public Texture2D[] ConceptArt
        {
            get { return m_ConceptArt; }
            internal set { m_ConceptArt = value; } 
        }
    }
}
