using UnityEngine;

namespace OperationBlackwell.Core
{
    public class Vent : MonoBehaviour, IInteractable
    {
        [SerializeField] private float TeleportPosX;
        [SerializeField] private float TeleportPosY;

        public void Interact() {
            CoreUnit player = GridCombatSystem.Instance.GetActiveUnit();
            player.transform.position = new Vector3(TeleportPosX, TeleportPosY, 0);
            // GameObject.Find("Player").transform.position = new Vector3(TeleportPosX, TeleportPosY, 0);
            //player.MoveTo(new Vector3(TeleportPosX, TeleportPosY, 0), () => {});
        }
    }
}
