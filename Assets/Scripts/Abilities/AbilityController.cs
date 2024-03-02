using System;
using UnityEngine;

public class AbilityController : Singleton<AbilityController>
{
    public static event Action MadeChanges;

    [SerializeField] private GameObject _border;

    public bool IsPreviewing { get; private set; } = false;

    public void UpdatePreview()
    {
        GridManager.Instance.PlayerPreviewGrid = GridManager.Instance.PlayerGrid.Clone() as bool[,];
        IsPreviewing = true;
        if (GridManager.Instance.SelectedCell != null && HandController.Instance.ActiveAbility != null)
        {
            _border.SetActive(HandController.Instance.ActiveAbility.IsFullCanvas);
            HandController.Instance.ActiveAbility.Apply(GridManager.Instance.PlayerPreviewGrid, GridManager.Instance.SelectedCell.Coordinates);
        }
        MadeChanges?.Invoke();
    }

    public void CallChange() => MadeChanges?.Invoke();

    public void StopPreview()
    {
        _border.SetActive(false);
        IsPreviewing = false;
        GridManager.Instance.PlayerPreviewGrid = GridManager.Instance.PlayerGrid.Clone() as bool[,];
        MadeChanges?.Invoke();
    }

    public void TryApplyingAbility()
    {
        if (GridManager.Instance.SelectedCell != null && HandController.Instance.ActiveAbility != null)
        {
            StopPreview();

            AudioReceiver.AbilityApplied();

            Recorder.Instance.Record(HandController.Instance.ActiveAbility);
            var applied = HandController.Instance.ActiveAbility.Apply(GridManager.Instance.PlayerGrid, GridManager.Instance.SelectedCell.Coordinates);
            if (applied)
            {
                HandController.Instance.DestroyDraggedCard();
                MadeChanges?.Invoke();
                Screenshake.Instance.Shake(0.03f, 0.05f);
            }
            else
            {
                Recorder.Instance.RemoveLastRecord();
            }
        }
    }

    private void UndoLastMove()
    {
        Recorder.Instance.GoBack(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && HandController.Instance.ActiveAbility == null && !GameManager.Instance.IsSolved) UndoLastMove();
    }
}
