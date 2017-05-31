using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour {
	private List<Item> database = new List<Item>();
    private JsonData itemData;

    void Start() {
        // Application.dataPath refers Assets directory
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));

        ConstructItemDatabase();
    }

    public Item FetchItemById(int id) {
        for(int i = 0; i < database.Count; i++) {
            if(database[i].ID == id) {
                return database[i];
            }
        }

        return null;
    }

    void ConstructItemDatabase() {
        for(int i = 0; i < itemData.Count; i++) {
            database.Add(
                new Item(
                    (int) itemData[i]["id"],
                    (string) itemData[i]["title"],
                    (int) itemData[i]["value"],
                    (int) itemData[i]["stats"]["power"],
                    (int) itemData[i]["stats"]["defence"],
                    (int) itemData[i]["stats"]["vitality"],
                    (string) itemData[i]["description"],
                    (bool) itemData[i]["stackable"],
                    (int) itemData[i]["rarity"],
                    (bool) itemData[i]["isEquipment"],
                    (int) itemData[i]["equipmentType"],
                    (string) itemData[i]["slug"]
                )
            );
        }
    }
}

public class Item {
    public int ID { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
    public int Power { get; set; }
    public int Defence { get; set; }
    public int Vitality { get; set; }
    public string Description { get; set; }
    public bool Stackable { get; set; }
    public int Rarity { get; set; }
    public string Slug { get; set; }
    public bool IsEquipment { get; set; }
    public int EquipmentType { get; set; }
    public Sprite Sprite { get; set; }

    public Item(int id, string title, int value, int power, int defence, int vitality, string description, bool stackable, int rarity, bool isEquipment, int equipmentType, string slug) {
        this.ID = id;
        this.Title = title;
        this.Value = value;
        this.Power = power;
        this.Vitality = vitality;
        this.Description = description;
        this.Stackable = stackable;
        this.Rarity = rarity;
        this.IsEquipment = isEquipment;
        this.EquipmentType = equipmentType;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Sprites/" + slug);
    }
    public Item() {
        this.ID = -1;
    }
}