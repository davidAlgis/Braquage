using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Key : Items
{
    [SerializeField]
    private GameObject m_doorAssociated;

    public GameObject DoorAssociated { get => m_doorAssociated; set => m_doorAssociated = value; }
}