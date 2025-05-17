using System.Collections;
using System.Collections.Generic;
using DEAL.Event;
using UnityEngine;

namespace DEAL
{
    public class GameManager : MonoBehaviour
    {
        public void OnClickDealJokers()
        {
            var payload = new StateEventPayload()
            {
                eventKey = "FinishSelect"
            };
            NetworkEventSystem.BroadcastEvent(EventType.STATE_EVENT, payload);
        }
    }
}
