using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Change Text Color", order: 2)]
    public class ChangeTextColorTransition : Transition
    {
        [Title("Values")]
        [SerializeField] Color startColor = new Color(0.3254902f, 0.227451f, 0.1176471f);
        [SerializeField] Color endColor = new Color(0.9607844f, 0.9450981f, 0.9019608f);

        TMP_Text text;

        public override float StartDuration => 0;

        public override float EndDuration => 0;

        public new void Awake()
        {
            base.Awake();

            text = GetComponentInChildren<TMP_Text>();
        }

        public override void PlayStartTransition()
        {
            GoToEndState();
        }

        public override void PlayEndTransition()
        {
            GoToStartState();
        }

        public override void GoToStartState()
        {
            GetComponentInChildren<TMP_Text>().color = startColor;
        }

        public override void GoToEndState()
        {
            GetComponentInChildren<TMP_Text>().color = endColor;
        }

        protected override void OnSkip()
        {

        }
    }
}