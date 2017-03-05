using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

public class UnitWindow : EditorWindow {

    //initialization to pull assets from assetdatabase
    public List<Units> unitList = new List<Units>();
    public List<string> unitNameList = new List<string>();
    public string[] unitNameArray;

    string myName;
    Sprite myPortrait;
    int myHealth;
    int myAP;
    float myArmor;
    bool myFuji;
    float mydamageMod;

    bool nameFlag;
    bool existingFlag;
    bool noSpriteFlag;

    //choices for if it's a new enemy or a stored one
    public int currentRoyce = 0;
    int lastRoyce = 0;

    [MenuItem("Junk Paper Militia/Unit Creator %u")]
    static void WindowOpener()
    {
        EditorWindow.GetWindow<UnitWindow>("Unit Creator");
    }

    void Awake()
    {
        SwitchFlags();
        GetUnits();
    }

    private void OnGUI()
    {
        currentRoyce = EditorGUILayout.Popup(currentRoyce, unitNameArray);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //showing sprite only if it exists
        if (myPortrait != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture2D myTexture = SpriteUtility.GetSpriteTexture(myPortrait, false);
            GUILayout.Label(myTexture);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        myPortrait = (Sprite)EditorGUILayout.ObjectField(myPortrait, typeof(Sprite), false) as Sprite;
        myName = EditorGUILayout.DelayedTextField("Unit Name: ", myName);
        myHealth = EditorGUILayout.IntSlider("Health: ", myHealth, 30, 70);
        myAP = EditorGUILayout.IntSlider("Action Points: ", myAP, 2, 4);
        myArmor = EditorGUILayout.Slider("Armor: ", myArmor, 0.1f, 0.25f);
        myFuji = EditorGUILayout.Toggle("Fujiwara Unit: ", myFuji);
        GUI.enabled = myFuji;
        mydamageMod = EditorGUILayout.Slider("Damage Modifier: ", mydamageMod, 1.1f, 1.3f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        if (currentRoyce == 0)
        {
            if (GUILayout.Button("Create")){
                SwitchFlags();
                CreateUnit();}
        }
        else
        {
            if (GUILayout.Button("Save")){
                SwitchFlags();
                SaveUnit();}
        }

        if (currentRoyce != lastRoyce)
        {
            lastRoyce = currentRoyce;
            switch (currentRoyce)
            {
                case 0:
                    NewUnit();
                    break;
                default:
                    CurrentUnit();
                    break;
            }
        }

        //flag errors
        if (nameFlag)
        {
            EditorGUILayout.HelpBox("Name Can not be blank", MessageType.Warning);
        }
        if (existingFlag)
        {
            EditorGUILayout.HelpBox("This Unit Already exists", MessageType.Error);
        }
        if (noSpriteFlag)
        {
            EditorGUILayout.HelpBox("Can't save without a portrait", MessageType.Error);
        }
    }

    void GetUnits()
    {
        //clear the lists so they don't double
        unitList.Clear();
        unitNameList.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Units");
        foreach (string guid in guids)
        {
            string RoyceString = AssetDatabase.GUIDToAssetPath(guid);
            Units enemyInst = AssetDatabase.LoadAssetAtPath(RoyceString, typeof(Units)) as Units;
            unitNameList.Add(enemyInst.unitName);
            unitList.Add(enemyInst);
        }
        unitNameList.Insert(0, "New");
        unitNameArray = unitNameList.ToArray();
    }

    void NewUnit()
    {
        myName = string.Empty;
        myPortrait = null;
        myHealth = 30;
        myAP = 2;
        myArmor = 0.1f;
        myFuji = false;
        mydamageMod = 1.1f;
    }

    void CurrentUnit()
    {
        myName = unitList[currentRoyce - 1].unitName;
        myPortrait = unitList[currentRoyce - 1].portrait;
        myHealth = unitList[currentRoyce - 1].health;
        myAP = unitList[currentRoyce - 1].AP;
        myArmor = unitList[currentRoyce - 1].armor;
        myFuji = unitList[currentRoyce - 1].isFujiwara;
        mydamageMod = unitList[currentRoyce - 1].damageModifier;
    }

    void SaveUnit()
    {
        unitList[currentRoyce - 1].unitName = myName;
        unitList[currentRoyce - 1].portrait = myPortrait;
        unitList[currentRoyce - 1].health = myHealth;
        unitList[currentRoyce - 1].AP = myAP;
        unitList[currentRoyce - 1].armor = myArmor;
        unitList[currentRoyce - 1].isFujiwara = myFuji;
        unitList[currentRoyce - 1].damageModifier = (myFuji) ? mydamageMod : 1.0f;

        EditorUtility.SetDirty(unitList[currentRoyce - 1]);
        AssetDatabase.SaveAssets();
    }

    bool CreateUnit()
    {
        SwitchFlags();
        if (myName == string.Empty)
        {
            nameFlag = true;
            return false;
        }
        if (myPortrait == null)
        {
            noSpriteFlag = true;
            return false;
        }

        //check if guy already exists
        string[] assetString = AssetDatabase.FindAssets(myName.Replace(" ", "_"));
        if (assetString.Length > 0)
        {
            existingFlag = true;
            return false;
        }

        Units myUnit = ScriptableObject.CreateInstance<Units>();

        //fill stats
        myUnit.unitName = myName;
        myUnit.portrait = myPortrait;
        myUnit.health = myHealth;
        myUnit.AP = myAP;
        myUnit.armor = myArmor;
        myUnit.isFujiwara = myFuji;
        myUnit.damageModifier = (myUnit.isFujiwara) ? mydamageMod : 1.0f;

        AssetDatabase.CreateAsset(myUnit, "Assets/Resources/Data/UnitData/" + myUnit.unitName.Replace(" ", "_") + ".asset");
        SwitchFlags();
        GetUnits();

        //set correct index
        for (int i = 0; i < unitList.Count; i++)
        {
            if (unitList[i].unitName == myName)
            {
                currentRoyce = i + 1;
            }
        }
        return true;
    }

    void SwitchFlags()
    {
        nameFlag = false;
        existingFlag = false;
        noSpriteFlag = false;
    }
}
