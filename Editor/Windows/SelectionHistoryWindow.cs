using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIExtensions.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor.Windows
{
    public class SelectionHistoryWindow : EditorWindow
    {
        private const string HistorySizePrefKey = "SelectionHistoryWindow_HistorySize";
        private const string TrackFolderPrefKey = "SelectionHistoryWindow_TrackFolders";
        private const string SaveFavouritesPrefKey = "SelectionHistoryWindow_SaveFavourites";
        private const string SaveCachePrefKey = "SelectionHistory_SaveCache";

        private static int historySize;
        private static int HistorySize
        {
            get => historySize;
            set
            {
                historySize = Mathf.Clamp(value, 5, 100);
                EditorPrefs.SetInt(HistorySizePrefKey, historySize);
            }
        }

        private static bool trackFolders;
        private static bool TrackFolders
        {
            get => trackFolders;
            set
            {
                trackFolders = value;
                EditorPrefs.SetBool(TrackFolderPrefKey, trackFolders);
            }
        }

        private static bool saveFavourites;
        private static bool SaveFavourites
        {
            get => saveFavourites;
            set
            {
                saveFavourites = value;
                EditorPrefs.SetBool(SaveFavouritesPrefKey, saveFavourites);
            }
        }

        private const float lineHeight = 20f;

        private readonly List<HistoryEntry> _favourites;
        private readonly List<HistoryEntry> _history;

        private bool _updateScrollPos;
        private GUIStyle _historyItemStyle;
        private bool _editingSettings;

        //Scroll Positions
        private Vector2 _scrollPosMain;
        private Vector2 _scrollPosSettings;

        //Icon Cache
        private GUIContent _iconStar;

        [Serializable]
        public class HistoryEntry
        {
            public UnityEngine.Object obj;
            public bool favourite;

            public HistoryEntry(UnityEngine.Object nObj, bool isFavourite = false)
            {
                obj = nObj;
                favourite = isFavourite;
            }
        }

        #region STARTUP
        [MenuItem("Tools/Selection History")]
        public static void ShowWindow()
        {
            var historyWindow = GetWindow<SelectionHistoryWindow>();
            historyWindow.titleContent = new GUIContent("Selection History");
            historyWindow.minSize = new Vector2(200, 200);
            historyWindow.Show();
            historyWindow.Focus();
        }

        private void OnEnable()
        {
            _iconStar = EditorGUIUtility.IconContent("d_Favorite");
            LoadSettings();
        }

        private void LoadSettings()
        {
            historySize = EditorPrefs.GetInt(HistorySizePrefKey, 20);
            trackFolders = EditorPrefs.GetBool(TrackFolderPrefKey, true);
            saveFavourites = EditorPrefs.GetBool(SaveFavouritesPrefKey, false);
            if (SaveFavourites)
            {
                LoadSavedFavourites(EditorPrefs.GetString(SaveCachePrefKey));
            }
        }

        public SelectionHistoryWindow()
        {
            _favourites = new List<HistoryEntry>();
            _history = new List<HistoryEntry>();
        }
        #endregion

        private void OnSelectionChange()
        {
            if (!Selection.activeObject)
            {
                return;
            }

            var obj = Selection.activeObject;
            RegisterObject(obj);
        }

        private void RegisterObject(Object obj, bool isFavourite = false)
        {
            _updateScrollPos = true;
            Repaint();

            if (!TrackFolders)
            {
                if (obj is DefaultAsset)
                {
                    return;
                }
            }

            //Check if item exists in history
            foreach (var item in _history)
            {
                if (item.obj == obj)
                {
                    return;
                }
            }

            //Check if item exists in favourites
            foreach (var item in _favourites)
            {
                if (item.obj == obj)
                {
                    return;
                }
            }

            if (isFavourite)
            {
                _favourites.Insert(0, new HistoryEntry(obj, true));
                if (SaveFavourites)
                {
                    Save();
                }
            }
            else
            {
                _history.Insert(0, new HistoryEntry(obj));
                if (_history.Count > HistorySize)
                {
                    for (var i = _history.Count - 1; i >= HistorySize; i--)
                    {
                        _history.RemoveAt(i);
                    }
                }
            }
        }

        public static GUIStyle CreateObjectReferenceStyle()
        {
            var style = new GUIStyle
            {
                contentOffset = new Vector2(4, 0),
                fixedHeight = lineHeight,
                stretchWidth = true
            };
            return style;
        }

        private void OnGUI()
        {
            // Create gui style if needed (has to happen here as forbidden to do in constructor)
            if (_historyItemStyle == null)
            {
                _historyItemStyle = CreateObjectReferenceStyle();
            }

            _history.RemoveAll(x => x.obj == null);

            var size = EditorGUILayout.BeginHorizontal();

            var viewHeight = size.height;
            if (_updateScrollPos && Selection.activeObject != null && viewHeight > 0)
            {
                var selectedYPos = GetYPosition(Selection.activeObject);
                _scrollPosMain.y = Mathf.Clamp(_scrollPosMain.y, selectedYPos - viewHeight + HistorySize * 2, selectedYPos);
                _updateScrollPos = false;
            }

            using (new VerticalBlock())
            {
                DrawToolbar();
                if (_editingSettings)
                {
                    DrawSettings();
                    return;
                }
                if (_history.Count == 0 && _favourites.Count == 0)
                {
                    EditorGUILayoutHelper.CenteredMessage("No History Or Favourites");
                }
                else
                {
                    using (new ScrollviewBlock(ref _scrollPosMain))
                    {
                        GUILayout.Label("Favourites", EditorStyles.boldLabel);
                        if (_favourites.Count == 0)
                        {
                            using (new HorizontalBlock())
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("No favourites");
                                GUILayout.FlexibleSpace();
                            }
                        }
                        else
                        {
                            for (var i = 0; i < _favourites.Count; i++)
                            {
                                var favouriteEntry = _favourites[i];
                                DrawEntry(favouriteEntry);
                            }
                        }

                        GUILayout.Label("History", EditorStyles.boldLabel);
                        if (_history.Count == 0)
                        {
                            using (new HorizontalBlock())
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("No history");
                                GUILayout.FlexibleSpace();
                            }
                        }
                        else
                        {
                            for (var i = 0; i < _history.Count; i++)
                            {
                                var historyEntry = _history[i];
                                DrawEntry(historyEntry);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            using (new HorizontalBlock(EditorStyles.toolbar))
            {
                if (_editingSettings)
                {
                    if (GUILayout.Button("Go Back", EditorStyles.toolbarButton))
                    {
                        _editingSettings = false;
                        UpdateListsFromSettings();
                    }
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    if (GUILayout.Button("Settings", EditorStyles.toolbarButton))
                    {
                        _editingSettings = true;
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear History", EditorStyles.toolbarButton))
                    {
                        _history.Clear();
                    }

                    if (GUILayout.Button("Clear Favourites", EditorStyles.toolbarButton))
                    {
                        _favourites.Clear();
                        ClearSavedFavourites();
                    }
                }
            }
        }

        public static void DrawObjectReference(UnityEngine.Object obj, GUIStyle style)
        {
            var cev = Event.current;
            var mousePos = Vector2.zero;
            var mouseLeftClick = false;
            var mouseRightClick = false;
            var mouseStartDrag = false;
            if (cev != null)
            {
                mousePos = cev.mousePosition;
                mouseLeftClick = (cev.type == EventType.MouseUp) && cev.button == 0 && cev.clickCount == 1;
                mouseRightClick = (cev.type == EventType.MouseUp) && cev.button == 1 && cev.clickCount == 1;
                mouseStartDrag = (cev.type == EventType.MouseDrag) && cev.button == 0;
            }

            var guiElement = new GUIContent("impossible!");
            var thumbNail = AssetPreview.GetMiniThumbnail(obj);
            guiElement.image = thumbNail;
            guiElement.text = obj != null ? obj.name : "<invalid>";

            var lineRect = EditorGUILayout.BeginHorizontal();
            var baseCol = obj != null && EditorUtility.IsPersistent(obj) ? new Color(0.5f, 1.0f, 0.5f) : Color.white;

            var selected = ((IList) Selection.objects).Contains(obj);
            style.normal.textColor = selected ? baseCol : baseCol * 0.75f;
            GUILayout.Label(guiElement, style);

            EditorGUILayout.EndHorizontal();

            if (obj == null)
                return;

            // Handle mouse clicks and drags
            if (lineRect.Contains(mousePos))
            {
                if (mouseStartDrag)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.StartDrag(obj.name);
                    DragAndDrop.objectReferences = new UnityEngine.Object[] {obj};
                    Event.current.Use();
                }
                else if (mouseRightClick)
                {
                    EditorGUIUtility.PingObject(obj);
                    Event.current.Use();
                }
                else if (mouseLeftClick)
                {
                    if (Event.current.control)
                    {
                        var list = new List<UnityEngine.Object>(Selection.objects);
                        if (list.Contains(obj))
                        {
                            list.Remove(obj);
                        }
                        else
                        {
                            list.Add(obj);
                        }

                        Selection.objects = list.ToArray();
                    }
                    else
                    {
                        Selection.activeObject = obj;
                    }

                    Event.current.Use();
                }
            }
        }

        private void DrawEntry(HistoryEntry historyEntry)
        {
            using (new HorizontalBlock())
            {
                var historyObj = historyEntry.obj;
                if (historyEntry.favourite)
                {
                    GUI.contentColor = Color.yellow;
                    if(GUILayout.Button(_iconStar, GUILayout.Width(24)))
                    {
                        historyEntry.favourite = false;
                        _favourites.Remove(historyEntry);
                        _history.Add(historyEntry);
                        Repaint();
                    }
                    GUI.contentColor = Color.white;
                }
                else
                {
                    if(GUILayout.Button(_iconStar, GUILayout.Width(24)))
                    {
                        historyEntry.favourite = true;
                        _history.Remove(historyEntry);
                        _favourites.Add(historyEntry);
                        if (SaveFavourites)
                        {
                            Save();
                        }
                        Repaint();
                    }
                }

                DrawObjectReference(historyObj, _historyItemStyle);
            }
        }

        private float GetYPosition(UnityEngine.Object obj)
        {
            var favIndex = 0;
            var nonFavIndex = 0;
            foreach (var item in _history)
            {
                var favorite = item.favourite;

                if (item.obj == obj)
                {
                    var index = favorite ? favIndex : _favourites.Count + nonFavIndex;
                    return lineHeight * index + 1;
                }

                if (favorite)
                {
                    favIndex++;
                }
                else
                {
                    nonFavIndex++;
                }
            }

            return 0;
        }

        private void DrawSettings()
        {
            using (new ScrollviewBlock(ref _scrollPosSettings))
            {
                HistorySize = EditorGUILayout.IntField("History Size", HistorySize);
                TrackFolders = EditorGUILayout.Toggle("Track Folders", TrackFolders);
                SaveFavourites = EditorGUILayout.Toggle("Save Favourites", SaveFavourites);
            }
        }

        private void UpdateListsFromSettings()
        {
            //Resize history list
            if (_history.Count > historySize)
            {
                for (var i = _history.Count - 1; i >= historySize; i--)
                {
                    _history.RemoveAt(i);
                }
            }

            //Filter out folders
            if (!TrackFolders)
            {
                for (var i = 0; i < _history.Count; i++)
                {
                    var entry = _history[i];
                    if (entry.obj is DefaultAsset)
                    {
                        _history.Remove(entry);
                    }
                }
            }

            //Force save
            if (SaveFavourites)
            {
                Save();
            }
            else
            {
                ClearSavedFavourites();
            }
        }

        private void Save()
        {
            EditorPrefs.SetString(SaveCachePrefKey, GetSaveString());
        }

        private void ClearSavedFavourites()
        {
            EditorPrefs.SetString(SaveCachePrefKey, "");
        }

        private void LoadSavedFavourites(string rawSave)
        {
            var items = rawSave.Split(',');
            foreach (var item in items.Reverse())
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var path = AssetDatabase.GetAssetPath(int.Parse(item.Replace(",", "")));
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                RegisterObject(obj, true);
            }
        }

        private string GetSaveString()
        {
            var saveString = "";
            for (var i = 0; i < _favourites.Count; i++)
            {
                if (_favourites[i].obj == null)
                {
                    continue;
                }

                if (i != 0)
                {
                    saveString += ",";
                }
                saveString += $"{_favourites[i].obj.GetInstanceID().ToString()}";
            }

            return saveString;
        }
    }
}
