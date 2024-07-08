using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace WizardsCode.ArtBook
{
    [CreateAssetMenu(fileName = "New Artwork", menuName = "Wizards Code/Artwork")]
    public class Artwork : ScriptableObject
    {
        public enum Artform { Undefined, ConceptArt, CharacterConceptArt, Sketch, TradingCard }
        public enum Framing { Undefined = 0, SideView = 10, TopView = 20, AerialView = 30,  
            CloseUpView = 100, ExtremeCloseup = 110, LongShot = 120, ExtremeLongShot = 130,
            MassiveScale = 300, Landscape = 310, Panoramic = 320, Bokeh = 330, Fisheye = 340,
            DutchAngle = 400, LowAngle  = 410, HighAngle = 420, WideAngle = 430}

        [SerializeField, Tooltip("Image prompts for this artwork.")]
        public string ImagePrompt;
        [SerializeField, Tooltip("The location for the artwork. This will be used to creat the prompt.")]
        string m_Location;
        [SerializeField, Tooltip("The subject part of the prompt used to create this artwork.")]
        string m_Subject;
        [SerializeField, Tooltip("The action that is happening in the image, if any.")]
        string m_Action;
        [SerializeField, Tooltip("A description of the lighting to apply to the scene.")]
        private string m_Lighting; // accent lighting, backlight, blacklight, blinding light, candlelight, concert lighting, crepuscular rays, direct sunlight, dusk, Edison bulb, electric arc, fire, fluorescent, glowing, glowing radioactively, glow-stick, lava glow, moonlight, natural lighting, neon lamp, nightclub lighting, nuclear waste glow, quantum dot display, spotlight, strobe, sunlight, ultraviolet, dramatic lighting, dark lighting, soft lighting
        [SerializeField, Tooltip("Any additional artists that you want to be included in this prompt. Some ArtForms will define artists automatically.")]
        public string ByArtists;
        [SerializeField, Tooltip("The camera angle to use for this artwork.")]
        public Framing ShotFraming;
        [SerializeField, Tooltip("The form of the artwork. This will define things like the artist style, the size of the artwork, the number of colours, etc.")]
        public Artform Form;
        [SerializeField, Tooltip("A comma separated list of things that should not be in the scene.")]
        public string No;
        [SerializeField, Tooltip("Use a specified seed (set to tru) or a random one (set to false).")]
        public bool SpecifySeed = true;
        [SerializeField, Tooltip("The seed used to generate this image. This will initially be randomly generated but feel free to override it.")]
        int m_Seed;
        [SerializeField, Tooltip("Additional parameters to be added at the end of the prompt. These will appear before any that are automatically provided by the generator and thus will take precedence.")]
        public string AdditionalParameters;
        
        public string Prompt
        {
            get {
                // Anatomy of a prompt [Image Prompts] [Content type] [Description] [Style] [Composition] [Parameters]
                // from https://medium.com/mlearning-ai/the-anatomy-of-an-ai-art-prompt-dcf7d124406d
                return $"{ImagePrompt} {ContentType} {Description} {Style} {Composition} {PromptParameters}"; 
            }
        }
        
        public string NoParam
        {
            get
            {
                if (string.IsNullOrEmpty(No)) {
                    return string.Empty;
                }
                StringBuilder sb = new StringBuilder();
                string[] elements = No.Split(',');
                foreach (string element in elements) 
                {
                    sb.Append(" --no ");
                    sb.Append(element.Trim());
                }
                return sb.ToString();
            }
            set
            {
                No = value;
            }
        }

        public string Composition
        {
            get
            {
                // Examples: 
                return $"{CameraFraming} {Resolution}";
            }
        }

        public string Resolution
        {
            get
            {
                // Examples: highly detailed, depth of field (or dof), 4k, 8k uhd, ultra realistic, studio quality. 
                return $"";
            }
        }

        public string Style
        {
            get
            {
                return $"{Lighting} {Detail} {ArtStyle} {Technique} {Artists}";
            }
        }

        public string Technique
        {
            get
            {
                // Examples: Digital art, digital painting, color page, featured on pixiv (for anime/manga), trending on artstation, precise line-art, tarot card, character design, concept art, symmetry, golden ratio, evocative, award winning, shiny, smooth, surreal, divine, celestial, elegant, oil painting, soft, fascinating, fine art 
                return $"";
            }
        }

        public string ArtStyle
        {
            get
            {
                // Examples: Abstract, Medieval art, Renaissance, Baroque, Rococo, Neoclassicism, Romanticism, Impressionism, post-Expression, Cubism, Futurism, Art Deco, Abstract Expressionism, Contemporary, pop art, surrealism, fantasy
                return $"";
            }
        }

        public string Detail
        {
            get
            {
                // Examples: highly detailed, grainy, realistic, unreal engine, octane render, bokeh, vray, houdini render, quixel megascans, depth of field (or dof), arnold render, 8k uhd, raytracing, cgi, lumen reflections, cgsociety, ultra realistic, volumetric fog, overglaze, analog photo, polaroid, 100mm, film photography, dslr, cinema4d, studio quality
                return $"";
            }
        }

        public string Description
        {
            get
            {
                return $"{Location} {Subject} {Action}";
            }
        }

        public string CameraFraming
        {
            get
            {
                StringBuilder result = new StringBuilder();
                switch (ShotFraming)
                {
                    case Framing.Undefined:
                        break;
                    case Framing.SideView:
                        result.Append("Side-View,");
                        break;
                    case Framing.TopView:
                        result.Append("Top-View,");
                        break;
                    case Framing.AerialView:
                        result.Append("Aerial View,");
                        break;
                    case Framing.CloseUpView:
                        result.Append("Closeup-View,");
                        break;
                    case Framing.ExtremeCloseup:
                        result.Append("Extreme Closeup,");
                        break;
                    case Framing.LongShot:
                        result.Append("Long Shot,");
                        break;
                    case Framing.ExtremeLongShot:
                        result.Append("Extreme long Shot,");
                        break;
                    case Framing.MassiveScale:
                        result.Append("Massive scale shot,");
                        break;
                    case Framing.Landscape:
                        result.Append("Landscape,");
                        break;
                    case Framing.Panoramic:
                        result.Append("Panaoramic,");
                        break;
                    case Framing.Bokeh:
                        result.Append("Bokeh,");
                        break;
                    case Framing.Fisheye:
                        result.Append("Fish eye,");
                        break;
                    case Framing.DutchAngle:
                        result.Append("Dutch Angle,");
                        break;
                    case Framing.LowAngle:
                        result.Append("Low-Angle,");
                        break;
                    case Framing.HighAngle:
                        result.Append("High-Angle,");
                        break;
                    case Framing.WideAngle:
                        result.Append("Wide-Angle,");
                        break;
                    default:
                        Debug.LogWarning($"Unkown Shot Angle of {ShotFraming} providing now framing information in prompt.");
                        break;
                }

                return result.ToString();
            }
        }

        public string Location
        {
            get
            {
                if (!string.IsNullOrEmpty(m_Location))
                {
                    return $"{m_Location}";
                } else
                {
                    return "";
                }
            }
            set
            {
                m_Location = value;
            }
        }

        public string Subject
        {
            get
            {
                if (!string.IsNullOrEmpty(m_Subject))
                {
                    return $"{m_Subject}";
                }
                else
                {
                    return "";
                }
            }
            set
            {
                m_Subject = value;
            }
        }

        public string Action
        {
            get
            {
                if (!string.IsNullOrEmpty(m_Action))
                {
                    return $"{m_Action}";
                }
                else
                {
                    return "";
                }
            }
            set
            {
                m_Action = value;
            }
        }

        public string Lighting
        {
            get
            {
                if (!string.IsNullOrEmpty(m_Lighting))
                {
                    return $"{m_Lighting}";
                }
                else
                {
                    return "";
                }
            }
            set
            {
                m_Lighting = value;
            }
        }

        public string ContentType {

            get
            {
                switch (Form)
                {
                    case Artform.ConceptArt:
                        return $"";
                    case Artform.CharacterConceptArt:
                        return $"A portrait photograph in front of a solid black screen.";
                    case Artform.TradingCard:
                        return $"A digital render.";
                    case Artform.Sketch:
                        return $"An outline sketch.";
                    default:
                        return "";
                }
            }
        }

        public string Artists { 
            get
            {
                string artists = string.Empty;
                if (!string.IsNullOrEmpty(ByArtists))
                {
                    artists = ByArtists;
                }
                
                switch (Form)
                {
                    case Artform.ConceptArt:
                        return $"By {artists}";
                    case Artform.CharacterConceptArt:
                        return $"By {artists}";
                    case Artform.TradingCard:

                        if (string.IsNullOrEmpty(artists))
                        {
                            return $"By Alvin Lee and Jasmine Becket-Griffith.";
                        }
                        else
                        {
                            return $"By {artists}, Alvin Lee and Jasmine Becket-Griffith.";
                        }
                    default:
                        if (string.IsNullOrEmpty(artists))
                        {
                            return $"By {artists}.";
                        }
                        else
                        {
                            return $"By {artists}.";
                        }
                }
            }
        }
        public string PromptParameters {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($" {AdditionalParameters}");

                sb.Append($" {NoParam}");

                if (!SpecifySeed)
                {
                    Seed = 0;    
                }
                sb.Append($" --seed {Seed}");

                switch (Form)
                {
                    case Artform.ConceptArt:
                        break;
                    case Artform.CharacterConceptArt:
                        break;
                    case Artform.TradingCard:
                        // sb.Append(" --ar 320:256");
                        break;
                    default:
                        break;
                }
                return sb.ToString();
            }
        }

        public int Seed
        {
            get
            {
                if (m_Seed == 0)
                {
                    m_Seed = (int)(Random.value * int.MaxValue);
                }
                return m_Seed;
            }
            set
            {
                m_Seed = value;
            }
        }
    }
}
