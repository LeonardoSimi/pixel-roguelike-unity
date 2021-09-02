using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnBecameVisible()
    {
        this.enabled = true;
    }

    private void OnBecameInvisible()
    {
        this.enabled = false;
    }
}
