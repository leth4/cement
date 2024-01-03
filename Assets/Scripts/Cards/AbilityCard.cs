using UnityEngine;

public class AbilityCard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    public Ability Ability { get; private set; }

    public void Initialize(Ability ability)
    {
        Ability = ability;
        _renderer.sprite = ability.Image;
    }

    public void MoveAway()
    {
        Tween.Translate(this, transform, transform.position, transform.position + transform.up * 0.2f, 0.13f, EaseType.QuintOut, ended: () =>
        Tween.Translate(this, transform, transform.position, transform.position - transform.up * 10, 0.3f, EaseType.Linear, ended: () =>
        Destroy(gameObject)));
    }

    private void OnMouseEnter()
    {
        AudioReceiver.CardHover();
    }
}
