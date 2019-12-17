using System;
using System.Collections.Generic;
using System.Diagnostics;
using DdpClient.Models.Server;

namespace DdpClient
{
    public class DdpSubscriber<T> : IDisposable where T : DdpDocument
    {
        private readonly WebSocketAdapterBase _webSocketAdapterBase;

        public EventHandler<SubAddedModel<T>> Added;
        public EventHandler<SubAddedBeforeModel<T>> AddedBefore;
        public EventHandler<SubChangedModel<T>> Changed;
        public EventHandler<SubMovedBeforeModel> MovedBefore;
        public EventHandler<SubRemovedModel> Removed;

        internal DdpSubscriber(WebSocketAdapterBase webSocketAdapter, string name)
        {
            _webSocketAdapterBase = webSocketAdapter;
            _webSocketAdapterBase.DdpMessage += OnDdpMessage;

            Name = name;
            Subscribers = new List<IDdpSubscriber<T>>();
        }

        public List<IDdpSubscriber<T>> Subscribers { get; set; } 

        public string Name { get; set; }

        private void HandleAdded(SubAddedModel<T> added)
        {
            added.Object.Id = added.Id;
            Added?.Invoke(this, added);
            Subscribers.ForEach(sub => sub.Added(added));
        }

        private void HandleChanged(SubChangedModel<T> changed)
        {
            Changed?.Invoke(this, changed);
            Subscribers.ForEach(sub => sub.Changed(changed));
        }

        private void HandleRemoved(SubRemovedModel removed)
        {
            Removed?.Invoke(this, removed);
            Subscribers.ForEach(sub => sub.Removed(removed));
        }

        private void HandleMovedBefore(SubMovedBeforeModel movedBefore)
        {
            MovedBefore?.Invoke(this, movedBefore);
            Subscribers.ForEach(sub => sub.MovedBefore(movedBefore));
        }

        private void HandleAddedBefore(SubAddedBeforeModel<T> addedBefore)
        {
            AddedBefore?.Invoke(this, addedBefore);
            Subscribers.ForEach(sub => sub.AddedBefore(addedBefore));
        }

        private void OnDdpMessage(object sender, DdpMessage e)
        {
            Debug.WriteLine(e.Msg);
            if (e.Body["collection"] == null)
                return;
            if (e.Body["collection"].ToObject<string>() != Name)
                return;

            switch (e.Msg)
            {
                case "added":
                    HandleAdded(e.Get<SubAddedModel<T>>());
                    break;
                case "changed":
                    HandleChanged(e.Get<SubChangedModel<T>>());
                    break;
                case "removed":
                    HandleRemoved(e.Get<SubRemovedModel>());
                    break;
                case "addedBefore":
                    HandleAddedBefore(e.Get<SubAddedBeforeModel<T>>());
                    break;
                case "movedBefore":
                    HandleMovedBefore(e.Get<SubMovedBeforeModel>());
                    break;
            }
        }

        public void Dispose()
        {
            _webSocketAdapterBase.DdpMessage -= OnDdpMessage;
        }
    }
}