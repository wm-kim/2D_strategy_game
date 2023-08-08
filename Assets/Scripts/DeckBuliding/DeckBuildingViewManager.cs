using System.Collections;
using System.Collections.Generic;
using Minimax.UI.View.ComponentViews;
using UnityEngine;

namespace Minimax
{
    /// <summary>
    /// Stores references to all the views in the deck building scene.
    /// </summary>
    public class DeckBuildingViewManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DeckListView m_deckListView;
        [SerializeField] private DBCardScrollView m_dbCardScrollView;
        [SerializeField] private DBCardItemMenuView m_dbCardItemMenuView;
        [SerializeField] private DeckListItemMenuView m_deckListItemMenuView;
        
        public DeckListView DeckListView => m_deckListView;
        public DBCardScrollView DBCardScrollView => m_dbCardScrollView;
        public DBCardItemMenuView DBCardItemMenuView => m_dbCardItemMenuView;
        public DeckListItemMenuView DeckListItemMenuView => m_deckListItemMenuView;
    }
}
