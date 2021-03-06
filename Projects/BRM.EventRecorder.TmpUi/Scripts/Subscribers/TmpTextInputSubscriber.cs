﻿using System.Collections.Generic;
using BRM.EventRecorder.UnityUi.Models;
using BRM.EventRecorder.UnityUi.Subscribers;
using TMPro;
using UnityEngine.Events;

namespace BRM.EventRecorder.TmpUi.Subscribers
{
    public class TmpTextInputSubscriber : SelectableSubscriber<TMP_InputField, TextInputEvent>
    {
        public override string Name => nameof(TmpTextInputSubscriber);
        
        private readonly List<TextInputEvent> _events = new List<TextInputEvent>();
        private readonly List<UnityAction<string>> _onTmpInputFieldEndEdits = new List<UnityAction<string>>();

        public override void UnsubscribeAll()
        {
            base.UnsubscribeAll();
            _onTmpInputFieldEndEdits.Clear();
        }

        public override EventModelCollection ExtractNewEvents()
        {
            var collection = new EventModelCollection();
            collection.TextInputEvents.AddRange(_events);
            _events.Clear();
            return collection;
        }
        
        
        protected override void OnSubscribe(TMP_InputField tmpInput)
        {
            UnityAction<string> onInputFieldEndEdit = newValue =>
            {
                var newEvent = new TextInputEvent(TmpEventNames.TmpTextInputEvent)
                {
                    PropertyName = "text",
                    NewValue = newValue,//todo: encrypt for sensitive data
                };
                PopulateCommonEventData(newEvent, tmpInput.transform);
                _events.Add(newEvent);
            };
            _onTmpInputFieldEndEdits.Add(onInputFieldEndEdit);
            tmpInput.onEndEdit.AddListener(onInputFieldEndEdit);
        }

        protected override void OnUnsubscribe(TMP_InputField tmpInput)
        {
            _onTmpInputFieldEndEdits.ForEach(tmpInput.onEndEdit.RemoveListener);
        }
    }
}