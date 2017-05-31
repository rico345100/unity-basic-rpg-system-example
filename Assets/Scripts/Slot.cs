using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
	public int idx;
	private Inventory inv;

	void Start() {
		inv = GameObject.Find("Inventory").GetComponent<Inventory>();
	}

	// Remember that this OnDrop event fires before ItemData.OnEndDrag. So give it to slot index to proper job.
	public void OnDrop(PointerEventData eventData) {
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

		// If item slot is empty, move the item to new slot and give empty item to old slot.
		if(inv.items[idx].ID == -1) {
			inv.items[droppedItem.slotIdx] = new Item();	// Give empty item
			inv.items[idx] = droppedItem.item;
			droppedItem.slotIdx = idx;
		}
		// Else, means that item is already in new slot. In this case, just swap the items.
		// Addiotional condition added, because when you drag a item, that container does not have child and it causes error.
		else if(droppedItem.slotIdx != idx) {
			Transform item = this.transform.GetChild(0);
			item.GetComponent<ItemData>().slotIdx = droppedItem.slotIdx;
			item.transform.SetParent(inv.slots[droppedItem.slotIdx].transform);
			item.transform.position = inv.slots[droppedItem.slotIdx].transform.position;

			inv.items[droppedItem.slotIdx] = item.GetComponent<ItemData>().item;
			inv.items[idx] = droppedItem.item;

			droppedItem.slotIdx = idx;
		}
	}
}
