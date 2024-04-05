using System;
using UnityEngine;

public class AbilityController : Singleton<AbilityController>
{
    public static event Action OnMadeChanges;

    [SerializeField] private GameObject _border;

    public bool IsPreviewing { get; private set; } = false;

    public void UpdatePreview()
    {
        if (GridManager.Instance.LastSelectedCell == null) return;
        if (HandController.Instance.ActiveAbility == null) return;

        _border.SetActive(HandController.Instance.ActiveAbility.IsFullCanvas);
        GridManager.Instance.PlayerPreviewGrid = GridManager.Instance.PlayerGrid.Clone() as bool[,];
        HandController.Instance.ActiveAbility.Apply(GridManager.Instance.PlayerPreviewGrid, GridManager.Instance.LastSelectedCell.Coordinates);

        IsPreviewing = true;
        OnMadeChanges?.Invoke();
    }

    public void StopPreview()
    {
        _border.SetActive(false);
        GridManager.Instance.PlayerPreviewGrid = GridManager.Instance.PlayerGrid.Clone() as bool[,];

        IsPreviewing = false;
        OnMadeChanges?.Invoke();
    }

    public void ApplyAbility()
    {
        if (GridManager.Instance.LastSelectedCell == null) return;
        if (HandController.Instance.ActiveAbility == null) return;

        StopPreview();

        Recorder.Instance.Record(HandController.Instance.ActiveAbility);

        HandController.Instance.ActiveAbility.Apply(GridManager.Instance.PlayerGrid, GridManager.Instance.LastSelectedCell.Coordinates);
        HandController.Instance.DestroyDraggedCard();

        OnMadeChanges?.Invoke();

        Screenshake.Instance.Shake(0.03f, 0.05f);
        AudioReceiver.AbilityApplied();
    }
}
