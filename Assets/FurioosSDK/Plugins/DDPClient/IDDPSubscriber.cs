using DdpClient.Models.Server;

namespace DdpClient
{
    public interface IDdpSubscriber<T>
    {
        void Added(SubAddedModel<T> added);

        void AddedBefore(SubAddedBeforeModel<T> addedBefore);

        void MovedBefore(SubMovedBeforeModel movedBefore);

        void Changed(SubChangedModel<T> changed);

        void Removed(SubRemovedModel removed);
    }
}