using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Items : ScriptableObject
{
    private string m_name;
    private uint m_id;

    public string Name { get => m_name; set => m_name = value; }
    public uint Id { get => m_id; set => m_id = value; }
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Keys", order = 1)]
public class Keys : Items
{
    [SerializeField]
    private GameObject m_doorAssociated;


}