using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : Singleton<AbilityController>
{
    public static event Action MadeChanges;

    [SerializeField] private Transform _abilityCardContainer;
    [SerializeField] private AbilityCard _abilityCardPrefab;

    public bool IsPreviewing = false;

    public void UpdatePreview()
    {
        GridManager.Instance.PlayerPreviewGrid = GridManager.Instance.PlayerGrid.Clone() as Cell[,];
        IsPreviewing = true;
        if (GridManager.Instance.SelectedCell != null && HandController.Instance.ActiveAbility != null)
        {
            HandController.Instance.ActiveAbility.Apply(GridManager.Instance.PlayerPreviewGrid, GridManager.Instance.SelectedCell.Coordinates);
        }
        MadeChanges?.Invoke();
    }

    public void CallChange() => MadeChanges?.Invoke();

    public void StopPreview()
    {
        IsPreviewing = false;
        MadeChanges?.Invoke();
    }

    public void TryApplyingAbility()
    {
        if (GridManager.Instance.SelectedCell != null && HandController.Instance.ActiveAbility != null)
        {
            StopPreview();

            Recorder.Instance.Record(HandController.Instance.ActiveAbility);
            var applied = HandController.Instance.ActiveAbility.Apply(GridManager.Instance.PlayerGrid, GridManager.Instance.SelectedCell.Coordinates);
            if (applied)
            {
                HandController.Instance.DestroyDraggedCard();
                MadeChanges?.Invoke();
            }
            else
            {
                Recorder.Instance.RemoveLastRecord();
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Recorder.Instance.GoBack();
        }
    }
}
