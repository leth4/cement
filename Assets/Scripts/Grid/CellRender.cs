using UnityEngine;

public class CellRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _defaultSpriteBlack;
    [SerializeField] private Sprite _defaultSpriteWhite;
    [SerializeField] private Sprite _eraseSprite;
    [SerializeField] private Sprite _addSprite;

    public bool IsCellTaken => IsPlayerCell ? GridManager.Instance.PlayerGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public bool IsPreviewCellTaken => IsPlayerCell ? GridManager.Instance.PlayerPreviewGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];

    public Vector2Int Coordinates { get; private set; }
    public bool IsPlayerCell { get; private set; }
    public bool IsPhantom { get; private set; } = false;

    private float _appearDelay = 0;

    public void Initialize(bool isPlayerCell, Vector2Int coords, float appearDelay)
    {
        IsPlayerCell = isPlayerCell;
        Coordinates = coords;
        _appearDelay = appearDelay;
    }

    public void MakePhantom()
    {
        IsPhantom = true;
        _renderer.color = Color.clear;
    }

    public void Start()
    {
        transform.localScale = Vector3.zero;
        Tween.Delay(this, _appearDelay / 20, () => Tween.Scale(this, transform, Vector3.zero, Vector3.one, 0.4f, EaseType.SineOut));
    }

    public void Hide(float delay) => Tween.Delay(this, delay / 70, () => Tween.Scale(this, transform, Vector3.one, Vector3.zero, 0.4f, EaseType.SineOut));

    public void Render()
    {
        if (IsPhantom) return;

        if (AbilityController.Instance.IsPreviewing && IsPlayerCell)
        {
            if (IsPreviewCellTaken) _renderer.sprite = IsCellTaken ? _defaultSpriteBlack : _addSprite;
            else _renderer.sprite = IsCellTaken ? _eraseSprite : _defaultSpriteWhite;

            if (!IsPreviewCellTaken && IsCellTaken) _renderer.sprite = _eraseSprite;
        }
        else
        {
            _renderer.sprite = IsCellTaken ? _defaultSpriteBlack : _defaultSpriteWhite;
        }
    }

    private void OnEnable()
    {
        AbilityController.MadeChanges += Render;
    }

    private void OnDisable()
    {
        AbilityController.MadeChanges -= Render;
    }
}
