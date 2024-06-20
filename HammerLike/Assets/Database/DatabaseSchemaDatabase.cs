using BennyKok.NotionAPI;
using UnityEngine;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

public class DatabaseSchemaDatabase : ScriptableObject
{
    [System.Serializable]
    public class Definition
    {
        public TitleProperty Name;
        public MultiSelectPropertyDefinition _type;
        public MultiSelectPropertyDefinition _category;
    }
    
    [System.Serializable]
    public class Properties
    {
        public NumberProperty armor;
        public MultiSelectProperty _type;
        public NumberProperty detectRange;
        public NumberProperty knockbackPower;
        public NumberProperty movementSpeed;
        public NumberProperty healthPoint;
        public CheckboxProperty Build;
        public NumberProperty attackSpeed;
        public NumberProperty attackRange;
        public NumberProperty knockbackResistance;
        public NumberProperty index;
        public TextProperty _koreanName;
        public NumberProperty attackPower;
        public MultiSelectProperty _category;
        public NumberProperty weight;
        public TitleProperty entityName;
    }
    
    public DatabaseSchema databaseSchema;
    public Database<Definition> database;
    public Page<Properties>[] pages;
    
    #if UNITY_EDITOR
    [EditorButton("Fetch Data From Notion", "SyncEditor")]
    public bool doSync;
    
    /*public void SyncEditor()
    {
        var api = new NotionAPI(databaseSchema.apiKey);
        EditorCoroutineUtility.StartCoroutine(api.GetDatabase<Definition>(databaseSchema.database_id, (db) => { database = db; }), this);
        EditorCoroutineUtility.StartCoroutine(api.QueryDatabase<Properties>(databaseSchema.database_id, (pages) => { this.pages = pages.results; }), this);
        UnityEditor.EditorUtility.SetDirty(this);
    }*/
    #endif
}