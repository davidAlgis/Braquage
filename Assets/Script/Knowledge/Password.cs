using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Password : Knowledge
{
    [SerializeField]
    private GameObject m_elementAssociated;

    [SerializeField]
    private string m_password;

    public string getPassword { get => m_password; set => m_password = value; }
    public GameObject ElementAssociated { get => m_elementAssociated; set => m_elementAssociated = value; }
}
