/*
 * InteractManager.cs - by ThunderWire Studio
 * ver. 2.0
*/

using UnityEngine;
using UnityEngine.UI;
using ThunderWire.Utility;
using ThunderWire.Input;
using HFPS.Systems;
using HFPS.UI;

namespace HFPS.Player
{
    /// <summary>
    /// Main Interact Manager
    /// </summary>
    public class InteractManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private HFPS_GameManager GameManager;
        [SerializeField]
        private Inventory Inventory;
        [SerializeField]
        private ScriptManager ScriptManager;

        private ItemSwitcher itemSelector;

        private DynamicObject dynamicObj;
        private InteractiveItem interactItem;
        private UIObjectInfo objectInfo;
        private DraggableObject dragRigidbody;

        [Header("Raycast")]
        public Camera MainCamera;
        public float RaycastRange = 3;
        public LayerMask cullLayers;
        public LayerMask interactLayers;

        [Header("Crosshair Textures")]
        public Sprite defaultCrosshair;
        public Sprite interactCrosshair;
        private Sprite default_interactCrosshair;

        [Header("Crosshair")]
        private Image CrosshairUI;
        public int crosshairSize = 5;
        public int interactSize = 10;

        #region Texts
        private string TakeText;
        private string UseText;
        private string GrabText;
        private string UnlockText;
        private string DragText;
        private string ExamineText;
        private string RemoveText;

        private string PickupHint;
        private string PickupMessage;
        private string BackpacksMax;
        private string CantTake;
        private string NoInventorySpace;
        #endregion

        #region Private Variables
        [HideInInspector] public bool isHeld = false;
        [HideInInspector] public bool inUse;
        [HideInInspector] public Ray playerAim;
        [HideInInspector] public GameObject RaycastObject;
        private GameObject LastRaycastObject;

        private int default_interactSize;
        private int default_crosshairSize;

        private string bp_Use;
        private string bp_Pickup;
        private bool UsePressed;

        private bool isPressed;
        private bool isCorrectLayer;
        #endregion

        void Awake()
        {
            itemSelector = ScriptManager.Get<ItemSwitcher>();

            CrosshairUI = GameManager.userInterface.Crosshair;
            default_interactCrosshair = interactCrosshair;
            default_crosshairSize = crosshairSize;
            default_interactSize = interactSize;
            RaycastObject = null;
            dynamicObj = null;

            if (TextsSource.HasReference) 
                TextsSource.Subscribe(OnInitTexts);
            else OnInitTexts();
        }

        private void OnInitTexts()
        {
            TakeText = TextsSource.GetText("Interact.Take", "Take");
            UseText = TextsSource.GetText("Interact.Use", "Use");
            GrabText = TextsSource.GetText("Interact.Grab", "Grab");
            UnlockText = TextsSource.GetText("Interact.Unlock", "Unlock");
            DragText = TextsSource.GetText("Interact.Drag", "Drag");
            ExamineText = TextsSource.GetText("Interact.Examine", "Examine");
            RemoveText = TextsSource.GetText("Interact.Remove", "Remove");
            PickupHint = TextsSource.GetText("Interact.PickupHint", "You took the");
            PickupMessage = TextsSource.GetText("Interact.PickupMessage", "Picked up");
            BackpacksMax = TextsSource.GetText("Interact.BackpacksMax", "You can't carry more backpacks!");
            CantTake = TextsSource.GetText("Interact.CantTake", "You can't take more");
            NoInventorySpace = TextsSource.GetText("Interact.NoInventorySpace", "No inventory space!");
        }

        private void Update ()
        {
            if ( InputHandler.InputIsInitialized )
            {
                bp_Use = InputHandler.CompositeOf ( "Use" ).bindingPath;
                bp_Pickup = InputHandler.CompositeOf ( "Examine" ).bindingPath;
                UsePressed = InputHandler.ReadButtonOnce ( this, "Use" );
            }

            if ( UsePressed && RaycastObject && !isPressed && !isHeld && !inUse && !GameManager.isWeaponZooming )
            {
                Interact ( RaycastObject );
                isPressed = true;
            }

            if ( !UsePressed && isPressed )
            {
                isPressed = false;
            }
        }

        void FixedUpdate()
        {
            Ray playerAim = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(playerAim, out RaycastHit hit, RaycastRange, cullLayers))
            {
                if (interactLayers.CompareLayer(hit.collider.gameObject.layer) && !GameManager.isWeaponZooming)
                {
                    if (hit.collider.gameObject != RaycastObject)
                    {
                        GameManager.HideSprites(0);
                    }

                    RaycastObject = hit.collider.gameObject;
                    dragRigidbody = RaycastObject.GetComponent<DraggableObject>();
                    isCorrectLayer = true;

                    RaycastObject.HasComponent(out interactItem);
                    RaycastObject.HasComponent(out dynamicObj);
                    RaycastObject.HasComponent(out objectInfo);

                    if (RaycastObject.HasComponent(out CrosshairReticle reticle))
                    {
                        if (dynamicObj)
                        {
                            if (dynamicObj.useType != Type_Use.Locked)
                            {
                                interactCrosshair = reticle.interactSprite;
                                interactSize = reticle.size;
                            }
                        }
                        else
                        {
                            interactCrosshair = reticle.interactSprite;
                            interactSize = reticle.size;
                        }
                    }

                    if (LastRaycastObject)
                    {
                        if (!(LastRaycastObject == RaycastObject))
                        {
                            ResetCrosshair();
                        }
                    }
                    LastRaycastObject = RaycastObject;

                    if (objectInfo && !string.IsNullOrEmpty(objectInfo.ObjectTitle))
                    {
                        GameManager.ShowInteractInfo(objectInfo.ObjectTitle);
                    }

                    if (!inUse)
                    {
                        if (dynamicObj)
                        {
                            if (dynamicObj.useType == Type_Use.Locked && dynamicObj.CheckHasKey())
                            {
                                GameManager.ShowInteractSprite(1, UnlockText, bp_Use);
                            }
                            else
                            {
                                if (!objectInfo || (!objectInfo && objectInfo.overrideDoorText))
                                {
                                    if (dynamicObj.interactType == Type_Interact.Mouse)
                                    {
                                        GameManager.ShowInteractSprite(1, DragText, bp_Use);
                                    }
                                    else
                                    {
                                        GameManager.ShowInteractSprite(1, UseText, bp_Use);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(objectInfo.UseText))
                                        GameManager.ShowInteractSprite(1, objectInfo.UseText, bp_Use);
                                    else
                                        GameManager.ShowInteractSprite(1, UseText, bp_Use);
                                }
                            }
                        }
                        else
                        {
                            if (interactItem)
                            {
                                if (interactItem.interactShowTitle && !string.IsNullOrEmpty(interactItem.examineTitle))
                                {
                                    GameManager.ShowInteractInfo(interactItem.examineTitle);
                                }

                                if (!dragRigidbody || dragRigidbody && !dragRigidbody.dragAndUse)
                                {
                                    if (interactItem.itemType == InteractiveItem.ItemType.OnlyExamine)
                                    {
                                        GameManager.ShowInteractSprite(1, ExamineText, bp_Pickup);
                                    }
                                    else if (interactItem.itemType == InteractiveItem.ItemType.GenericItem)
                                    {
                                        if (interactItem.examineType != InteractiveItem.ExamineType.None)
                                        {
                                            GameManager.ShowInteractSprite(1, UseText, bp_Use);
                                            GameManager.ShowInteractSprite(2, ExamineText, bp_Pickup);
                                        }
                                        else
                                        {
                                            GameManager.ShowInteractSprite(1, UseText, bp_Use);
                                        }
                                    }
                                    else if (interactItem.examineType != InteractiveItem.ExamineType.None && interactItem.itemType != InteractiveItem.ItemType.GenericItem)
                                    {
                                        GameManager.ShowInteractSprite(1, TakeText, bp_Use);
                                        GameManager.ShowInteractSprite(2, ExamineText, bp_Pickup);
                                    }
                                    else if (interactItem.examineType == InteractiveItem.ExamineType.Paper)
                                    {
                                        GameManager.ShowInteractSprite(1, ExamineText, bp_Pickup);
                                    }
                                    else
                                    {
                                        GameManager.ShowInteractSprite(1, TakeText, bp_Use);
                                    }
                                }
                                else if (dragRigidbody && dragRigidbody.dragAndUse)
                                {
                                    if (interactItem.itemType != InteractiveItem.ItemType.OnlyExamine)
                                    {
                                        GameManager.ShowInteractSprite(1, TakeText, bp_Use);
                                        GameManager.ShowInteractSprite(2, GrabText, bp_Pickup);
                                    }
                                }
                            }
                            else if (RaycastObject.GetComponent<DynamicObjectPlank>())
                            {
                                GameManager.ShowInteractSprite(1, RemoveText, bp_Use);
                            }
                            else if (dragRigidbody)
                            {
                                if (!dragRigidbody.dragAndUse)
                                {
                                    GameManager.ShowInteractSprite(1, GrabText, bp_Pickup);
                                }
                                else if(objectInfo && !string.IsNullOrEmpty(objectInfo.UseText))
                                {
                                    GameManager.ShowInteractSprite(1, objectInfo.UseText, bp_Use);
                                    GameManager.ShowInteractSprite(2, GrabText, bp_Pickup);
                                }
                                else
                                {
                                    GameManager.ShowInteractSprite(1, UseText, bp_Use);
                                    GameManager.ShowInteractSprite(2, GrabText, bp_Pickup);
                                }
                            }
                            else if (objectInfo && !string.IsNullOrEmpty(objectInfo.UseText))
                            {
                                GameManager.ShowInteractSprite(1, objectInfo.UseText, bp_Use);
                            }
                            else
                            {
                                GameManager.ShowInteractSprite(1, UseText, bp_Use);
                            }
                        }
                    }

                    CrosshairChange(true);
                }
                else if (RaycastObject)
                {
                    isCorrectLayer = false;
                }
            }
            else if (RaycastObject)
            {
                isCorrectLayer = false;
            }

            if (!isCorrectLayer)
            {
                ResetCrosshair();
                CrosshairChange(false);
                GameManager.HideSprites(0);
                interactItem = null;
                RaycastObject = null;
                dynamicObj = null;
            }

            if (!RaycastObject)
            {
                GameManager.HideSprites(0);
                CrosshairChange(false);
                dynamicObj = null;
            }
        }

        void CrosshairChange(bool useTexture)
        {
            if (useTexture && CrosshairUI.sprite != interactCrosshair)
            {
                CrosshairUI.sprite = interactCrosshair;
                CrosshairUI.GetComponent<RectTransform>().sizeDelta = new Vector2(interactSize, interactSize);
            }
            else if (!useTexture && CrosshairUI.sprite != defaultCrosshair)
            {
                CrosshairUI.sprite = defaultCrosshair;
                CrosshairUI.GetComponent<RectTransform>().sizeDelta = new Vector2(crosshairSize, crosshairSize);
            }

            CrosshairUI.DisableSpriteOptimizations();
        }

        private void ResetCrosshair()
        {
            crosshairSize = default_crosshairSize;
            interactSize = default_interactSize;
            interactCrosshair = default_interactCrosshair;
        }

        public void CrosshairVisible ( bool state ) => CrosshairUI.enabled = state;

        public bool GetInteractBool () => RaycastObject != null;

        public void Interact(GameObject InteractObject)
        {
            InteractiveItem interactiveItem = interactItem;

            if (!interactiveItem)
                InteractObject.HasComponent(out interactiveItem);

            if (interactiveItem && interactiveItem.itemType == InteractiveItem.ItemType.OnlyExamine) 
                return;

            if (InteractObject.HasComponent(out Message msg))
            {
                if (msg.messageType == Message.MessageType.Hint)
                {
                    GameManager.ShowHintPopup(msg.message, msg.messageTime);
                }
                else if (msg.messageType == Message.MessageType.PickupHint)
                {
                    GameManager.ShowHintPopup($"{PickupHint} {msg.message}", msg.messageTime);
                }
                else if (msg.messageType == Message.MessageType.Message)
                {
                    GameManager.ShowQuickMessage(msg.message, "");
                }
                else if (msg.messageType == Message.MessageType.ItemName)
                {
                    GameManager.ShowQuickMessage($"{PickupMessage} {msg.message}", "");
                }
            }

            if (interactiveItem)
            {
                Item item = new Item();
                bool showMessage = true;
                string autoShortcut = string.Empty;

                if (interactiveItem.itemType == InteractiveItem.ItemType.InventoryItem || interactiveItem.itemType == InteractiveItem.ItemType.SwitcherItem)
                    item = Inventory.GetItem(interactiveItem.inventoryID);

                if (interactiveItem.itemType == InteractiveItem.ItemType.GenericItem)
                {
                    InteractEvent(InteractObject);
                }
                else if (interactiveItem.itemType == InteractiveItem.ItemType.BackpackExpand)
                {
                    if ((Inventory.settings.SlotAmount + interactiveItem.invExpandAmount) > Inventory.settings.MaxSlots)
                    {
                        GameManager.ShowQuickMessage(BackpacksMax, "", true);
                        return;
                    }

                    Inventory.ExpandSlots(interactiveItem.invExpandAmount);
                    InteractEvent(InteractObject);
                }
                else if (interactiveItem.itemType == InteractiveItem.ItemType.InventoryItem)
                {
                    if (Inventory.CheckInventorySpace() || Inventory.CheckItemInventoryStack(interactiveItem.inventoryID))
                    {
                        if (Inventory.GetItemAmount(item.ID) < item.Settings.maxStackAmount || item.Settings.maxStackAmount == 0)
                        {
                            autoShortcut = Inventory.AddItem(interactiveItem.inventoryID, interactiveItem.itemAmount, interactiveItem.itemData, interactiveItem.autoShortcut);
                            InteractEvent(InteractObject);
                        }
                        else if (Inventory.GetItemAmount(item.ID) >= item.Settings.maxStackAmount)
                        {
                            GameManager.ShowQuickMessage($"{CantTake} {item.Title}", "MaxItemCount");
                            showMessage = false;
                        }
                    }
                    else
                    {
                        GameManager.ShowQuickMessage(NoInventorySpace, "NoSpace");
                        showMessage = false;
                    }
                }
                else if (interactiveItem.itemType == InteractiveItem.ItemType.SwitcherItem)
                {
                    if (Inventory.CheckInventorySpace() || Inventory.CheckItemInventoryStack(interactiveItem.inventoryID))
                    {
                        if (Inventory.GetItemAmount(item.ID) < item.Settings.maxStackAmount || item.Settings.maxStackAmount == 0)
                        {
                            autoShortcut = Inventory.AddItem(interactiveItem.inventoryID, interactiveItem.itemAmount, null, interactiveItem.autoShortcut);

                            if (interactiveItem.autoSwitch)
                            {
                                itemSelector.SelectSwitcherItem(interactiveItem.switcherID);
                            }

                            if (item.ItemType == ItemType.Weapon)
                            {
                                itemSelector.weaponItem = interactiveItem.switcherID;
                            }

                            InteractEvent(InteractObject);
                        }
                        else if (Inventory.GetItemAmount(item.ID) >= item.Settings.maxStackAmount)
                        {
                            GameManager.ShowQuickMessage($"{CantTake} {item.Title}", "MaxItemCount");
                            showMessage = false;
                        }
                    }
                    else
                    {
                        GameManager.ShowQuickMessage(NoInventorySpace, "NoSpace");
                        showMessage = false;
                    }
                }
                else if (interactiveItem.itemType == InteractiveItem.ItemType.GenericItem)
                {
                    InteractEvent(InteractObject);
                }

                if (showMessage)
                {
                    if (interactiveItem.messageType == InteractiveItem.MessageType.PickupHint)
                    {
                        foreach (var tip in interactiveItem.messageTips)
                        {
                            if (tip.InputAction.Equals("?"))
                            {
                                tip.InputAction = autoShortcut;
                                break;
                            }
                        }

                        if (interactiveItem.useDefaultHint) 
                            GameManager.ShowHintPopup($"{PickupHint} {interactiveItem.titleOrMsg.ToLower()}", interactiveItem.messageTime, interactiveItem.messageTips);
                        else
                            GameManager.ShowHintPopup(interactiveItem.titleOrMsg, interactiveItem.messageTime, interactiveItem.messageTips);
                    }
                    else if (interactiveItem.messageType == InteractiveItem.MessageType.Message)
                    {
                        GameManager.ShowQuickMessage(interactiveItem.titleOrMsg, "");
                    }
                    else if (interactiveItem.messageType == InteractiveItem.MessageType.ItemName)
                    {
                        GameManager.ShowQuickMessage($"{PickupMessage} {interactiveItem.titleOrMsg}", "");
                    }
                }
            }
            else
            {
                InteractEvent(InteractObject);
            }
        }

        void InteractEvent(GameObject InteractObject)
        {
            GameManager.HideSprites(0);
            InteractObject.SendMessage("UseObject", SendMessageOptions.DontRequireReceiver);
        }
    }
}