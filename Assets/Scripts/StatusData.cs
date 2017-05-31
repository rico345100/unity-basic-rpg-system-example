using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusData : MonoBehaviour {
    public StatusInfo statusInfo;
    public int slotIdx;
    public int spentVirtualPoint;

    private Status status;

    void Start() {
        status = GameObject.Find("Status").GetComponent<Status>();
    }
}
