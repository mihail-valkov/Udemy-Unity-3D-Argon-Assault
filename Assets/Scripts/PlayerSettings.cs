using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] float maxHealthValue = 100f;
    [SerializeField] float hitDamage = 10f;

    public float MaxHealthValue { get { return maxHealthValue; } }
    public float HitDamage { get { return hitDamage; } }
}
