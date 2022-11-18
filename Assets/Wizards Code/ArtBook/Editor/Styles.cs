using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WizardsCode
{
    public static class Styles
    {
        public static GUIStyle Prompt
        {
            get
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.wordWrap = true;
                return style;
            }
        }
    }
}
