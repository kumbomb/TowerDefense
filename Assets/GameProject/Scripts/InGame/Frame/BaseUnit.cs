using System;
using UnityEngine;

//모든 적 유닛, 플레이어 캐릭터, 소환 수들의 베이스 
public abstract class BaseUnit : MonoBehaviour 
{
    [Header("Unit Stats")]
    public float moveSpeed = 5f;

    [Header("Status")]
    public bool isDead = false;

}
