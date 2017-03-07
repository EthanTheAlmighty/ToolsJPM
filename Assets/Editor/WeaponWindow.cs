using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class WeaponWindow : EditorWindow {

    //initialization to pull assets from assetdatabase
    public List<Weapons> weaponList = new List<Weapons>();
    public List<string> weaponNameList = new List<string>();
    public string[] weaponNameArray;

    //storing selections from the selection grids
    string[] effectTypes = { "Damage", "Heal", "Debuff" };
    string[] weaponType = { "Primary", "Secondary" };
    int effectSelection = 0;
    int weaponEffect = 0;

    //toggle for which type of gun it is
    bool myType1, myType2, myType3;
    string myName;
    int myDamage;
    int myAmmo;
    float myAccuracy;
    float myDropOff;
    int myMaxRange;
    int myHeal;
    int myDebuffTime;
    float myDebuffAmount;

    bool nameFlag;
    bool existingFlag;

    //choices for if it's a new enemy or a stored one
    public int currentRoyce = 0;
    int lastRoyce = 0;

    [MenuItem("Junk Paper Militia/Weapon Creator %w")]
    static void WindowOpener()
    {
        EditorWindow.GetWindow<WeaponWindow>("Weapon Creator");
    }

    void Awake()
    {
        SwitchFlags();
        GetWeapons();
    }

    void OnGUI()
    {
        currentRoyce = EditorGUILayout.Popup(currentRoyce, weaponNameArray);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        myName = EditorGUILayout.TextField("Weapon Name: ", myName);

        EditorGUILayout.Space(); 

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Weapon Type: ", GUILayout.Width(90));
        weaponEffect = GUILayout.SelectionGrid(weaponEffect, weaponType, 2);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Effect Type: ", GUILayout.Width(90));
        effectSelection = GUILayout.SelectionGrid(effectSelection, effectTypes, 3);
        EditorGUILayout.EndHorizontal();

        switch (effectSelection)
        {
            case 0:
                //damage
                myDamage = EditorGUILayout.IntSlider("Amount: ", myDamage, 5, 20);
                GUI.enabled = false;
                myDebuffTime = EditorGUILayout.IntSlider("Time: ", myDebuffTime, 2, 3);
                GUI.enabled = true;
                break;
            case 1:
                //heal
                myHeal = EditorGUILayout.IntSlider("Amount: ", myHeal, 6, 17);
                GUI.enabled = false;
                myDebuffTime = EditorGUILayout.IntSlider("Time: ", myDebuffTime, 2, 3);
                GUI.enabled = true;
                break;
            case 2:
                //debuff
                myDebuffAmount = EditorGUILayout.Slider("Amount: ", myDebuffAmount, 0.15f, 0.4f);
                myDebuffTime = EditorGUILayout.IntSlider("Time: ", myDebuffTime, 2, 3);
                break;
        }

        //everything else
        myAmmo = EditorGUILayout.IntSlider("Clip Size: ", myAmmo, 1, 15);
        myAccuracy = EditorGUILayout.Slider("Accuracy: ", myAccuracy, .6f, 1f);
        myDropOff = EditorGUILayout.Slider("Accuracy Dropoff: ", myDropOff, .02f, 1f);
        myMaxRange = EditorGUILayout.IntSlider("Max Range: ", myMaxRange, 1, 20);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (currentRoyce == 0)
        {
            if (GUILayout.Button("Create"))
            {
                SwitchFlags();
                CreateWeapon();
            }
        }
        else
        {
            if (GUILayout.Button("Save"))
            {
                SwitchFlags();
                SaveWeapon();
            }
        }

        if (currentRoyce != lastRoyce)
        {
            lastRoyce = currentRoyce;
            switch (currentRoyce)
            {
                case 0:
                    NewWeapon();
                    break;
                default:
                    CurrentWeapon();
                    break;
            }
        }
    }

    private void GetWeapons()
    {
        //clear the lists so they don't double
        weaponList.Clear();
        weaponNameList.Clear();

        string[] guids = AssetDatabase.FindAssets("t:PrimaryWeapon t:SecondaryWeapon");
        foreach (string guid in guids)
        {
            string RoyceString = AssetDatabase.GUIDToAssetPath(guid);
            Weapons weaponInst = AssetDatabase.LoadAssetAtPath(RoyceString, typeof(Weapons)) as Weapons;
            weaponNameList.Add(weaponInst.weaponName);
            weaponList.Add(weaponInst);
        }
        weaponNameList.Insert(0, "new");
        weaponNameArray = weaponNameList.ToArray();
    }

    void NewWeapon()
    {
        //myType = WeaponType.DAMAGE;
        weaponEffect = 0;
        myName = string.Empty;
        myDamage = 5;
        myAmmo = 1;
        myAccuracy = .6f;
        myDropOff = .02f;
        myMaxRange = 1;
        myHeal = 6;
        myDebuffTime = 2;
        myDebuffAmount = .15f;
    }

    void CurrentWeapon()
    {
        //myType = weaponList[currentRoyce - 1].usage;
        weaponEffect = weaponList[currentRoyce - 1].weaponEffect;
        myName = weaponList[currentRoyce - 1].weaponName;
        myDamage = weaponList[currentRoyce - 1].damageAmount;
        myAmmo = weaponList[currentRoyce - 1].ammo;
        myAccuracy = weaponList[currentRoyce - 1].acc;
        myDropOff = weaponList[currentRoyce - 1].dropOff;
        myMaxRange = weaponList[currentRoyce - 1].MaxRange;
        myHeal = weaponList[currentRoyce - 1].healAmount;
        myDebuffTime = weaponList[currentRoyce - 1].debuffTime;
        myDebuffAmount = weaponList[currentRoyce - 1].debuffAmount;
    }

    void SaveWeapon()
    {
        //weaponList[currentRoyce - 1].usage = myType;
        weaponList[currentRoyce - 1].weaponEffect = weaponEffect;
        weaponList[currentRoyce - 1].weaponName = myName;
        weaponList[currentRoyce - 1].damageAmount = myDamage;
        weaponList[currentRoyce - 1].ammo = myAmmo;
        weaponList[currentRoyce - 1].acc = myAccuracy;
        weaponList[currentRoyce - 1].dropOff = myDropOff;
        weaponList[currentRoyce - 1].MaxRange = myMaxRange;
        weaponList[currentRoyce - 1].healAmount = myHeal;
        weaponList[currentRoyce - 1].debuffTime = myDebuffTime;
        weaponList[currentRoyce - 1].debuffAmount = myDebuffAmount;

        EditorUtility.SetDirty(weaponList[currentRoyce - 1]);
        AssetDatabase.SaveAssets();
    }

    bool CreateWeapon()
    {
        SwitchFlags();
        if (myName == string.Empty)
        {
            nameFlag = true;
            return false;
        }

        //check if guy already exists
        string[] assetString = AssetDatabase.FindAssets(myName.Replace(" ", "_"));
        if (assetString.Length > 0)
        {
            existingFlag = true;
            return false;
        }

        if(weaponEffect == 0)
        {
            PrimaryWeapon SuperWeapon = ScriptableObject.CreateInstance<PrimaryWeapon>();

            //fill stats
            //SuperWeapon.usage = myType;
            SuperWeapon.weaponEffect = weaponEffect;
            SuperWeapon.weaponName = myName;
            SuperWeapon.damageAmount = myDamage;
            SuperWeapon.ammo = myAmmo;
            SuperWeapon.acc = myAccuracy;
            SuperWeapon.dropOff = myDropOff;
            SuperWeapon.MaxRange = myMaxRange;
            SuperWeapon.healAmount = myHeal;
            SuperWeapon.debuffTime = myDebuffTime;
            SuperWeapon.debuffAmount = myDebuffAmount;

            AssetDatabase.CreateAsset(SuperWeapon, "Assets/Resources/Data/WeaponData/Primary/" + SuperWeapon.weaponName.Replace(" ", "_") + ".asset");
        }
        else if(weaponEffect ==1)
        {
            SecondaryWeapon SuperWeapon = ScriptableObject.CreateInstance<SecondaryWeapon>();

            //fill stats
            //SuperWeapon.usage = myType;
            SuperWeapon.weaponEffect = weaponEffect;
            SuperWeapon.weaponName = myName;
            SuperWeapon.damageAmount = myDamage;
            SuperWeapon.ammo = myAmmo;
            SuperWeapon.acc = myAccuracy;
            SuperWeapon.dropOff = myDropOff;
            SuperWeapon.MaxRange = myMaxRange;
            SuperWeapon.healAmount = myHeal;
            SuperWeapon.debuffTime = myDebuffTime;
            SuperWeapon.debuffAmount = myDebuffAmount;

            AssetDatabase.CreateAsset(SuperWeapon, "Assets/Resources/Data/WeaponData/Secondary/" + SuperWeapon.weaponName.Replace(" ", "_") + ".asset");
        }
        

        
        SwitchFlags();
        GetWeapons();

        //set correct index
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (weaponList[i].weaponName == myName)
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
    }
}


