using System;
using System.Collections.Generic;
using UnityEngine;
using DEAL.Event;
using UnityEngine.Serialization;
using System.Collections;
using UnityEngine.UI;

namespace DEAL.UI
{
    public class BetManViewManager : MonoBehaviour, IEventListener
    {
        static BetManViewManager _instance = null;

        public static BetManViewManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BetManViewManager>();
                    if (_instance == null)
                    {
                        var _cache = new GameObject("BetManViewManager");
                        _instance = _cache.AddComponent<BetManViewManager>();
                    }
                }

                return _instance;
            }
        }

        protected delegate void ViewEventHandler(ViewEventPayload payload);
        private readonly Dictionary<string, ViewEventHandler> _viewEventHandlers = new Dictionary<string, ViewEventHandler>();

        [FormerlySerializedAs("m_cardPlayerUI")]
        [SerializeField]
        protected DialogManager mDialogManager;

        [SerializeField]
        private Animator mAnimator;

        [SerializeField]
        private Slider mCountDownSlider;

        private void Awake()
        {
            AttachViewEventListeners();
            NetworkEventSystem.AttachListener(EventType.VIEW_EVENT, this);
        }

        private void OnDestroy()
        {
            NetworkEventSystem.DetachListener(EventType.VIEW_EVENT, this);
        }

        public void OnClickDealJokers()
        {
            var payload = new StateEventPayload()
            {
                eventKey = "FinishSelect"
            };
            NetworkEventSystem.BroadcastEvent(EventType.STATE_EVENT, payload);
        }

        public void OnEvent(EventMsg e)
        {
            switch (e.Type)
            {
                case EventType.VIEW_EVENT:
                    {
                        if (e.Data is ViewEventPayload payload)
                        {
                            if (_viewEventHandlers.ContainsKey(payload.eventKey))
                            {
                                _viewEventHandlers[payload.eventKey](payload);
                            }

                            Debug.Log("payload.eventKey: " + payload.eventKey);

                            if (payload.data is CountdownData countdownData)
                            {
                                Debug.Log("payload.data: " + payload.data);
                                StartCoroutine(CountdownCoroutine(countdownData.duration, countdownData.callbackEventKey));
                            }
                        }
                        else
                        {
                            throw new System.Exception("Invalid Payload Type");
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Invalid Event Type");
                    }
            }
        }

        protected void AttachViewEventListener(string eventKey, ViewEventHandler callback)
        {
            _viewEventHandlers[eventKey] = callback;
        }

        protected virtual void AttachViewEventListeners()
        {
            AttachViewEventListener("OpenDialog", OnOpenDialog);
        }

        private void OnOpenDialog(ViewEventPayload payload)
        {
            var data = (PopDialogData)payload.data;
            if (data.TargetIds == null || data.TargetIds.Length == 0)
            {
                var localAvatar = DealEngine.Instance.LocalAvatar;
                data.CallbackActionTargetId = localAvatar.PlayerId;
                mDialogManager.OpenDialog(localAvatar, data);
            }
            else
            {
                foreach (var avatarId in data.TargetIds)
                {
                    data.CallbackActionTargetId = avatarId;
                    var avatar = NetworkEventSystem.Find<Avatar>(avatarId);
                    switch (avatar.AvatarType)
                    {
                        case GlobalEnum.AvatarType.Npc:
                            avatar.OnNpcProcessDialog(data);
                            break;
                        case GlobalEnum.AvatarType.Player:
                            mDialogManager.OpenDialog(avatar, data);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private IEnumerator CountdownCoroutine(float duration, string callbackEventKey)
        {
            yield return new WaitForSeconds(duration);
            NetworkEventSystem.BroadcastEvent(EventType.STATE_EVENT, new ViewEventPayload { eventKey = callbackEventKey });
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

        public void OnClickGameNormalToVictory()
        {
            mAnimator.SetTrigger("Victory"); // 轉到 ani_Victory
        }

        public void OnClickGameNormalToFail()
        {
            mAnimator.SetTrigger("Fail"); // 轉到 ani_Fail
        }
    }
}
