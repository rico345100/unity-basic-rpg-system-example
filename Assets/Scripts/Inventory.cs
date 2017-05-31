using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	private GameObject inventoryPanel;
	private GameObject slotPanel;
	private ItemDatabase database;
	private GameObject equipment;

	private int equippedGloves, equippedWeapon;
	private enum equipmentType { GLOVES = 1, WEAPON = 2 };
	private GameObject movingItem;

	public GameObject inventorySlot;
	public GameObject inventoryItem;

	int totalSlots;
	public List<Item> items = new List<Item>();
	public List<GameObject> slots = new List<GameObject>();

	public void SetMovingItem(GameObject item) {
		movingItem = item;
	}

	void Start() {
		database = GetComponent<ItemDatabase>();
		equippedGloves = -1;
		equippedWeapon = -1;

		totalSlots = 20;
		inventoryPanel = GameObject.Find("Inventory Panel");
		slotPanel = inventoryPanel.transform.Find("Slot Panel").gameObject;
		equipment = GameObject.Find("Equipment");

		for(int i = 0; i < totalSlots; i++) {
			items.Add(new Item());
			slots.Add(Instantiate(inventorySlot));
			slots[i].GetComponent<Slot>().idx = i;
			slots[i].transform.SetParent(slotPanel.transform);
		}

		AddItemById(0);
		AddItemById(1);
		AddItemById(0);
		AddItemById(1);
		AddItemById(2);
		AddItemById(2);
		AddItemById(2);
		AddItemById(2);
	}

	public void AddItemById(int id) {
		Item itemToAdd = database.FetchItemById(id);

		if(itemToAdd.Stackable && CheckIfItemIsInInventory(itemToAdd)) {
			for(int i = 0; i < items.Count; i++) {
				if(items[i].ID == id) {
					ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
					data.amount++;
					data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
					break;
				}
			}
		}
		else {
			for(int i = 0; i < items.Count; i++) {
				if(items[i].ID == -1) {
					items[i] = itemToAdd;
					GameObject itemObj = Instantiate(inventoryItem);

					itemObj.GetComponent<ItemData>().item = itemToAdd;
					itemObj.GetComponent<ItemData>().amount = 1;
					itemObj.GetComponent<ItemData>().slotIdx = i;
					itemObj.transform.SetParent(slots[i].transform);
					itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
					itemObj.name = itemToAdd.Title;
					break;
				}
			}
		}
	}

	public void UseItemByIndex(int slotIdx) {
		bool used = _UseItemByIndex(slotIdx);

		if(used && !items[slotIdx].IsEquipment) {
			Transform targetSlotPanel = slots[slotIdx].transform.GetChild(0);
			ItemData targetItemData = targetSlotPanel.GetComponent<ItemData>();
			targetItemData.amount -= 1;

			if(targetItemData.amount == 1) {
				targetSlotPanel.GetChild(0).GetComponent<Text>().text = "";
			}
			else if(targetItemData.amount <= 0) {
				ResetSlot(slotIdx);
			}
			else {
				targetSlotPanel.GetChild(0).GetComponent<Text>().text = targetItemData.amount.ToString();
			}
		}
	}

	private void equipItem(int slotIdx, Transform targetSlot) {
		Item item = items[slotIdx];

		slots[slotIdx].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Equipped";

		targetSlot.GetComponent<Image>().sprite = item.Sprite;
		targetSlot.GetChild(0).GetComponent<Text>().text = item.Title;
	}

	private void unequipItem(int slotIdx, Transform targetSlot) {
		Item item = items[slotIdx];

		slots[slotIdx].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "";

		targetSlot.GetComponent<Image>().sprite = null;

		string emptyText;

		switch(item.EquipmentType) {
			case (int) equipmentType.GLOVES:
				emptyText = "Gloves: none";
				break;
			case (int) equipmentType.WEAPON:
				emptyText = "Weapon: none";
				break;
			default:
				throw new System.InvalidOperationException("UNSUPPORTED EQUIPMENT TYPE.");
				break;
		}

		targetSlot.GetChild(0).GetComponent<Text>().text = emptyText;
	}

	private bool _UseItemByIndex(int slotIdx) {
		Item item = items[slotIdx];
		Transform targetSlot;

		if(item.IsEquipment) {
			switch(item.EquipmentType) {
				case (int) equipmentType.GLOVES: 
					targetSlot = equipment.transform.GetChild(1);

					// Set glove to used item if it wasn't set
					if(equippedGloves == -1) {
						equippedGloves = slotIdx;
						equipItem(slotIdx, targetSlot);
					}
					else {
						// In this case, there are two more available situations.
						if(equippedGloves == slotIdx) {
							equippedGloves = -1;
							unequipItem(slotIdx, targetSlot);
						}
						// First is user used same equipment. In this case, have to unequip.
						// Second is used different equipment. In this case, have to replace.
						else {
							// Reset previous equipped item's related jobs
							slots[equippedGloves].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "";

							equippedGloves = slotIdx;
							equipItem(slotIdx, targetSlot);
						}
					}

					break;
				case (int) equipmentType.WEAPON:
					targetSlot = equipment.transform.GetChild(2);

					// Set glove to used item if it wasn't set
					if(equippedWeapon == -1) {
						equippedWeapon = slotIdx;
						equipItem(slotIdx, targetSlot);
					}
					else {
						// In this case, there are two more available situations.
						if(equippedWeapon == slotIdx) {
							equippedWeapon = -1;
							unequipItem(slotIdx, targetSlot);
						}
						// First is user used same equipment. In this case, have to unequip.
						// Second is used different equipment. In this case, have to replace.
						else {
							// Reset previous equipped item's related jobs
							slots[equippedWeapon].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "";

							equippedWeapon = slotIdx;
							equipItem(slotIdx, targetSlot);
						}
					}

					break;
				default:
					throw new System.InvalidOperationException("UNSUPPORTED EQUIPMENT TYPE.");
			}
		}

		return true;
	}

	public void ResetSlot(int slotIdx) {
		items[slotIdx] = new Item();
		Destroy(slots[slotIdx].transform.GetChild(0).gameObject);
	}

	bool CheckIfItemIsInInventory(Item item)  {
		for(int i = 0; i < items.Count; i++) {
			if(items[i].ID == item.ID) {
				return true;
			}
		}
		
		return false;
	}

	public void DropItemByIndex(int slotIdx) {
		Item item = items[slotIdx];

		if(slotIdx == equippedGloves) {
			unequipItem(slotIdx, equipment.transform.GetChild(1));
		}
		else if(slotIdx == equippedWeapon) {
			unequipItem(slotIdx, equipment.transform.GetChild(2));
		}

		items[slotIdx] = new Item();
		Destroy(movingItem);
	}
}
