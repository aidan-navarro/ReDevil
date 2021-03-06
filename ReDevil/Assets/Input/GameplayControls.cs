// GENERATED AUTOMATICALLY FROM 'Assets/Input/GameplayControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameplayControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameplayControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameplayControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""d5b25cba-e105-4c50-8c5a-a28f537b1b5a"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""979e346b-d27e-4772-bc8b-5e09e566eef1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""90244107-0b44-481e-959f-de18ed610c55"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""f290efee-7b2b-44dc-8bed-f52bfe9eab2c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Toggle Soul Armament"",
                    ""type"": ""Button"",
                    ""id"": ""a17b6638-c426-4b9b-8eda-7211fff192a0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Soul Power Shot"",
                    ""type"": ""Button"",
                    ""id"": ""c8a05d0f-1842-40a0-8fc0-f1ac5d1f0714"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash Left"",
                    ""type"": ""Button"",
                    ""id"": ""200ee355-2992-4155-8bdf-f1efca93d26f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash Right"",
                    ""type"": ""Button"",
                    ""id"": ""5501e298-cc16-4c5a-9cd4-933493fc90eb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""30fc28cf-ed75-4ffc-825b-e1f3ca21d0dc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d0540fd3-be2b-4bf3-8f7b-4a5f7eeafa81"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1d6c3d09-2abe-4a77-8d29-86a0f3e528bc"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3de75244-7d5e-48ff-8910-64fbae1394e5"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8d2b09b6-df12-4a6c-91dc-7d11f70d3b45"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9cca31f7-047c-498c-a66a-9900c81f4046"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""da5d1dc6-7f36-4d73-b9e5-9f7878207adc"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""33295bfc-0fdc-4177-91b7-eae8d6c442d9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0e8ad870-6dcc-4252-be87-5637bdb0b6dc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c29f0c06-4771-4e19-a744-bb98f0d1815e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4a7025dc-836e-4036-af20-1079179be02a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""a5d59849-5008-4d15-bc9c-2b4cb9ad5622"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6db2c75d-d9a3-4da8-a532-97a2bdcfa25d"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""35a3f415-b678-472b-ac0f-645f01c878e7"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0bf7e813-c38f-4b3f-a019-e8dd0ec4333e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""24acb07b-28a7-4d07-869d-f1305e8ce60f"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DPAD"",
                    ""id"": ""87159bee-8f6c-45d0-93b3-68eb5affaa78"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e49b4158-0244-4161-a770-49a35b677a24"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d0ab5cb2-3aae-4757-8478-b87447b361d3"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d069db59-7fd2-4cb1-9f37-96311da16f26"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""281f9cde-b37e-476a-8a7e-926ecd239ea0"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""341e0f27-1220-45c7-b715-95bb75a982fd"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd2673df-6323-4355-a3d9-f2b81f0d749d"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""352e2e51-969c-49f4-980d-54a085149278"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c5c574c-5aa9-4112-931e-006a317dc081"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc2b401b-5aaf-425b-9121-6f16624cca52"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Toggle Soul Armament"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b35cf72-ad9a-4aa1-8527-6a6c44b8b8ce"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Toggle Soul Armament"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5ce2192-05e7-4b0f-bd36-5567c491f64d"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Soul Power Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd2b9a94-bb39-491f-89bc-f90ab030a8d3"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Soul Power Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""815f96fe-c170-408b-8431-190b96b6107d"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14a8de45-1805-4212-9680-ecbfa5c38037"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Dash Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d99abd93-89f1-4fab-a92a-2cdf2a14cd35"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7c888d4-6379-422f-b864-891c7c98952e"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83703a1e-ecb4-4692-98d2-2af6e37d0e95"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a60c116d-b259-464b-a2f2-484ec7cafd22"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e68932f6-67ec-4f4b-83e7-3c8d6701f615"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""a336cac7-f03c-4d53-9d57-b0320fb47f12"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""38aba802-5f22-43e3-bfe5-46d6504ffd01"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d9dbb235-c138-4895-a0b2-522c5be8f13e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""271b9cee-a0d2-4256-8120-1bc0da736a35"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""EasterEgg"",
            ""id"": ""3bcf6b05-908a-438a-a174-8a53c2b4630f"",
            ""actions"": [
                {
                    ""name"": ""HitOni"",
                    ""type"": ""Button"",
                    ""id"": ""a9de320d-340b-4d4e-96c6-62279be14b24"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""14f8837f-f53d-4c24-836b-7ec168e32932"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HitOni"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""58268714-7136-4962-a57c-3bc9ef09f4b4"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HitOni"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Movement = m_Gameplay.FindAction("Movement", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_Attack = m_Gameplay.FindAction("Attack", throwIfNotFound: true);
        m_Gameplay_ToggleSoulArmament = m_Gameplay.FindAction("Toggle Soul Armament", throwIfNotFound: true);
        m_Gameplay_SoulPowerShot = m_Gameplay.FindAction("Soul Power Shot", throwIfNotFound: true);
        m_Gameplay_DashLeft = m_Gameplay.FindAction("Dash Left", throwIfNotFound: true);
        m_Gameplay_DashRight = m_Gameplay.FindAction("Dash Right", throwIfNotFound: true);
        m_Gameplay_Pause = m_Gameplay.FindAction("Pause", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Select = m_Menu.FindAction("Select", throwIfNotFound: true);
        // EasterEgg
        m_EasterEgg = asset.FindActionMap("EasterEgg", throwIfNotFound: true);
        m_EasterEgg_HitOni = m_EasterEgg.FindAction("HitOni", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Movement;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_Attack;
    private readonly InputAction m_Gameplay_ToggleSoulArmament;
    private readonly InputAction m_Gameplay_SoulPowerShot;
    private readonly InputAction m_Gameplay_DashLeft;
    private readonly InputAction m_Gameplay_DashRight;
    private readonly InputAction m_Gameplay_Pause;
    public struct GameplayActions
    {
        private @GameplayControls m_Wrapper;
        public GameplayActions(@GameplayControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Gameplay_Movement;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @Attack => m_Wrapper.m_Gameplay_Attack;
        public InputAction @ToggleSoulArmament => m_Wrapper.m_Gameplay_ToggleSoulArmament;
        public InputAction @SoulPowerShot => m_Wrapper.m_Gameplay_SoulPowerShot;
        public InputAction @DashLeft => m_Wrapper.m_Gameplay_DashLeft;
        public InputAction @DashRight => m_Wrapper.m_Gameplay_DashRight;
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @ToggleSoulArmament.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToggleSoulArmament;
                @ToggleSoulArmament.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToggleSoulArmament;
                @ToggleSoulArmament.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToggleSoulArmament;
                @SoulPowerShot.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSoulPowerShot;
                @SoulPowerShot.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSoulPowerShot;
                @SoulPowerShot.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSoulPowerShot;
                @DashLeft.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashLeft;
                @DashLeft.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashLeft;
                @DashLeft.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashLeft;
                @DashRight.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashRight;
                @DashRight.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashRight;
                @DashRight.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDashRight;
                @Pause.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @ToggleSoulArmament.started += instance.OnToggleSoulArmament;
                @ToggleSoulArmament.performed += instance.OnToggleSoulArmament;
                @ToggleSoulArmament.canceled += instance.OnToggleSoulArmament;
                @SoulPowerShot.started += instance.OnSoulPowerShot;
                @SoulPowerShot.performed += instance.OnSoulPowerShot;
                @SoulPowerShot.canceled += instance.OnSoulPowerShot;
                @DashLeft.started += instance.OnDashLeft;
                @DashLeft.performed += instance.OnDashLeft;
                @DashLeft.canceled += instance.OnDashLeft;
                @DashRight.started += instance.OnDashRight;
                @DashRight.performed += instance.OnDashRight;
                @DashRight.canceled += instance.OnDashRight;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Select;
    public struct MenuActions
    {
        private @GameplayControls m_Wrapper;
        public MenuActions(@GameplayControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_Menu_Select;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSelect;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // EasterEgg
    private readonly InputActionMap m_EasterEgg;
    private IEasterEggActions m_EasterEggActionsCallbackInterface;
    private readonly InputAction m_EasterEgg_HitOni;
    public struct EasterEggActions
    {
        private @GameplayControls m_Wrapper;
        public EasterEggActions(@GameplayControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @HitOni => m_Wrapper.m_EasterEgg_HitOni;
        public InputActionMap Get() { return m_Wrapper.m_EasterEgg; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(EasterEggActions set) { return set.Get(); }
        public void SetCallbacks(IEasterEggActions instance)
        {
            if (m_Wrapper.m_EasterEggActionsCallbackInterface != null)
            {
                @HitOni.started -= m_Wrapper.m_EasterEggActionsCallbackInterface.OnHitOni;
                @HitOni.performed -= m_Wrapper.m_EasterEggActionsCallbackInterface.OnHitOni;
                @HitOni.canceled -= m_Wrapper.m_EasterEggActionsCallbackInterface.OnHitOni;
            }
            m_Wrapper.m_EasterEggActionsCallbackInterface = instance;
            if (instance != null)
            {
                @HitOni.started += instance.OnHitOni;
                @HitOni.performed += instance.OnHitOni;
                @HitOni.canceled += instance.OnHitOni;
            }
        }
    }
    public EasterEggActions @EasterEgg => new EasterEggActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnToggleSoulArmament(InputAction.CallbackContext context);
        void OnSoulPowerShot(InputAction.CallbackContext context);
        void OnDashLeft(InputAction.CallbackContext context);
        void OnDashRight(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnSelect(InputAction.CallbackContext context);
    }
    public interface IEasterEggActions
    {
        void OnHitOni(InputAction.CallbackContext context);
    }
}
