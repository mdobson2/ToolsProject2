using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

public class EnemyTool : EditorWindow {

    public List<Enemies> enemyList = new List<Enemies>();
    public List<string> enemyNameList = new List<string>();
    public string[] enemyNameArray;

    //enemy sttributes
    string enemyName = "";
    int currentChoice = 0;
    int lastChoice = 0;
    int enemyHealth;
    Sprite enemySprite = null;
    float enemyAttackTime = 0f;
    int enemyAttack = 0;
    int enemyDefence = 0;
    int enemyAgility = 0;
    bool isMagic = false;
    int enemyMana = 0;

    bool nameFlag = false;
    bool existsFlag = false;
    bool spriteFlag = false;

    //used to open the window
    [MenuItem("Custom Tools/Enemy Tool %g")]
    private static void initialization()
    {
        EditorWindow.GetWindow<EnemyTool>();
    }

    void Awake()
    {
        getEnemies();
    }

    void OnGUI()
    {
        //Enum popup for the enemy selection
        currentChoice = EditorGUILayout.Popup(currentChoice, enemyNameArray);

        //Start enemy manipulation zone
        EditorGUILayout.Space();
        if(enemySprite != null)
        {
            //Centered Sprite
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture2D tempTexture = SpriteUtility.GetSpriteTexture(enemySprite, false);
            GUILayout.Label(tempTexture);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        enemySprite = EditorGUILayout.ObjectField(enemySprite, typeof(Sprite), false) as Sprite;
        enemyName = EditorGUILayout.TextField("Name: ", enemyName);
        enemyHealth = EditorGUILayout.IntSlider("Health: ", enemyHealth, 1, 300);
        enemyAttack = EditorGUILayout.IntSlider("Attack: ", enemyAttack, 1, 100);
        enemyAttackTime = EditorGUILayout.Slider("Attack Timer: ", enemyAttackTime, 1, 20);
        enemyDefence = EditorGUILayout.IntSlider("Defence: ", enemyDefence, 1, 100);
        enemyAgility = EditorGUILayout.IntSlider("Agility: ", enemyAgility, 1, 100);
        isMagic = EditorGUILayout.BeginToggleGroup("Magic User", isMagic);
        enemyMana = EditorGUILayout.IntSlider("Mana: ", enemyMana, 0, 100);
        EditorGUILayout.EndToggleGroup();

        //End enemy manipulation zone
        EditorGUILayout.Space();

        if (currentChoice == 0)
        {
            if (GUILayout.Button("Create"))
            {
                createNew();
            }
        }
        else
        {
            if(GUILayout.Button("Save"))
            {
                saveCurrentEnemy();
            }
        }

        if(currentChoice != lastChoice)
        {
            if(currentChoice == 0)
            {
                newEnemy();
            }
            else
            {
                populateUI();
            }
            lastChoice = currentChoice;
        }
        if(nameFlag)
        {
            EditorGUILayout.HelpBox("Name can not be blank", MessageType.Error);
        }
        if(existsFlag)
        {
            EditorGUILayout.HelpBox("That enemy already exists", MessageType.Error);
        }
        if(spriteFlag)
        {
            EditorGUILayout.HelpBox("The enemy must have a sprite", MessageType.Error);
        }
    }

    private void getEnemies()
    {
        enemyList.Clear();
        enemyNameList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Enemies");
        foreach(string guid in guids)
        {
            //Debug.Log(guid);
            string tempString = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log(myString);
            Enemies enemyInst = AssetDatabase.LoadAssetAtPath(tempString, typeof(Enemies)) as Enemies;

            enemyNameList.Add(enemyInst.emname);

            enemyList.Add(enemyInst);            
        }
        enemyNameList.Insert(0, "New");
        enemyNameArray = enemyNameList.ToArray();
    }

    private void populateUI()
    {
        flagReset();

        enemyName = enemyList[currentChoice - 1].emname;
        enemySprite = enemyList[currentChoice - 1].mySprite;
        enemyHealth = enemyList[currentChoice - 1].health;
        enemyAttack = enemyList[currentChoice - 1].atk;
        enemyAttackTime = enemyList[currentChoice - 1].atkTime;
        enemyAttack = enemyList[currentChoice - 1].def;
        isMagic = enemyList[currentChoice - 1].isMagic;
        enemyMana = enemyList[currentChoice - 1].manaPool;
    }

    private void newEnemy()
    {
        flagReset();

        enemyName = "";
        enemySprite = null;
        enemyHealth = 1;
        enemyAttack = 1;
        enemyAttackTime = 1;
        enemyDefence = 1;
        isMagic = false;
        enemyMana = 0;
    }

    private void createNew()
    {
        if(enemyName == "")
        {
            nameFlag = true;
            return;
        }
        if(enemySprite == null)
        {
            spriteFlag = true;
            return;
        }
        string[] assetString = AssetDatabase.FindAssets(enemyName.Replace(" ", "_"));
        if (assetString.Length > 0)
        {
            existsFlag = true;
            return;
        }
        Enemies meinEnemy = ScriptableObject.CreateInstance<Enemies>();
        meinEnemy.emname = enemyName;
        meinEnemy.mySprite = enemySprite;
        meinEnemy.health = enemyHealth;
        meinEnemy.atk = enemyAttack;
        meinEnemy.atkTime = enemyAttackTime;
        meinEnemy.def = enemyDefence;
        meinEnemy.isMagic = isMagic;
        meinEnemy.manaPool = enemyMana;
        AssetDatabase.CreateAsset(meinEnemy, "Assets/Resources/Data/EnemyData/" + meinEnemy.emname.Replace(" ", "_") + ".asset");
        flagReset();
        getEnemies();   
        for(int i = 0; i < enemyList.Count; i++)
        {
            if(enemyList[i].emname == enemyName)
            {
                currentChoice = i + 1;
            }
        }
        //newEnemy();
    }

    private void saveCurrentEnemy()
    {
        if (enemyName == "")
        {
            nameFlag = true;
            return;
        }
        if (enemySprite == null)
        {
            spriteFlag = true;
            return;
        }
        enemyList[currentChoice - 1].emname = enemyName;
        enemyList[currentChoice - 1].mySprite = enemySprite;
        enemyList[currentChoice - 1].health = enemyHealth;
        enemyList[currentChoice - 1].atk = enemyAttack;
        enemyList[currentChoice - 1].atkTime = enemyAttackTime;
        enemyList[currentChoice - 1].def = enemyDefence;
        enemyList[currentChoice - 1].isMagic = isMagic;
        enemyList[currentChoice - 1].manaPool = enemyMana;
    }

    private void flagReset()
    {
        nameFlag = false;
        existsFlag = false;
        spriteFlag = false;
    }
}
