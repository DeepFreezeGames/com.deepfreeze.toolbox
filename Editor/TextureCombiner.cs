using System.IO;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    public class TextureCombiner : EditorWindow
    {
        //Input textures
        private readonly Texture2D[] _textures = new Texture2D[4];

        //The value for the channels where a texture is not provided
        private float _defaultValue = 1.0f;

        //Output texture
        private Texture2D _generatedTexture;
        private bool _hasAlpha;

        private Vector2 _scrollPos;

        //Dimensions of output texture
        private Vector2Int _textureDimensions;

        //Counter of generated textures used for naming
        private int _totalTextures;

        [MenuItem("Tools/TextureCombiner")]
        private static void ShowWindow()
        {
            var window = GetWindow<TextureCombiner>();
            window.titleContent = new GUIContent("Texture Combiner");
            window.Show();
        }

        private void OnGUI()
        {
            //Displaying the texture fields
            _textures[0] =
                (Texture2D) EditorGUILayout.ObjectField("Texture 1 (R)", _textures[0], typeof(Texture2D), false);
            _textures[1] =
                (Texture2D) EditorGUILayout.ObjectField("Texture 2 (G)", _textures[1], typeof(Texture2D), false);
            _textures[2] =
                (Texture2D) EditorGUILayout.ObjectField("Texture 3 (B)", _textures[2], typeof(Texture2D), false);
            _textures[3] =
                (Texture2D) EditorGUILayout.ObjectField("Texture 4 (A)", _textures[3], typeof(Texture2D), false);

            //Displaying the texture information
            _textureDimensions = EditorGUILayout.Vector2IntField("Dimensions", _textureDimensions);
            _defaultValue = EditorGUILayout.Slider("Default value", _defaultValue, 0.0f, 1.0f);

            if (GUILayout.Button("Generate texture")) 
                GenerateTexture();

            if (GUILayout.Button("Save texture")) 
                SaveGeneratedTexture();

            //Showing preview of the generated texture and its alpha (if it has any)
            if (_generatedTexture == null) 
                return;
            
            EditorGUILayout.LabelField("Generated texture preview");
            EditorGUI.DrawPreviewTexture(new Rect(50, 380, 100, 100), _generatedTexture);
            
            if (_hasAlpha) 
                EditorGUI.DrawTextureAlpha(new Rect(200, 380, 100, 100), _generatedTexture);
        }

        private void GenerateTexture()
        {
            _generatedTexture = null;
            
            if (AllTexturesAreEmpty())
            {
                Debug.LogWarning("No textures were provided, not generating any new textures.");
            }
            else
            {
                _hasAlpha = _textures[3] !=
                            null; //If the 4th texture has been assigned, the generated texture will have an alpha channel
                _generatedTexture = new Texture2D(_textureDimensions.x, _textureDimensions.y,
                    _hasAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);

                for (var i = 0; i < _textureDimensions.x; i++)
                for (var j = 0; j < _textureDimensions.y; j++)
                {
                    var x = i / (float) _textureDimensions.x;
                    var y = j / (float) _textureDimensions.y;

                    //Using GetPixelBilinear so that the size of the output texture does not depend on the size of the input textures.
                    var colR = _textures[0] == null ? _defaultValue : _textures[0].GetPixelBilinear(x, y).r;
                    var colG = _textures[1] == null ? _defaultValue : _textures[1].GetPixelBilinear(x, y).r;
                    var colB = _textures[2] == null ? _defaultValue : _textures[2].GetPixelBilinear(x, y).r;
                    var colA = _textures[3] == null ? _defaultValue : _textures[3].GetPixelBilinear(x, y).r;
                    _generatedTexture.SetPixel(i, j, new Color(colR, colG, colB, colA));
                }

                _generatedTexture.Apply();
            }
        }

        private void SaveGeneratedTexture()
        {
            GenerateTexture();

            //If a directory called "Textures/Generated Textures" doesn't exist, create it
            if (!Directory.Exists(Application.dataPath + "Textures/Generated Textures"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Textures/Generated Textures/");
                _totalTextures = 0;
            }
            else
            {
                _totalTextures = Directory.GetFiles(Application.dataPath + "/Textures/Generated Textures/").Length;
            }

            var bytes = _generatedTexture.EncodeToPNG();
            while (File.Exists(Application.dataPath + "/Textures/Generated Textures/generated_texture_" +
                               _totalTextures + ".png")) _totalTextures++;
            File.WriteAllBytes(
                Application.dataPath + "/Textures/Generated Textures/generated_texture_" + _totalTextures + ".png",
                bytes);
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject =
                AssetDatabase.LoadMainAssetAtPath("Assets/Textures/Generated Textures/generated_texture_" +
                                                  _totalTextures + ".png");
        }

        private bool AllTexturesAreEmpty()
        {
            for (var i = 0; i < 4; i++)
            {
                if (_textures[i] != null)
                    return false;
            }
            
            return true;
        }
    }
}