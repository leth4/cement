using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public static event Action Solved;

    public int Size => _size;

    [SerializeField] private CellRender _cellPrefab;
    [SerializeField] private LayerMask _cellLayer;
    [SerializeField] private Vector3 _touchSelectOffset;
    [SerializeField] private int _size;
    [SerializeField] private int _numbersToPreview;
    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;

    [HideInInspector] public CellRender SelectedCell { get; private set; }

    public bool[,] PlayerGrid;
    public bool[,] PlayerPreviewGrid;
    public bool[,] AnswerGrid;

    private List<CellRender> AnswerRenders = new();
    private List<CellRender> PlayerRenders = new();

    public void InitializeGrids(bool[,] shape)
    {
        AnswerRenders.ForEach(cell => Destroy(cell.gameObject));
        PlayerRenders.ForEach(cell => Destroy(cell.gameObject));
        AnswerRenders = new();
        PlayerRenders = new();

        AnswerGrid = shape.Clone() as bool[,];
        PlayerGrid = new bool[_size, _size];
        PlayerPreviewGrid = new bool[_size, _size];

        for (int i = -1; i <= _size; i++)
        {
            for (int j = -1; j <= _size; j++)
            {
                bool isPhantomCell = i < 0 || i >= _size || j < 0 || j >= _size;

                if (!isPhantomCell)
                {
                    var answerCellRender = Instantiate(_cellPrefab, _answerGridContainer);
                    answerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                    answerCellRender.Initialize(false, new(i, j), i - j);
                    AnswerRenders.Add(answerCellRender);
                }

                var playerCellRender = Instantiate(_cellPrefab, _playerGridContainer);
                playerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                playerCellRender.Initialize(true, new(i, j), i - j);

                if (isPhantomCell)
                {
                    playerCellRender.MakePhantom();
                }
                else
                {
                    PlayerRenders.Add(playerCellRender);
                }
            }
        }

        PlayerGrid[_size / 2, _size / 2] = true;
    }

    public void MakeDisappear()
    {
        for (int i = 0; i < AnswerRenders.Count; i++)
        {
            AnswerRenders[i].Hide(i);
            PlayerRenders[i].Hide(i);
        }
    }

    private void CheckWin()
    {
        for (int i = 0; i < _size; i++)
            for (int j = 0; j < _size; j++)
                if (AnswerGrid[i, j] != PlayerGrid[i, j]) return;

        Solved?.Invoke();
    }

    public void Update()
    {
        var deviceOffset = Application.isMobilePlatform ? _touchSelectOffset : Vector3.zero;

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) + deviceOffset, Vector2.zero, 1000, _cellLayer);

        if (hit.collider == null)
        {
            if (SelectedCell != null) AbilityController.Instance.StopPreview();
            SelectedCell = null;
            return;
        }

        var cellRender = hit.transform.GetComponent<CellRender>();

        if (!cellRender.IsPlayerCell)
        {
            if (SelectedCell != null) AbilityController.Instance.StopPreview();
            SelectedCell = null;
            return;
        }

        if (cellRender.IsPhantom && HandController.Instance.ActiveAbility && HandController.Instance.ActiveAbility.IsFullCanvas)
        {
            if (SelectedCell != null) AbilityController.Instance.StopPreview();
            SelectedCell = null;
            return;
        }

        bool isNewCell = SelectedCell != cellRender;
        SelectedCell = cellRender;

        if (isNewCell) AbilityController.Instance.UpdatePreview();
    }

    public void Activate(Ability ability) => ability.ApplyRandom(AnswerGrid);

    private void OnEnable()
    {
        AbilityController.MadeChanges += CheckWin;
    }

    private void OnDisable()
    {
        AbilityController.MadeChanges -= CheckWin;
    }
}
