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
            tree.Add("Create New Unit Card", new CreateNewUnitCardData());
            
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
                    var cardData = ScriptableObject.CreateInstance<UnitBaseData>();
                    cardData.CardId = i;
                    cardData.CardName = "NewCard_" + i;
                    cardData.Cost = Random.Range(1, 10);
                    cardData.Description = "This is a random card";
                    cardData.Attack = Random.Range(1, 10);
                    cardData.Health = Random.Range(1, 10);
                    cardData.MoveRange = Random.Range(1, 5);
                    cardData.AttackRange = Random.Range(1, 3);
                    AssetDatabase.CreateAsset(cardData, $"Assets/ScriptableObjects/CardDatas/{cardData.CardName}.asset");
                }
                AssetDatabase.SaveAssets();
            }
        }
        
        public class CreateNewUnitCardData
        {
            public CreateNewUnitCardData()
            {
                UnitBaseData = ScriptableObject.CreateInstance<UnitBaseData>();
                UnitBaseData.CardName = "New Unit Card";
            }

            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public UnitBaseData UnitBaseData;
            
            [Button("Add New Unit Card Data")]
            private void CreateNewCard()
            {
                AssetDatabase.CreateAsset(UnitBaseData, $"Assets/ScriptableObjects/CardDatas/{UnitBaseData.CardName}.asset");
                AssetDatabase.SaveAssets();
                
                // create new instance of the card data SO 
                UnitBaseData = ScriptableObject.CreateInstance<UnitBaseData>();
                UnitBaseData.CardName = "New Unit Card";
            }
        }
    }
}
