using UnityEngine;

[SelectionBase]
public class GridDot : MonoBehaviour
{
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}