using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CubeArray))]
    public class CubeArrayEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var cubeArr = (CubeArray) target;
            
            base.DrawDefaultInspector();
            
            serializedObject.Update();

            if (GUILayout.Button("Generate"))
            {
                cubeArr.Build();                
            }

            if (GUILayout.Button("Clear"))
            {
                cubeArr.Clear();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
