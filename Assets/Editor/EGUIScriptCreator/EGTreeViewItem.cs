using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EG
{
    public class EGTreeViewItem<T> : TreeViewItem where T : TreeElement
    {
        public T data;

        public EGTreeViewItem(T data) : base(data.id, data.depth, "")
        {
            this.data = data;
        }
    }
}