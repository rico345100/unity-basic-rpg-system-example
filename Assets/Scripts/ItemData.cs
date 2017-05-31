using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Drag Events are for handling drag and drop inventory,
// Point Events are for handling Tooltip
public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler  {
	public Item item;
	public int slotIdx;	// Which slot in?
	public int amount;	// How many items have per slot?

	private Inventory inv;
	private Tooltip tooltip;
	private Vector2 offset;
	private bool isDragging = false;

	void Start() {
		inv = GameObject.Find("Inventory").GetComponent<Inventory>();
		tooltip = inv.GetComponent<Tooltip>();
	}
	
	public void OnBeginDrag(PointerEventData eventData) {
		if(item != null) {
			inv.SetMovingItem(this.transform.gameObject);

			offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
			this.transform.SetParent(this.transform.parent.parent);
			isDragging = true;
			GetComponent<CanvasGroup>().blocksRaycasts = false;	// This is for dragging event will not fired correctly
		}
	}

	public void OnDrag(PointerEventData eventData) {
		if(item != null && isDragging) {
			this.transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		inv.SetMovingItem(null);

		this.transform.SetParent(inv.slots[slotIdx].transform);
		this.transform.position = inv.slots[slotIdx].transform.position;
		isDragging = false;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		tooltip.Activate(item);
	}

	public void OnPointerExit(PointerEventData eventData) {
		tooltip.Deactivate();
	}

	private float doubleClickTimeout = 0.2f;

	private IEnumerator checkDoubleClick(System.Action callback) {
		float endTime = Time.time + doubleClickTimeout;

		while(Time.time < endTime) {
			if(Input.GetMouseButtonDown(0)) {
				callback();
			}

			yield return 0;
		}

		yield break;		// Stop Coroutine
	}

	public void OnPointerUp(PointerEventData eventData) {
		StartCoroutine(checkDoubleClick(() => {
			inv.UseItemByIndex(slotIdx);
		}));
	}

	// IPointerUpHandler is not working properly if only it has implemented,
	// At least it requires IPointerDownHandler or IPointerClickHandler to work.
	// It sucks :(
	public void OnPointerDown(PointerEventData eventData) {
		// Do nothing
		return;
	}
}
