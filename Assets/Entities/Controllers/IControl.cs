using UnityEngine;

public interface IControl
{
    public void OnDrag(float deltaX, float deltaY);
    public float GetValue();
}
