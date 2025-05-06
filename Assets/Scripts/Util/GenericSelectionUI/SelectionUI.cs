using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum SelectionType { List, Grid}

namespace Utils.GenericSelectionUI
{
    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        List<T> items;
        protected int selectedItem = 0;

        float selectionTimer = 0f;

        public List<T> Items { 
            get { return items; }
            set { items = value; } 
        }

        public event Action<int> OnSelected;
        public event Action OnBack;

        SelectionType type = SelectionType.Grid;
        int gridWidth = 1;

        public void SetSelectionType(SelectionType type, int gridWidth)
        {
            this.type = type;
            this.gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)
        {
            foreach (var item in items)
            {
                item.Init();
            }
            this.items = items;
            UpdateSelectionUI();
        }

        public virtual void HandleUpdate()
        {
            int prevSelection = selectedItem;

            if (type == SelectionType.List)
                HandleListSelection();
            else if (type == SelectionType.Grid)
                HandleGridSelection();

            selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

            if (prevSelection != selectedItem)
            {
                UpdateSelectionUI();
            }

            HandleSelectionTimer();

            if (Input.GetButtonDown("Action"))
            {
                OnSelected?.Invoke(selectedItem);
            }
            else if (Input.GetButtonDown("Back"))
            {
                OnBack?.Invoke();
            }
        }
        void HandleListSelection()
        {
            float v = Input.GetAxis("Vertical");
            if (selectionTimer < 0.05f && Mathf.Abs(v)>0.2f)
            {
                selectedItem -= (int) Mathf.Sign(v);
                selectionTimer = 0.4f;
            }

            if (Mathf.Abs(v) < 0.2f)
                selectionTimer = 0f;

        }

        void HandleGridSelection()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            if (selectionTimer < 0.05f && (Mathf.Abs(v) > 0.2f || Mathf.Abs(h) > 0.2f))
            {
                if (Mathf.Abs(v) < Mathf.Abs(h))
                    selectedItem += (int)Mathf.Sign(h);
                else 
                    selectedItem -= gridWidth * (int)Mathf.Sign(v);

                selectionTimer = 0.4f;
            }

            if (Mathf.Abs(v) < 0.2f && Mathf.Abs(h) < 0.2f)
                selectionTimer = 0f;
        }

        public virtual void UpdateSelectionUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelectionChanged(i == selectedItem);
            }
        }

        void HandleSelectionTimer()
        {
            if (selectionTimer > 0f)
                selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime,0,1);
        }

        public void ClearItems()
        {
            items.ForEach(i => i.Clear());

            this.items = null;
        }
    }
}

