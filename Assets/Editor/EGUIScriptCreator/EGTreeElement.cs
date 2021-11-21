using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EG
{
    [Serializable]
    public class EGTreeElement : TreeElement
    {
        public static List<EGTreeElement> allElements;
        private static int curId = 0;
        public bool selected;
        public Texture icon;
        public Texture state;
        public string content;
        public int compType;
        public int eventType;
        public GameObjectData gameObjectData;
        public string tips;

        public EGTreeElement(string name, int depth, int id, GameObjectData gameObjectData) : base(name, depth, id)
        {
            icon = EditorGUIUtility.IconContent("Folder Icon").image;
            selected = CheckParent(gameObjectData);
            content = name;
            compType = gameObjectData?.firstShowComp ?? 0;
            eventType = 0;
            this.gameObjectData = gameObjectData;
            allElements = allElements == null ? new List<EGTreeElement>() : allElements;
            CheckState();
            allElements.Add(this);
        }

        public void CheckState()
        {
            if (!selected || gameObjectData == null)
            {
                state = null;
                tips = "";
                return;
            }

            var temp = allElements.Find(e =>
                e.selected && e.gameObjectData.gameObjectName == gameObjectData.gameObjectName);
            if (temp != null && temp != this)
            {
                state = EditorGUIUtility.IconContent("sv_icon_dot6_pix16_gizmo").image;
                tips = "name has already exists!!!";
                return;
            }

            if (EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex].componentSettings
                .Find(e => e.componentName == gameObjectData.gameObjectName) != null)
            {
                state = EditorGUIUtility.IconContent("sv_icon_dot6_pix16_gizmo").image;
                tips = "name duplicate with component name!!!";
                return;
            }

            if (gameObjectData.gameObject.name.Contains(" "))
            {
                state = EditorGUIUtility.IconContent("sv_icon_dot6_pix16_gizmo").image;
                tips = "name contains space!!!";
                return;
            }

            state = EditorGUIUtility.IconContent("sv_icon_dot3_pix16_gizmo").image;
            tips = "";
        }


        private bool CheckParent(GameObjectData gameObjectData)
        {
            if (gameObjectData == null)
            {
                return false;
            }

            if (gameObjectData.parent == null)
            {
                return true;
            }

            var pa = gameObjectData.parent;
            while (true)
            {
                if (pa != null)
                {
                    if (pa.firstShowCS.sort < gameObjectData.firstShowCS.sort)
                    {
                        return false;
                    }

                    pa = pa.parent;
                }
                else
                {
                    return true;
                }
            }
        }

        public static int GetId()
        {
            int temp = curId;
            curId++;
            return temp;
        }
    }
}