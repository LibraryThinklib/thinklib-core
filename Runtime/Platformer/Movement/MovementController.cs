using UnityEngine;

[AddComponentMenu("Thinklib/Platformer/Movement/Movement Controller", -99)]
public class MovementController : MonoBehaviour
{
    protected float currentSpeed;
    protected Vector2 direction;

    public virtual void Move(Vector2 inputDirection, float speed)
    {
        direction = inputDirection.normalized;
        currentSpeed = speed;
        transform.Translate(direction * currentSpeed * Time.deltaTime);
    }
}
