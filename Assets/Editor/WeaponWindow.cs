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
    int typeSelection = 0;

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
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        myName = EditorGUILayout.TextField("Weapon Name: ", myName);

        EditorGUILayout.Space(); 

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Weapon Type: ", GUILayout.Width(90));
        typeSelection = GUILayout.SelectionGrid(typeSelection, weaponType, 2);
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
                break;
            case 1:
                //heal
                break;
            case 2:
                //debuff
                break;
        }

        //everything else
        myAmmo = EditorGUILayout.IntSlider("Clip Size: ", myAmmo, 10, 20);
        myAccuracy = EditorGUILayout.Slider("Accuracy: ", myAccuracy, .65f, 1f);
        myDropOff = EditorGUILayout.Slider("Accuracy Dropoff: ", myDropOff, .65f, 1f);
        myMaxRange = EditorGUILayout.IntSlider("Max Range: ", myMaxRange, 10, 20);
    }

    private void GetWeapons()
    {
        //clear the lists so they don't double
        weaponList.Clear();
        weaponNameList.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Weapons");
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
        myName = string.Empty;
        myDamage = 30;
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
        weaponList[currentRoyce - 1].weaponName = myName;
        weaponList[currentRoyce - 1].damageAmount = myDamage;
        weaponList[currentRoyce - 1].ammo = myAmmo;
        weaponList[currentRoyce - 1].acc = myAccuracy;
        weaponList[currentRoyce - 1].dropOff = myDropOff;
        weaponList[currentRoyce - 1].MaxRange = myMaxRange;
        weaponList[currentRoyce - 1].healAmount = myHeal;
        weaponList[currentRoyce - 1].debuffTime = myDebuffTime;
        weaponList[currentRoyce - 1].debuffAmount = myDebuffAmount;
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

        Weapons SuperWeapon = ScriptableObject.CreateInstance<Weapons>();

        //fill stats
        //SuperWeapon.usage = myType;
        SuperWeapon.weaponName = myName;
        SuperWeapon.damageAmount = myDamage;
        SuperWeapon.ammo = myAmmo;
        SuperWeapon.acc = myAccuracy;
        SuperWeapon.dropOff = myDropOff;
        SuperWeapon.MaxRange = myMaxRange;
        SuperWeapon.healAmount = myHeal;
        SuperWeapon.debuffTime = myDebuffTime;
        SuperWeapon.debuffAmount = myDebuffAmount;

        AssetDatabase.CreateAsset(SuperWeapon, "Assets/Resources/Data/WeaponData/" + SuperWeapon.weaponName.Replace(" ", "_") + ".asset");
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


