using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minimax.ScriptableObjects.CardDatas;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Minimax
{
    public class CardEditorWindow : EditorWindow
    {
        [SerializeField] private int m_SelectedIndex = -1;
        private VisualElement m_RightPane;
        
        [MenuItem("Tools/Card Editor Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<CardEditorWindow>("Card Editor");
            window.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            CardBaseData[] allCards;
            FindAllCards(out allCards);
            
            // Create a two-pane view with the left pane being fixed with
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            // Add the view to the visual tree by adding it as a child to the root element
            rootVisualElement.Add(splitView);

            // A TwoPaneSplitView always needs exactly two child elements
            var leftPane = new ListView();
            splitView.Add(leftPane);
            m_RightPane = new ScrollView();
            splitView.Add(m_RightPane);
            
            // Initialize the list view
            leftPane.makeItem = () => new Label();
            leftPane.bindItem = (element, i) => (element as Label).text = allCards[i].name;
            leftPane.itemsSource = allCards;

            leftPane.selectionChanged += OnCardSelectionChange;
            
            // Store the selection index when the selection changes
            leftPane.selectionChanged += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
        }

        private void OnCardSelectionChange(IEnumerable<object> obj)
        {
            // Clear all previous content from the pane
            m_RightPane.Clear();

        }
        
        private void FindAllCards(out CardBaseData[] allCards)
        {
            var guids = AssetDatabase.FindAssets("t:CardBaseData");
            allCards = new CardBaseData[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                allCards[i] = AssetDatabase.LoadAssetAtPath<CardBaseData>(path);
            }
        }
    }
}
