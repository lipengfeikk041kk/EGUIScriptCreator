using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EG;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;

public class EGUIScriptCreator : EditorWindow
{
    private Vector2 scroPos;
    private TreeViewState treeViewState;
    private MultiColumnHeader multiColumnHeader;
    private MultiColumnHeaderState multiColumnHeaderState;
    private EGTreeView egTreeView;
    private GameObject curSelectedGameObject;
    private GameObject lastSelectGameObject;


    private string[] events;

    private string loaded = "unload";
    private string modelName = "Test";

    [MenuItem("EGTools/EGUIScriptCreator")]
    static void Init()
    {
        GetWindow<EGUIScriptCreator>().titleContent =
            new GUIContent("EGUIScriptCreator", Resources.Load<Texture>("Icon/eg"));
    }

    private void OnGUI()
    {
        scroPos = GUILayout.BeginScrollView(scroPos, GUILayout.Width(position.width),
            GUILayout.Height(position.height));
        GUI.Label(new Rect(10, 10, 70, 20), "ConfigPath:");
        EGScriptCreatorConfig.configPath =
            GUI.TextField(new Rect(80, 10, position.width - 225, 20), EGScriptCreatorConfig.configPath);
        if (GUI.Button(new Rect(position.width - 140, 10, 80, 20), "LoadConfig"))
        {
            bool res = EGScriptCreatorConfig.LoadConfig();
            loaded = res && EGScriptCreatorConfig.scriptCreatorTemps != null ? "loaded!" : "unload";
        }

        GUI.Label(new Rect(position.width - 55, 10, 45, 20), loaded);

        if (EGScriptCreatorConfig.scriptCreatorTemps == null)
        {
            bool res = EGScriptCreatorConfig.LoadConfig();
            loaded = res && EGScriptCreatorConfig.scriptCreatorTemps != null ? "loaded!" : "unload";

            if (EGScriptCreatorConfig.scriptCreatorTemps == null || EGScriptCreatorConfig.scriptCreatorTemps.Count == 0)
            {
                GUILayout.EndScrollView();
                return;
            }
        }

        events = new string[EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex].eventSettings
            .Count + 1];
        events[0] = "None";
        for (int i = 0;
            i < EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex].eventSettings
                .Count;
            i++)
        {
            events[i + 1] = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
                .eventSettings[i]
                .eventName;
        }

        GUI.Label(new Rect(10, 40, 80, 20), "SelectTemp:");
        string[] tempNames = new string[EGScriptCreatorConfig.scriptCreatorTemps.Count];
        for (int i = 0; i < EGScriptCreatorConfig.scriptCreatorTemps.Count; i++)
        {
            tempNames[i] = EGScriptCreatorConfig.scriptCreatorTemps[i].tempName;
        }

        EGScriptCreatorConfig.curSelectIndex =
            EditorGUI.Popup(new Rect(90, 40, 150, 20), EGScriptCreatorConfig.curSelectIndex, tempNames);

        GUI.Label(new Rect(257, 40, 80, 20), "ModelName:");
        modelName = GUI.TextField(new Rect(335, 40, position.width - 340, 20), modelName);


        for (int i = 0;
            i < EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex].classTemps.Count;
            i++)
        {
            var cl = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
                .classTemps[i];
            GUI.Label(new Rect(10, 70 + i * 30, 110, 20), "ClassTempName:");
            GUI.Label(new Rect(115, 70 + i * 30, 150, 20), cl.name);
            GUI.Label(new Rect(270, 70 + i * 30, 70, 20), "SavePath:");
            cl.savePath = GUI.TextField(new Rect(335, 70 + i * 30, position.width - 340, 20), cl.savePath);
        }

        if (egTreeView != null)
        {
            egTreeView.events = events;
        }

        if (treeViewState == null || egTreeView == null)
        {
            treeViewState = new TreeViewState();
            multiColumnHeaderState = new MultiColumnHeaderState(new[]
            {
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = false,
                    canSort = false,
                    headerContent = new GUIContent("ChildSelect", EditorGUIUtility.IconContent("Folder Icon").image),
                    headerTextAlignment = TextAlignment.Center,
                    minWidth = 120,
                    width = 120,
                    maxWidth = 120,
                    sortedAscending = false,
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = false,
                    canSort = false,
                    headerContent =
                        new GUIContent("State", EditorGUIUtility.IconContent("NetworkDiscovery Icon").image),
                    headerTextAlignment = TextAlignment.Center,
                    minWidth = 80,
                    width = 80,
                    maxWidth = 80,
                    sortedAscending = false,
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = true,
                    autoResize = true,
                    canSort = true,
                    headerContent = new GUIContent("Name", EditorGUIUtility.IconContent("d_CollabEdit Icon").image),
                    headerTextAlignment = TextAlignment.Center,
                    minWidth = 180,
                    width = 300,
                    sortedAscending = true,
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = true,
                    canSort = false,
                    headerContent = new GUIContent("Component",
                        EditorGUIUtility.IconContent("AnimatorOverrideController Icon").image),
                    headerTextAlignment = TextAlignment.Center,
                    minWidth = 180,
                    width = 180,
                    sortedAscending = false,
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = true,
                    canSort = false,
                    headerContent = new GUIContent("Event",
                        EditorGUIUtility.IconContent("d_EventSystem Icon").image),
                    headerTextAlignment = TextAlignment.Center,
                    minWidth = 180,
                    width = 180,
                    sortedAscending = false,
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = true,
                    canSort = false,
                    headerContent = new GUIContent("Tip",
                        EditorGUIUtility.IconContent("console.infoicon").image),
                    headerTextAlignment = TextAlignment.Center,
                    minWidth = 180,
                    width = 280,
                    sortedAscending = false,
                }
            });
            multiColumnHeader = new MultiColumnHeader(multiColumnHeaderState);
            egTreeView = new EGTreeView(treeViewState, multiColumnHeader);
            egTreeView.events = events;
            egTreeView.Reload();
        }

        if (EGTreeElement.allElements[1].name != "Nothing!!!" && GUI.Button(new Rect(20,
            70 + EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex].classTemps.Count *
            30 + 10, position.width - 40, 20), "GenerateCode"))
        {
            GenerateCode();
        }

        egTreeView.OnGUI(new Rect(20,
            110 + EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex].classTemps.Count *
            30, position.width - 40, position.height - 40 - (110 +
                                                             EGScriptCreatorConfig
                                                                 .scriptCreatorTemps[
                                                                     EGScriptCreatorConfig.curSelectIndex]
                                                                 .classTemps.Count *
                                                             30)));

        float y = position.height - 20;
        if (GUI.Button(new Rect(20,
            position.height - 30, position.width / 2 - 25, 20), "ExpandAll"))
        {
            egTreeView.ExpandAll();
        }

        if (GUI.Button(new Rect(position.width / 2 + 5,
            position.height - 30, position.width / 2 - 25, 20), "CollapseAll"))
        {
            egTreeView.CollapseAll();
        }


        GUILayout.EndScrollView();
    }

    private void SelectedGameObject()
    {
        if (egTreeView?.root == null)
        {
            return;
        }

        if (Selection.activeGameObject != curSelectedGameObject)
        {
            if (Selection.activeGameObject == null && lastSelectGameObject == null)
            {
                EGTreeElement.allElements = null;
                egTreeView.root = new EGTreeViewItem<EGTreeElement>(new EGTreeElement("Root", -1, -1, null));
                var ch = new EGTreeViewItem<EGTreeElement>(new EGTreeElement("Nothing!!!", 0, 0, null));
                egTreeView.root.AddChild(ch);
                egTreeView.Reload();
            }
            else if (Selection.activeGameObject != null && Selection.activeGameObject != lastSelectGameObject)
            {
                EGTreeElement.allElements = null;
                egTreeView.root = new EGTreeViewItem<EGTreeElement>(new EGTreeElement("Root", -1, -1,
                    GameObjectData.CreateData(null, Selection.activeGameObject)));
                GetGameObjectInfo(egTreeView.root);
                if (!egTreeView.root.hasChildren)
                {
                    var ch = new EGTreeViewItem<EGTreeElement>(new EGTreeElement("Nothing!!!", 0, 0, null));
                    egTreeView.root.AddChild(ch);
                }

                lastSelectGameObject = Selection.activeGameObject;
                egTreeView.Reload();
            }

            curSelectedGameObject = Selection.activeGameObject;
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
        SelectedGameObject();
    }

    private void GetGameObjectInfo(EGTreeViewItem<EGTreeElement> element)
    {
        for (int i = 0; i < element.data.gameObjectData.gameObject.transform.childCount; i++)
        {
            Transform tr = element.data.gameObjectData.gameObject.transform.GetChild(i);
            EGTreeElement el = new EGTreeElement(tr.name, element.depth + 1, EGTreeElement.GetId(),
                GameObjectData.CreateData(element.id == -1 ? null : element.data.gameObjectData, tr.gameObject));
            el.gameObjectData.gameObject = tr.gameObject;
            EGTreeViewItem<EGTreeElement> item = new EGTreeViewItem<EGTreeElement>(el);
            element.AddChild(item);
            GetGameObjectInfo((item));
        }
    }

    private void GenerateCode()
    {
        List<string> propertyNames = new List<string>();
        List<string> propertyComps = new List<string>();
        List<string> gameObjectPaths = new List<string>();
        List<int> eventNames = new List<int>();

        PropertySetting propertySetting = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
            .propertySettings;
        var componentSettings = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
            .componentSettings;
        var eventSetting = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
            .eventSettings;
        List<ClassTemp> classTemps = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
            .classTemps;

        List<string> propertyDefines = new List<string>();
        List<string> propertyFinds = new List<string>();
        List<string> eventAdds = new List<string>();
        List<string> eventFuncs = new List<string>();
        GameObject root = EGTreeElement.allElements[0].gameObjectData.gameObject;

        for (int i = 1; i < EGTreeElement.allElements.Count; i++)
        {
            if (EGTreeElement.allElements[i].selected)
            {
                string propertyName = EGTreeElement.allElements[i].content;
                string propertyComp = EGTreeElement.allElements[i].gameObjectData
                    .components[EGTreeElement.allElements[i].compType];
                string propertyFullComp = EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
                    .componentSettings.Find(c => c.componentName == propertyComp).fullComponentName;
                string gameObjectPath = EGTreeElement.allElements[i].gameObjectData.path;
                int eventName = EGTreeElement.allElements[i].eventType;


                bool ok = true;
                if (propertyNames.Contains(propertyName))
                {
                    Debug.LogError($"name:{propertyName} has allready exists!!!");
                    ok = false;
                }

                if (componentSettings.Find(cs => cs.componentName == propertyName) != null)
                {
                    Debug.LogError($"name:{propertyName} duplicate with component name!!!");
                    ok = false;
                }

                if (!ok)
                {
                    return;
                }

                propertyNames.Add(propertyName);
                propertyComps.Add(propertyComp);
                gameObjectPaths.Add(gameObjectPath);
                eventNames.Add(eventName);

                string propertyDefine = propertySetting.propertyDefineTemp.Replace("[propertyComp]", propertyComp)
                    .Replace("[propertyName]", propertyName)
                    .Replace("[PropertyName]", EGScriptCreatorUtil.UpFirstChar(propertyName))
                    .Replace("[propertyFullComp]", propertyFullComp);
                string propertyFind = "";

                if (propertyComp == "GameObject")
                {
                    propertyFind = propertySetting.propertyGameObjectFindTemp.Replace("[propertyName]", propertyName)
                        .Replace("[gameObjectPath]", gameObjectPath)
                        .Replace("[PropertyName]", EGScriptCreatorUtil.UpFirstChar(propertyName))
                        .Replace("[propertyFullComp]", propertyFullComp);
                }
                else if (propertyComp == "Transform")
                {
                    propertyFind = propertySetting.propertyTransformFindTemp.Replace("[propertyName]", propertyName)
                        .Replace("[gameObjectPath]", gameObjectPath)
                        .Replace("[PropertyName]", EGScriptCreatorUtil.UpFirstChar(propertyName))
                        .Replace("[propertyFullComp]", propertyFullComp);
                }
                else
                {
                    propertyFind = propertySetting.propertyFindTemp.Replace("[propertyName]", propertyName)
                        .Replace("[gameObjectPath]", gameObjectPath).Replace("[propertyComp]", propertyComp)
                        .Replace("[PropertyName]", EGScriptCreatorUtil.UpFirstChar(propertyName))
                        .Replace("[propertyFullComp]", propertyFullComp);
                }

                string eventAdd = "";
                string eventFunc = "";
                if (eventName != 0)
                {
                    eventAdd = eventSetting[eventName - 1].eventAddTemp.Replace("[propertyName]", propertyName)
                        .Replace("[eventName]", events[eventName]).Replace("[EventName]",
                            EGScriptCreatorUtil.UpFirstChar(events[eventName]))
                        .Replace("[PropertyName]", EGScriptCreatorUtil.UpFirstChar(propertyName));
                    eventFunc = eventSetting[eventName - 1].eventFunctionTemp.Replace("[propertyName]", propertyName)
                        .Replace("[eventName]", events[eventName]).Replace("[EventName]",
                            EGScriptCreatorUtil.UpFirstChar(events[eventName]))
                        .Replace("[PropertyName]", EGScriptCreatorUtil.UpFirstChar(propertyName));
                }


                propertyDefines.Add(propertyDefine);
                propertyFinds.Add(propertyFind);
                eventAdds.Add(eventAdd);
                eventFuncs.Add(eventFunc);
            }
        }

        bool isNeedReferesh = false;
        for (int i = 0; i < classTemps.Count; i++)
        {
            string res = "";
            string[] split = classTemps[i].content.Replace("\r\n", "\n").Split('\n');

            foreach (var s in split)
            {
                int classNameIndex = s.IndexOf("[gameObjectName]", StringComparison.Ordinal);
                int propertyDefineIndex = s.IndexOf("[propertyDefine]", StringComparison.Ordinal);
                int propertyFindIndex = s.IndexOf("[propertyFind]", StringComparison.Ordinal);
                int eventAddIndex = s.IndexOf("[eventAdd]", StringComparison.Ordinal);
                int eventFunctionIndex = s.IndexOf("[eventFunction]", StringComparison.Ordinal);
                int modelNameIndex = s.IndexOf("[modelName]", StringComparison.Ordinal);

                string temp = s;

                if (classNameIndex != -1)
                {
                    temp = temp.Replace("[gameObjectName]", root.name);
                }

                if (modelNameIndex != -1)
                {
                    temp = temp.Replace("[modelName]", modelName);
                }

                if (propertyDefineIndex != -1)
                {
                    string propertyDefine = "";
                    for (int j = 0; j < propertyDefines.Count; j++)
                    {
                        string _propertyDefine =
                            propertyDefines[j].Replace("\n", "\n" + AddSpace("", propertyDefineIndex));

                        propertyDefine += $"\n{AddSpace(_propertyDefine, propertyDefineIndex)}";
                    }

                    temp = propertyDefine;
                }
                else if (propertyFindIndex != -1)
                {
                    string propertyFind = "";
                    for (int j = 0; j < propertyFinds.Count; j++)
                    {
                        string _propertyFind = propertyFinds[j].Replace("\n", "\n" + AddSpace("", propertyFindIndex));

                        propertyFind += $"\n{AddSpace(_propertyFind, propertyFindIndex)}";
                    }

                    temp = propertyFind;
                }
                else if (eventAddIndex != -1)
                {
                    string eventAdd = "";
                    for (int j = 0; j < eventAdds.Count; j++)
                    {
                        if (string.IsNullOrEmpty(eventAdds[j]))
                        {
                            continue;
                        }

                        string _eventAdd = eventAdds[j].Replace("\n", "\n" + AddSpace("", eventAddIndex));

                        eventAdd += $"\n{AddSpace(_eventAdd, eventAddIndex)}";
                    }

                    temp = eventAdd;
                }
                else if (eventFunctionIndex != -1)
                {
                    string eventFunction = "";
                    for (int j = 0; j < eventFuncs.Count; j++)
                    {
                        if (string.IsNullOrEmpty(eventFuncs[j]))
                        {
                            continue;
                        }

                        string _eventFunc = eventFuncs[j].Replace("\n", "\n" + AddSpace("", eventFunctionIndex));

                        eventFunction += $"\n\n{AddSpace(_eventFunc, eventFunctionIndex)}";
                    }

                    temp = eventFunction;
                }
                else
                {
                    temp = $"\n{temp}";
                }

                res += temp;
            }

            string savePath = classTemps[i].savePath;
            savePath = savePath.Replace("[modelName]", modelName).Replace("[gameObjectName]", root.name);
            savePath = Application.dataPath + "/" + savePath;
            string dirPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (File.Exists(savePath))
            {
                // Debug.LogError($"can not create file,file [{savePath}] already exists!!!");
                // if (isNeedReferesh)
                // {
                //     AssetDatabase.Refresh();
                // }
                //
                // return;
                File.Delete(savePath);
            }

            StreamWriter streamWriter = new StreamWriter(savePath);
            streamWriter.Write(res);
            streamWriter.Flush();
            streamWriter.Close();
            isNeedReferesh = true;
        }

        if (isNeedReferesh)
        {
            AssetDatabase.Refresh();
        }
    }

    private string AddSpace(string s, int count)
    {
        string res = "";
        for (int i = 0; i < count; i++)
        {
            res += " ";
        }

        return res + s;
    }
}

[Serializable]
public class EventSetting
{
    public string eventName;
    public string eventAddTemp;
    public string eventFunctionTemp;

    public static List<EventSetting> FromJson(string json)
    {
        return SimpleJson.SimpleJson.DeserializeObject<List<EventSetting>>(json);
    }
}

[Serializable]
public class PropertySetting
{
    public string propertyGameObjectFindTemp;
    public string propertyTransformFindTemp;
    public string propertyDefineTemp;
    public string propertyFindTemp;

    public static PropertySetting FromJson(string json)
    {
        return SimpleJson.SimpleJson.DeserializeObject<PropertySetting>(json);
    }
}

[Serializable]
public class ComponentSetting
{
    public string componentName;
    public string fullComponentName;
    public int sort;

    public static List<ComponentSetting> FromJson(string json)
    {
        return SimpleJson.SimpleJson.DeserializeObject<List<ComponentSetting>>(json);
    }
}

public class ClassTemp
{
    public string name;
    public string content;
    public string savePath;
}

public class ScriptCreatorTemp
{
    public string tempPath;
    public string tempName;
    public List<ClassTemp> classTemps;
    public List<ComponentSetting> componentSettings;
    public List<EventSetting> eventSettings;
    public PropertySetting propertySettings;

    public void LoadTemp()
    {
        DirectoryInfo classTempDir = new DirectoryInfo(Path.Combine(tempPath, "ClassTemp/"));
        classTemps = new List<ClassTemp>();
        foreach (var fileInfo in classTempDir.GetFiles())
        {
            if (fileInfo.Extension.ToLower() == ".meta")
            {
                continue;
            }

            List<string> classRes = EGScriptCreatorUtil.ReadClassFile(fileInfo.FullName);
            ClassTemp classTemp = new ClassTemp()
            {
                content = classRes[1],
                name = fileInfo.Name,
                savePath = classRes[0].Split(':')[1],
            };
            classTemps.Add(classTemp);
        }

        componentSettings =
            ComponentSetting.FromJson(EGScriptCreatorUtil.ReadFile(Path.Combine(tempPath, "ComponentSetting.json")));
        eventSettings =
            EventSetting.FromJson(EGScriptCreatorUtil.ReadFile(Path.Combine(tempPath, "EventSetting.json")));
        propertySettings =
            PropertySetting.FromJson(EGScriptCreatorUtil.ReadFile(Path.Combine(tempPath, "PropertySetting.json")));
    }
}

public class EGScriptCreatorConfig
{
    public static int curSelectIndex;
    public static string configPath = @"Editor\EGUIScriptCreator\EGUIScriptCreatorTemp";
    public static List<ScriptCreatorTemp> scriptCreatorTemps;

    public static bool LoadConfig()
    {
        try
        {
            List<DirectoryInfo> directoryInfos =
                new DirectoryInfo(Application.dataPath + "/" + configPath).GetDirectories().ToList();
            scriptCreatorTemps = new List<ScriptCreatorTemp>();
            foreach (var directoryInfo in directoryInfos)
            {
                ScriptCreatorTemp scriptCreatorTemp = new ScriptCreatorTemp()
                    {tempName = directoryInfo.Name, tempPath = directoryInfo.FullName};
                scriptCreatorTemp.LoadTemp();
                scriptCreatorTemps.Add(scriptCreatorTemp);
            }

            PlayerPrefs.SetString("EGScriptConfigPath", configPath);
            PlayerPrefs.Save();

            return true;
        }
        catch (Exception e)
        {
            PlayerPrefs.DeleteKey("EGScriptConfigPath");
            Debug.LogError("invalid path!!!");
            Debug.LogError(e);
        }

        return false;
    }
}

public class EGScriptCreatorUtil
{
    public static string UpFirstChar(string s)
    {
        return s.Substring(0, 1).ToUpper() + s.Substring(1);
    }

    public static string ReadFile(string path)
    {
        StreamReader streamReader = new StreamReader(path);
        string content = streamReader.ReadToEnd();
        streamReader.Close();
        return content;
    }

    public static List<string> ReadClassFile(string path)
    {
        List<string> res = new List<string>();
        StreamReader streamReader = new StreamReader(path);
        res.Add(streamReader.ReadLine());
        string s = "";
        while (true)
        {
            string temp = streamReader.ReadLine();
            if (temp == null)
            {
                streamReader.Close();
                break;
            }

            s += temp + "\n";
        }

        res.Add(s);
        return res;
    }
}

public class GameObjectData
{
    public string gameObjectName;
    public List<string> components;
    public int firstShowComp;
    public ComponentSetting firstShowCS;
    public string path;
    public GameObject gameObject;
    public GameObjectData parent;

    public static GameObjectData CreateData(GameObjectData parent, GameObject gameObject)
    {
        GameObjectData data = new GameObjectData()
        {
            gameObjectName = gameObject.name, components = new List<string>(), gameObject = gameObject, parent = parent
        };
        if (parent == null)
        {
            data.path = gameObject.name;
        }
        else
        {
            data.path = $"{parent.path}/{gameObject.name}";
        }

        data.components = GetComponentSettings(gameObject, out data.firstShowComp, out data.firstShowCS);

        return data;
    }

    private static List<string> GetComponentSettings(GameObject gameObject,
        out int firstShowComp, out ComponentSetting firstShowCS)
    {
        List<string> componentSettings = new List<string>();
        firstShowCS = null;
        firstShowComp = -1;
        foreach (var componentSetting in EGScriptCreatorConfig.scriptCreatorTemps[EGScriptCreatorConfig.curSelectIndex]
            .componentSettings)
        {
            bool has = false;
            if (componentSetting.componentName == "Transform" || componentSetting.componentName == "RectTransform" ||
                componentSetting.componentName == "GameObject")
            {
                has = true;
            }
            else
            {
                var comp = gameObject.GetComponent(componentSetting.componentName);
                if (comp != null)
                {
                    has = true;
                }
            }

            if (has)
            {
                componentSettings.Add(componentSetting.componentName);
                if (firstShowCS == null || componentSetting.sort < firstShowCS.sort)
                {
                    firstShowComp = componentSettings.Count - 1;
                    firstShowCS = componentSetting;
                }
            }
        }

        return componentSettings;
    }
}