using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Crosshair : MonoBehaviour
{
    public float sensitivity = 0.1f;

    private Vector3 mousePos;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bc2d;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bc2d = GetComponent<BoxCollider2D>();

        bc2d.isTrigger = true;

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused)
        {
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = Vector2.Lerp(transform.position, mousePos, sensitivity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.GetType() == typeof(BoxCollider2D))
        {
            spriteRenderer.color = Color.magenta;
        }
        if (other.CompareTag("Player"))
        {
            spriteRenderer.color = Color.cyan;
        }
        //if (other.CompareTag("Friendly"))
        //{
        //    spriteRenderer.color = Color.yellow;
        //}
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        spriteRenderer.color = Color.white;
    }
}