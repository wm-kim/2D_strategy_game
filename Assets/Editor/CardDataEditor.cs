using Sirenix.OdinInspector.Editor;
using Minimax.ScriptableObjects.CardDatas;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Minimax
{
    public class CardDataEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Card Data Editor")]
        private static void OpenWindow()
        {
            GetWindow<CardDataEditor>().Show();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            
            tree.Add("Create Random Cards", new CreateRandomCardData());
            tree.Add("Create New Card", new CreateNewCardData());
            tree.AddAllAssetsAtPath("Card Datas", "Assets/ScriptableObjects/CardDatas", typeof(CardBaseData));
            
            return tree;
        }

        public class CreateRandomCardData
        {
            public int NumberOfGeneratingCards = 1;
            
            [Button("Add Random Card Data")]
            private void CreateRandomCard()
            {
                for (int i = 0; i < NumberOfGeneratingCards; i++)
                {
                    var cardData = ScriptableObject.CreateInstance<CardBaseData>();
                    cardData.CardId = i;
                    cardData.CardName = "NewCard_" + i;
                    AssetDatabase.CreateAsset(cardData, $"Assets/ScriptableObjects/CardDatas/{cardData.CardName}.asset");
                }
                AssetDatabase.SaveAssets();
            }
        }
        
        public class CreateNewCardData
        {
            public CreateNewCardData()
            {
                CardData = ScriptableObject.CreateInstance<CardBaseData>();
                CardData.CardName = "New Card";
            }

            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public CardBaseData CardData;
            
            [Button("Add New Card Data")]
            private void CreateNewCard()
            {
                AssetDatabase.CreateAsset(CardData, $"Assets/ScriptableObjects/CardDatas/{CardData.CardName}.asset");
                AssetDatabase.SaveAssets();
                
                // create new instance of the card data SO 
                CardData = ScriptableObject.CreateInstance<CardBaseData>();
                CardData.CardName = "New Card";
            }
        }
    }
}
