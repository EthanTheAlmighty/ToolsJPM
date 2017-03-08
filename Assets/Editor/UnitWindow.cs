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
    PrimaryWeapon myPrimary;
    SecondaryWeapon mySecondary;

    bool nameFlag;
    bool existingFlag;
    bool noSpriteFlag;
    bool primaryFlag;
    bool secondFlag;

    //choices for if it's a new enemy or a stored one
    public int currentRoyce = 0;
    int lastRoyce = 0;

    //current choice of 

    [MenuItem("Junk Paper Militia/Unit Creator %u")]
    static void WindowOpener()
    {
        EditorWindow.GetWindow<UnitWindow>("Unit Creator");
        EditorWindow.GetWindow<UnitWindow>().minSize = new Vector2(375, 350);
        EditorWindow.GetWindow<UnitWindow>().maxSize = new Vector2(400, 425);
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
        //primary
        //primaryChoice = EditorGUILayout.Popup(primaryChoice, primaryNameArray);
        myPrimary = EditorGUILayout.ObjectField("Primary Weapon: ", myPrimary, typeof(PrimaryWeapon), false) as PrimaryWeapon;

        EditorGUILayout.Space();
        //secondary
        mySecondary = EditorGUILayout.ObjectField("Secondary Weapon: ", mySecondary, typeof(SecondaryWeapon), false) as SecondaryWeapon;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //GUIStyle centerer = GUI.skin.button;
        //centerer.alignment = TextAnchor.MiddleCenter;
        //centerer.fixedWidth = 100;

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (currentRoyce == 0)
        {
            GUI.color = new Color(.235f, .588f, .392f);
            if (GUILayout.Button("Create", GUILayout.Width(100))){
                SwitchFlags();
                CreateUnit();}
        }
        else
        {
            GUI.color = new Color(.12f, .588f, .86f);
            if (GUILayout.Button("Save", GUILayout.Width(100))){
                SwitchFlags();
                SaveUnit();}
        }
        GUI.color = Color.white;

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

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
        DrawFlags();
    }

    void DrawFlags()
    {
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
            EditorGUILayout.HelpBox("Can't save without a portrait", MessageType.Warning);
        }
        if(primaryFlag)
        {
            EditorGUILayout.HelpBox("Can't save without a Primary Weapon", MessageType.Warning);
        }
        if (secondFlag)
        {
            EditorGUILayout.HelpBox("Can't save without a Secondary Weapon", MessageType.Warning);
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
        myPrimary = unitList[currentRoyce - 1].primary;
        mySecondary = unitList[currentRoyce - 1].second;
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
        unitList[currentRoyce - 1].primary = myPrimary;
        unitList[currentRoyce - 1].second = mySecondary;

        EditorUtility.SetDirty(unitList[currentRoyce - 1]);
        AssetDatabase.SaveAssets();
    }

    bool CreateUnit()
    {
        SwitchFlags();
        //check if it's savable: needs a portrait, a name, if it exists already,
        //and if it has a primary and secondary weapon attached
        if (!CheckSavable())
            return false;

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

    bool CheckSavable()
    {
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

        if(myPrimary == null)
        {
            primaryFlag = true;
            return false;
        }
        if(mySecondary = null)
        {
            secondFlag = true;
            return false;
        }

        return true;
    }

    //resets all of the flags on attempt to resave or recreate unit
    void SwitchFlags()
    {
        nameFlag = false;
        existingFlag = false;
        noSpriteFlag = false;
        primaryFlag = false;
        secondFlag = false;
    }
}
