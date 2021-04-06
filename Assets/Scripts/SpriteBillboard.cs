using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool isFacingUp;

    private Camera cam;

    private void Start() 
    {
        cam = Camera.main;
    }
    private void LateUpdate()
    {

        if (isFacingUp)
        {
            // Makes sure the sprite is facing up
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            transform.LookAt(cam.transform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}
