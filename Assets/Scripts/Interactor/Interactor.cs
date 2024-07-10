using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private EMaterialType _MaterialType;

    public EMaterialType MaterialType { get => _MaterialType; set => _MaterialType = value; }
}
