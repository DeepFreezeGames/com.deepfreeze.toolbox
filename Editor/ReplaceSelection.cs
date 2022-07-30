using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    public class ReplaceSelection : ScriptableWizard
    {
        private static GameObject _replacement = null;
        private static bool _keep = false;

        public GameObject replacementObject = null;
        public bool keepOriginals = false;

        [MenuItem("Tools/Replace Selection")]
        private static void CreateWizard()
        {
            DisplayWizard("Replace Selection", typeof(ReplaceSelection), "Replace");
        }

        public ReplaceSelection()
        {
            replacementObject = _replacement;
            keepOriginals = _keep;
        }

        private void OnWizardUpdate()
        {
            _replacement = replacementObject;
            _keep = keepOriginals;
        }

        private void OnWizardCreate()
        {
            if (_replacement == null)
            {
                return;
            }

            Undo.RegisterSceneUndo("Replace Selection");

            var transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);

            foreach (var transform in transforms)
            {
                GameObject spawnedObject;
                var prefab = PrefabUtility.GetPrefabAssetType(_replacement);

                if (prefab is PrefabAssetType.Regular or PrefabAssetType.Model or PrefabAssetType.Variant)
                {
                    spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(_replacement);
                }
                else
                {
                    spawnedObject = Instantiate(_replacement);
                }

                var spawnedObjectTransform = spawnedObject.transform;
                spawnedObjectTransform.parent = transform.parent;
                spawnedObject.name = _replacement.name;
                spawnedObjectTransform.localPosition = transform.localPosition;
                spawnedObjectTransform.localScale = transform.localScale;
                spawnedObjectTransform.localRotation = transform.localRotation;
            }

            if (_keep)
            {
                return;
            }
            
            foreach (var gameObject in Selection.gameObjects)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
