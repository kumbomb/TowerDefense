using BaseEnum;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    public int width;
    public int height;
    public LocationType locationType;
    public TowerType type;
    public float attackPower;
    public float attackSpeed;
    public int towerLevel;
    public int towerUpGrade;

    public virtual void UpdateAbility()
    {

    }
}
