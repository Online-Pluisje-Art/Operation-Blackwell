namespace OperationBlackwell.Core {
    public interface IInteractable {
        void Interact();
        bool IsInRange(CoreUnit unit);
        int GetCost();
    }
}