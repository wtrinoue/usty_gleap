using UnityEngine;

public class modifierMonoBehaviour : MonoBehaviour
{
    public ModifierDefinition modifierDefinition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentPosition = transform.position;
        currentPosition.x -= 0.02f;
        transform.position = currentPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StatusContainer mm = collision.gameObject.GetComponent<StatusContainer>();
        if (mm == null) return;
        Modifier modifier = new Modifier(modifierDefinition, this.gameObject);
        mm.AddModifier(modifier);
    }
}
