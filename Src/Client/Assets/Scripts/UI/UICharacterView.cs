using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UICharacterView:MonoBehaviour 
    {
        public GameObject[] characters;
        
        private int currentCharacter = 0;

        public int CurrentCharacter
        {
            get
            {
                return currentCharacter;
            }
            set
            {
                currentCharacter = value;
                this.UpdateCharacterView();
            }
        }


        private void UpdateCharacterView()
        {
            for (int i = 0; i < 3; i++)
            {
                characters[i].SetActive(i == currentCharacter);
            }
        }
        
    }
    
    
}