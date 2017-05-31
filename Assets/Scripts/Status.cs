using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour {
	private GameObject statusPanel;
	private GameObject statusListPanel;
	private GameObject statusPointObject;
	private GameObject applyButton;
	private List<StatusInfo> statusList = new List<StatusInfo>();	
	private List<GameObject> statusSlots = new List<GameObject>();
	private int availableStatusPoints;
	
	public GameObject statusSlot;
	
	public void GiveStatusPoint(int points) {
		availableStatusPoints += points;
	}
	public void UpdateStatusPointUI() {
		statusPointObject.GetComponent<Text>().text = "Available Points: " + availableStatusPoints.ToString();
	}

	void Start() {
		GiveStatusPoint(10);

		statusPanel = GameObject.Find("Status Panel");
		statusListPanel = GameObject.Find("Status List Panel");
		statusPointObject = statusPanel.transform.Find("Available Points").gameObject;
		applyButton = statusPanel.transform.Find("Apply Button").gameObject;

		applyButton.GetComponent<Button>().onClick.AddListener(ApplySpentStatusPoints);

		statusList.Add(new StatusInfo("MaxHP", 100.0f));
		statusList.Add(new StatusInfo("MaxMP", 100.0f));
		statusList.Add(new StatusInfo("Power", 10.0f));
		statusList.Add(new StatusInfo("Defense", 10.0f));
		statusList.Add(new StatusInfo("Vitality", 0.0f));

		initializeGUI();
	}

	private void initializeGUI() {
		UpdateStatusPointUI();

		for(int i = 0; i < statusList.Count; i++) {
			CreateStatusSlot(statusList[i]);
		}
	}

	private void UpdateStatusSlot(GameObject slot) {
		StatusData statusData = slot.GetComponent<StatusData>();
		StatusInfo statusInfo = statusData.statusInfo;
		string statusText;

		if(statusData.spentVirtualPoint > 0) {
			statusText = statusInfo.name + ": <color=#ff0000>" + (statusInfo.value + statusData.spentVirtualPoint) + "</color>    <color=#00ffff>+" + statusData.spentVirtualPoint + "</color>";
		}
		else {
			statusText = statusInfo.name + ": " + statusInfo.value;
		}

		slot.transform.GetChild(0).GetComponent<Text>().text = statusText;
	}

	private void CreateStatusSlot(StatusInfo statusInfo) {
		GameObject instance = Instantiate(statusSlot);
		instance.name = statusInfo.name;
		instance.GetComponent<StatusData>().statusInfo = statusInfo;
		instance.transform.SetParent(statusListPanel.transform);

		StatusData statusData = instance.GetComponent<StatusData>();

		UpdateStatusSlot(instance);
		Button upButton = instance.transform.GetChild(1).GetComponent<Button>();
		Button downButton = instance.transform.GetChild(2).GetComponent<Button>();
		
		upButton.onClick.AddListener(() => {
			if(availableStatusPoints <= 0) return;

			availableStatusPoints--;
			statusData.spentVirtualPoint++;

			UpdateStatusPointUI();
			UpdateStatusSlot(instance);
		});

		downButton.onClick.AddListener(() => {
			if(statusData.spentVirtualPoint <= 0) return;

			availableStatusPoints++;
			statusData.spentVirtualPoint--;

			UpdateStatusPointUI();
			UpdateStatusSlot(instance);
		});

		statusSlots.Add(instance);
	}

	private void ApplySpentStatusPoints() {
		for(int i = 0; i < statusSlots.Count; i++) {
			GameObject statusSlot = statusSlots[i];
			StatusData statusData = statusSlot.GetComponent<StatusData>();

			if(statusData.spentVirtualPoint <= 0) continue;

			StatusInfo statusInfo = statusData.statusInfo;
			statusInfo.value += statusData.spentVirtualPoint;
			statusData.spentVirtualPoint = 0;

			UpdateStatusSlot(statusSlot);
		}
	}
}

public class StatusInfo {
	public string name;
	public float value;
	
	public StatusInfo(string name, float value) {
		this.name = name;
		this.value = value;
	}
}