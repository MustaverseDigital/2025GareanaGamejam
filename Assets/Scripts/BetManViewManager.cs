using System.Collections;
using UnityEngine;
using DEAL.Event;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

namespace DEAL.UI
{
    public class BetManViewManager : ViewManager
    {
        public MMF_Player VictoryFeedback;

        [SerializeField]
        private Animator mAnimator;

        [SerializeField]
        private Slider mCountDownSlider;
        
        private bool isCountdownActive = false;



        protected override void Awake()
        {
            _instance = this;
            base.Awake();
        }

        protected override void AttachViewEventListeners()
        {
            base.AttachViewEventListeners();
            AttachViewEventListener("Victory", GameNormalToVictory);
            AttachViewEventListener("Lose", GameNormalToFail);
        }

        protected override void OnStopCountDown(ViewEventPayload payload)
        {
            isCountdownActive = false;
        }

        protected override IEnumerator CountdownCoroutine(float maxTime, float duration, string callbackEventKey)
        {
            isCountdownActive = true;
            mCountDownSlider.maxValue = maxTime == 0 ? duration : maxTime;
            mCountDownSlider.value = duration;
            while (duration > 0 && isCountdownActive)
            {
                duration -= Time.deltaTime;
                mCountDownSlider.value = duration;
                yield return null;
            }

            if (isCountdownActive)
            {
                NetworkEventSystem.BroadcastEvent(EventType.STATE_EVENT, new ViewEventPayload { eventKey = callbackEventKey });
                isCountdownActive = false;
            }
            NetworkEventSystem.BroadcastEvent(EventType.ACTION_EVENT, new ActionPayload
            {
                eventKey = "PropertyChange",
                receiverViewIds = DealEngine.Instance.AllPlayerIds,
                senderViewId = DealEngine.Instance.LocalAvatarId,
                dataType = GlobalEnum.PropertyType.Float,
                data = duration,
                actionName = "AvailableTime",
            });
        }

        public void OnClickDealJokers()
        {
            var payload = new StateEventPayload()
            {
                eventKey = "FinishSelect"
            };
            NetworkEventSystem.BroadcastEvent(EventType.STATE_EVENT, payload);
        }

        // Animator State Transition Methods
        public void OnClickStartToInfo()
        {
            mAnimator.SetTrigger("Info"); // 轉到 Info
        }

        public void OnClickStartToGameIn()
        {
            mAnimator.SetTrigger("In"); // 轉到 Game_In
        }

        // Info 狀態
        public void OnClickInfoToStart()
        {
            mAnimator.SetTrigger("Out"); // 轉到 Start
        }

        public void OnClickBackToStart()
        {
            mAnimator.SetTrigger("Out"); // 轉到 Game_Normal
        }

        // Game_Normal 狀態
        public void OnClickGameNormalToInfo()
        {
            mAnimator.SetTrigger("Info"); // 轉到 Info
        }

        public void GameNormalToVictory(ViewEventPayload payload)
        {
            mAnimator.SetTrigger("Victory"); // 轉到 ani_Victory
            VictoryFeedback.PlayFeedbacks();
        }

        public void GameNormalToFail(ViewEventPayload payload)
        {
            mAnimator.SetTrigger("Fail"); // 轉到 ani_Fail
        }
    }
}
