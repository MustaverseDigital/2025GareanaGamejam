using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using DEAL;

public class HorizontalHandSorter : MonoBehaviour
{
    [SerializeField] private CardUGUI selectedCard;
    [SerializeReference] private CardUGUI hoveredCard;
    private RectTransform rect;
    public List<CardUGUI> cards;

    [SerializeField] private float cardSpacing = 300f;
    [SerializeField] private float swapAnimationDuration = 0.15f;
    [SerializeField] private bool tweenCardReturn = true;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        Initialize();
    }

    private void OnTransformChildrenChanged()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Clear old event listeners
        if (cards != null)
        {
            foreach (CardUGUI card in cards)
            {
                if (card != null)
                {
                    RemoveCardListeners(card);
                }
            }
        }

        // Get new card list
        cards = GetComponentsInChildren<CardUGUI>().ToList();
        SortCardsByHierarchy();

        // Add event listeners to new cards
        for (int i = 0; i < cards.Count; i++)
        {
            CardUGUI card = cards[i];
            AddCardListeners(card);
            card.name = i.ToString();
        }
    }

    private void AddCardListeners(CardUGUI card)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);
    }

    private void RemoveCardListeners(CardUGUI card)
    {
        card.PointerEnterEvent.RemoveListener(CardPointerEnter);
        card.PointerExitEvent.RemoveListener(CardPointerExit);
        card.BeginDragEvent.RemoveListener(BeginDrag);
        card.EndDragEvent.RemoveListener(EndDrag);
    }

    private void SortCardsByHierarchy()
    {
        cards.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }

    private void BeginDrag(CardUGUI card)
    {
        selectedCard = card;
    }

    void EndDrag(CardUGUI card)
    {
        if (selectedCard == null)
            return;

        int currentIndex = selectedCard.ParentIndex();
        float xOffset = currentIndex * cardSpacing;
        // Vector3 targetPosition = new Vector3(xOffset, selectedCard.selected ? selectedCard.selectionOffset : 0, 0);

        // selectedCard.transform.DOLocalMove(targetPosition, tweenCardReturn ? swapAnimationDuration : 0)
        //     .SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;
    }

    void CardPointerEnter(CardUGUI card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(CardUGUI card)
    {
        hoveredCard = null;
    }

    void Update()
    {
        HandleInput();
        HandleCardSwapping();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Delete) && hoveredCard != null)
        {
            Destroy(hoveredCard.transform.parent.gameObject);
            cards.Remove(hoveredCard);
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (CardUGUI card in cards)
            {
                card.Deselect();
            }
        }
    }

    private void HandleCardSwapping()
    {
        if (selectedCard == null)
            return;

        float selectedCardX = selectedCard.transform.position.x;
        int selectedIndex = selectedCard.ParentIndex();

        for (int i = 0; i < cards.Count; i++)
        {
            if (i == selectedIndex)
                continue;

            float otherCardX = cards[i].transform.position.x;
            int otherIndex = cards[i].ParentIndex();

            // Check if cards should swap based on position
            if ((selectedCardX > otherCardX && selectedIndex < otherIndex) ||
                (selectedCardX < otherCardX && selectedIndex > otherIndex))
            {
                SwapCards(selectedIndex, otherIndex);
                break;
            }
        }
    }

    private void SwapCards(int index1, int index2)
    {
        // Swap in hierarchy
        Transform card1 = cards[index1].transform;
        Transform card2 = cards[index2].transform;
        
        int siblingIndex1 = card1.GetSiblingIndex();
        int siblingIndex2 = card2.GetSiblingIndex();
        
        card1.SetSiblingIndex(siblingIndex2);
        card2.SetSiblingIndex(siblingIndex1);
    }
}