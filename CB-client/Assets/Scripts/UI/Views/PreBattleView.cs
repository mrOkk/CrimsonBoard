using TMPro;
using UnityEngine;

namespace CrimsonBoard
{
    public class PreBattleView : BaseView
    {
        [SerializeField] private TMP_Text _label;

        public System.Action OnPlayerInput;

        public override void Tick(float deltaTime)
        {
            if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                OnPlayerInput?.Invoke();
        }
    }
}
