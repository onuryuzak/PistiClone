using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TableSettings", menuName = "Pisti/Table Settings")]
public class TableSettings : ScriptableObject
{
    public TableType TableType;
    public string TableName => TableType.ToString();
    public int MinBet;
    public int MaxBet;
}