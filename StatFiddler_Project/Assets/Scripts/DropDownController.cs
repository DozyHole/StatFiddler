﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Dropdown))]
[DisallowMultipleComponent]
public class DropDownController : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Indexes that should be ignored. Indexes are 0 based.")]
    public List<int> indexesToDisable = new List<int>();

    private Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<Dropdown>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var dropDownList = GetComponentInChildren<Canvas>();
        if (!dropDownList) return;

        // If the dropdown was opened find the options toggles
        var toogles = dropDownList.GetComponentsInChildren<Toggle>(true);

        // the first item will always be a template item from the dropdown we have to ignore
        // so we start at one and all options indexes have to be 1 based
        for (var i = 1; i < toogles.Length; i++)
        {
            // disable buttons if their 0-based index is in indexesToDisable
            // the first item will always be a template item from the dropdown
            // so in order to still have 0 based indexes for the options here we use i-1
            toogles[i].interactable = !indexesToDisable.Contains(i - 1);
        }
    }
}
