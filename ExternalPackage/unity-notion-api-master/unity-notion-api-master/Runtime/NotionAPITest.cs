using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BennyKok.NotionAPI
{
    public class NotionAPITest : MonoBehaviour
    {
        public string apiKey;
        public string database_id;

        private IEnumerator Start()
        {
            var api = new NotionAPI(apiKey);

            yield return api.GetDatabase<CardDatabasePropertiesDefinition>(database_id, (db) =>
            {
                // 필요한 정보만 로깅
                //Debug.Log($"Database ID: {db.id}");
                //Debug.Log($"Created Time: {db.created_time}");
                //if (db.title.Any())
                //    Debug.Log($"Title: {db.title.First().text.content}");
            });

            yield return api.QueryDatabase<ItemDatabaseProperties>(database_id, (db) =>
            {
                // 각 레코드에 대한 필요한 정보만 출력
                foreach (var record in db.results)
                {
                    string name = record.properties.Name.title.FirstOrDefault()?.text.content ?? "No Name"; // 이름이 없으면 No Name 
                    string tag = record.properties.Tags?.Value.FirstOrDefault()?.name ?? "No Tag";
                    //string tag = record.properties.Tags.multi_select.ToString();
                    bool isActive = record.properties.Checkbox.Value;
                    float number = record.properties.index.Value;
                    string description = record.properties.Description.rich_text.FirstOrDefault()?.text.content ?? "No Description";    // 설명이 없으면 
                   
                    Debug.Log($"Number: {number},Name: {name},Tag:{tag},IsActive:{isActive}, Description: {description}");
                }
            });
        }

        [Serializable]
        public class CardDatabasePropertiesDefinition
        {
            public MultiSelectPropertyDefinition Tags;
            public TitleProperty Name;
            public CheckboxProperty Checkbox;
            public DateProperty Date;
            public SelectPropertyDefinition Type;
            public NumberProperty number;
            public TextProperty Description;
        }

        [Serializable]
        public class ItemDatabaseProperties
        {
            public MultiSelectProperty Tags;
            public TitleProperty Name;
            public CheckboxProperty Checkbox;
            public DateProperty Date;
            public SelectProperty Type;
            public NumberProperty index;
            public NumberProperty UseTime;
            public TextProperty Description;
        }

        public class EntityDatabaseProperties
        {
            public MultiSelectProperty Tags;
            public TitleProperty Name;
            public CheckboxProperty IsActive;
            public DateProperty Date;
            public SelectProperty Type;
            public NumberProperty number;
            //public NumberProperty UseTime;
            public TextProperty Description;
        }
    }
}