using UnityEngine;

public class GridBorder : MonoBehaviour
{
    [SerializeField] private Transform _tr;
    
    public void SetSize(int size)
    {
        _tr.localScale = new Vector3(1, size, 1);
    }
}