using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Knowledge : MonoBehaviour
{
    [SerializeField]
    private string m_name;

    public string Name { get => m_name; set => m_name = value; }
}

