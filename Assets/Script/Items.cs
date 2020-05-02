using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Items : MonoBehaviour
{
    private string m_name;
    private uint m_id;

    public string Name { get => m_name; set => m_name = value; }
    public uint Id { get => m_id; set => m_id = value; }
}
