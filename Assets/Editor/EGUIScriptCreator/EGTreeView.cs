using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace EG
{
    public class EGTreeView : TreeView
    {
        public EGTreeViewItem<EGTreeElement> root;
        public string[] events;
        public int compSelectIndex;
        public int eventSelectIndex;

        public EGTreeView(TreeViewState state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
        }

        public EGTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) :
            base(state, multiColumnHeader)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            columnIndexForTreeFoldouts = 2;
            rowHeight = 25;
            customFoldoutYOffset = (30 - EditorGUIUtility.singleLineHeight) * 0.5f;
            extraSpaceBeforeIconAndLabel = 30;
        }

        protected override TreeViewItem BuildRoot()
        {
            if (root == null)
            {
                root = new EGTreeViewItem<EGTreeElement>(new EGTreeElement("Root", -1, -1, null));
                var ch = new EGTreeViewItem<EGTreeElement>(new EGTreeElement("Nothing!!!", 0, 0, null));
                root.AddChild(ch);
            }

            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            var item = (EGTreeViewItem<EGTreeElement>) args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                var cellRect = args.GetCellRect(i);
                CellGUI(cellRect, item, args.GetColumn(i), ref args);
            }
        }

        private bool CheckSelectChild(List<TreeViewItem> children)
        {
            if (children == null)
            {
                return false;
            }

            foreach (var egTreeViewItem in children)
            {
                var item = egTreeViewItem as EGTreeViewItem<EGTreeElement>;
                if (item.data.selected)
                {
                    return true;
                }

                bool res = CheckSelectChild(egTreeViewItem.children);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        void CellGUI(Rect cellRect, EGTreeViewItem<EGTreeElement> item, int column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case 0:
                {
                    // Default icon and label
                    if (CheckSelectChild(item.children))
                    {
                        GUI.DrawTexture(cellRect, item.data.icon, ScaleMode.ScaleToFit);
                    }

                    break;
                }
                case 1:
                {
                    if (item.data.state != null)
                    {
                        GUI.DrawTexture(cellRect, item.data.state,
                            ScaleMode.ScaleToFit);
                    }

                    break;
                }
                case 2:
                {
                    Rect toggleRect = cellRect;
                    toggleRect.x += GetContentIndent(item);
                    toggleRect.width = 150;
                    if (toggleRect.xMax < cellRect.xMax)
                    {
                        bool old = item.data.selected;
                        item.data.selected = GUI.Toggle(toggleRect, item.data.selected, item.data.content);
                        if (old != item.data.selected)
                        {
                            foreach (var egTreeElement in EGTreeElement.allElements)
                            {
                                egTreeElement.CheckState();
                            }

                            CellGUI(cellRect, item, column, ref args);
                        }
                    }

                    args.rowRect = cellRect;
                    base.RowGUI(args);
                    break;
                }
                case 3:
                {
                    // Rect toggleRect = cellRect;
                    // toggleRect.x += GetContentIndent(item);
                    // toggleRect.width = 150;
                    // if (toggleRect.xMax < cellRect.xMax)
                    if (item.data.gameObjectData != null)
                    {
                        item.data.compType = EditorGUI.Popup(cellRect, "", item.data.compType,
                            item.data.gameObjectData.components.ToArray());
                    }
                    else
                    {
                    }

                    // args.rowRect = cellRect;
                    // base.RowGUI(args);
                    break;
                }
                case 4:
                {
                    item.data.eventType = EditorGUI.Popup(cellRect, "", item.data.eventType, events);
                    break;
                }
                case 5:
                {
                    EditorGUI.LabelField(cellRect, item.data.tips);
                    break;
                }
            }
        }
    }
}