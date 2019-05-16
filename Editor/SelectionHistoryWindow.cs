using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public class SelectionHistoryWindow : EditorWindow
    {
        private const string MenuName = "Tools/Selection History";
        private const int HistorySize = 20;
        private const float LineHeight = 20f;

        public List<HistoryEntry> history;
        private bool _updateScrollPos;
        private GUIStyle _historyItemStyle;
        private Vector2 _scrollPos;

        [Serializable]
        public class HistoryEntry
        {
            public UnityEngine.Object obj;
            public bool favourite;

            public HistoryEntry(UnityEngine.Object nObj)
            {
                obj = nObj;
                favourite = false;
            }
        }

        [MenuItem(MenuName)]
        public static void ShowWindow()
        {
            GetWindow<SelectionHistoryWindow>(false, "Selection History", true);
        }

        public SelectionHistoryWindow()
        {
            history = new List<HistoryEntry>();
        }

        private void OnSelectionChange()
        {
            if (!Selection.activeObject)
                return;

            var obj = Selection.activeObject;

            _updateScrollPos = true;
            Repaint();

            foreach (var item in history)
            {
                if (item.obj == obj)
                    return;
            }

            history.Insert(0, new HistoryEntry(obj));

            //Nuke the oldest non-fav if over max count
            if (history.Count <= HistorySize) 
                return;
            
            for (var i = history.Count - 1; i > 0; i--)
            {
                if (history[i].favourite) 
                    continue;
                    
                history.RemoveAt(i);
                break;
            }
        }

        public static GUIStyle CreateObjectReferenceStyle()
        {
            var style = new GUIStyle();
            style.contentOffset = new Vector2(4, 0);
            style.fixedHeight = LineHeight;
            style.stretchWidth = true;
            return style;
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
            if (!lineRect.Contains(mousePos)) 
                return;
            
            if (mouseStartDrag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.StartDrag(obj.name);
                DragAndDrop.objectReferences = new[] {obj};
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
                        list.Remove(obj);
                    else
                        list.Add(obj);
                    Selection.objects = list.ToArray();
                }
                else
                    Selection.activeObject = obj;

                Event.current.Use();
            }
        }

        private void OnGUI()
        {
            // Create gui style if needed (has to happen here as forbidden to do in constructor)
            if (_historyItemStyle == null)
            {
                _historyItemStyle = CreateObjectReferenceStyle();
            }

            history.RemoveAll(x => x.obj == null);

            var size = EditorGUILayout.BeginHorizontal();

            var viewHeight = size.height;
            if (_updateScrollPos && Selection.activeObject != null && viewHeight > 0)
            {
                var selectedYPos = GetYPosition(Selection.activeObject);
                _scrollPos.y = Mathf.Clamp(_scrollPos.y, selectedYPos - viewHeight + HistorySize * 2, selectedYPos);
                _updateScrollPos = false;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            GUILayout.BeginVertical();

            DrawHistory(true);
            DrawHistory(false);

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHistory(bool favorites)
        {
            foreach (var item in history)
            {
                if (item.favourite != favorites)
                    continue;

                var o = item.obj;

                EditorGUILayout.BeginHorizontal();

                var newFavorite = GUILayout.Toggle(item.favourite, "", GUILayout.Width(16.0f));
                if (newFavorite != item.favourite)
                    Repaint();
                item.favourite = newFavorite;

                DrawObjectReference(o, _historyItemStyle);

                EditorGUILayout.EndHorizontal();
            }
        }

        private int GetFavoriteCount()
        {
            var count = 0;
            foreach (var item in history)
            {
                if (item.favourite)
                    count++;
            }

            return count;
        }

        private float GetYPosition(UnityEngine.Object obj)
        {
            var favoriteCount = GetFavoriteCount();
            var favIndex = 0;
            var nonFavIndex = 0;
            foreach (var item in history)
            {
                var favorite = item.favourite;

                if (item.obj == obj)
                {
                    var index = favorite ? favIndex : favoriteCount + nonFavIndex;
                    return LineHeight * index + 1;
                }

                if (favorite)
                    favIndex++;
                else
                    nonFavIndex++;
            }

            return 0;
        }
    }
}
