//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Scripts/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""game"",
            ""id"": ""5b5f768a-6fcf-4c23-9fdd-12db4ba1d92a"",
            ""actions"": [
                {
                    ""name"": ""axis"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0fd05d3e-51ca-4e50-8919-2c4f247e7cd7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""shoot"",
                    ""type"": ""Button"",
                    ""id"": ""7e5e39b1-a9cd-40d2-a78a-e08f5ff405d6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""shootAlt"",
                    ""type"": ""Button"",
                    ""id"": ""8a5dbf94-1302-4a0f-a3fc-147e6b072654"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""restart"",
                    ""type"": ""Button"",
                    ""id"": ""86ca45b1-c1f7-4694-9e22-4a5af48c88ef"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""start_practice"",
                    ""type"": ""Button"",
                    ""id"": ""4ee227a3-4a5c-4b00-a8c2-6eba9b4a5258"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""end_practice"",
                    ""type"": ""Button"",
                    ""id"": ""9b150fcb-2a6e-4ed1-ae85-6cd7df01775b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""mouse"",
                    ""type"": ""Value"",
                    ""id"": ""54fcea8e-1176-42ff-b8e4-2cbe901e2f21"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Arrows"",
                    ""id"": ""928e59b0-8ba6-4756-937b-6185d70626b5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""axis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d841b809-7b0a-4a20-9a1a-a39c4761ad76"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""26c7fedd-6fe1-4ec4-ad95-1bfbc66f811b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fcbf18ef-68f0-458d-a63a-5de2efc29ba1"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""abd9b21d-d885-4572-b6b8-11399ff964d5"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""axis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8d191d75-2a1f-4e3e-b7d1-021faa75a1dc"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a2bb25f5-8b0a-441b-be05-7f20b58913b3"",
                    ""path"": ""<Touchscreen>/touch*/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d6aa5de-5c2f-4a9d-a017-27a645a6326f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shootAlt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73e0a41a-6ee6-4d33-ad02-54aa1fdc0abf"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9013f07b-1cae-42ec-8ef2-b2022b2bff07"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""981ff6ff-9a4a-40b7-9700-2a89d5444395"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e215989e-7702-4f47-9d94-06be6977a86d"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d907378f-eb78-4c2e-a6a8-dbe95bc828b8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""start_practice"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ddd090f-2bd0-4559-9e98-0fb8a0d9f244"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""end_practice"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // game
        m_game = asset.FindActionMap("game", throwIfNotFound: true);
        m_game_axis = m_game.FindAction("axis", throwIfNotFound: true);
        m_game_shoot = m_game.FindAction("shoot", throwIfNotFound: true);
        m_game_shootAlt = m_game.FindAction("shootAlt", throwIfNotFound: true);
        m_game_restart = m_game.FindAction("restart", throwIfNotFound: true);
        m_game_start_practice = m_game.FindAction("start_practice", throwIfNotFound: true);
        m_game_end_practice = m_game.FindAction("end_practice", throwIfNotFound: true);
        m_game_mouse = m_game.FindAction("mouse", throwIfNotFound: true);
    }

    ~@PlayerControls()
    {
        UnityEngine.Debug.Assert(!m_game.enabled, "This will cause a leak and performance issues, PlayerControls.game.Disable() has not been called.");
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // game
    private readonly InputActionMap m_game;
    private List<IGameActions> m_GameActionsCallbackInterfaces = new List<IGameActions>();
    private readonly InputAction m_game_axis;
    private readonly InputAction m_game_shoot;
    private readonly InputAction m_game_shootAlt;
    private readonly InputAction m_game_restart;
    private readonly InputAction m_game_start_practice;
    private readonly InputAction m_game_end_practice;
    private readonly InputAction m_game_mouse;
    public struct GameActions
    {
        private @PlayerControls m_Wrapper;
        public GameActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @axis => m_Wrapper.m_game_axis;
        public InputAction @shoot => m_Wrapper.m_game_shoot;
        public InputAction @shootAlt => m_Wrapper.m_game_shootAlt;
        public InputAction @restart => m_Wrapper.m_game_restart;
        public InputAction @start_practice => m_Wrapper.m_game_start_practice;
        public InputAction @end_practice => m_Wrapper.m_game_end_practice;
        public InputAction @mouse => m_Wrapper.m_game_mouse;
        public InputActionMap Get() { return m_Wrapper.m_game; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
        public void AddCallbacks(IGameActions instance)
        {
            if (instance == null || m_Wrapper.m_GameActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameActionsCallbackInterfaces.Add(instance);
            @axis.started += instance.OnAxis;
            @axis.performed += instance.OnAxis;
            @axis.canceled += instance.OnAxis;
            @shoot.started += instance.OnShoot;
            @shoot.performed += instance.OnShoot;
            @shoot.canceled += instance.OnShoot;
            @shootAlt.started += instance.OnShootAlt;
            @shootAlt.performed += instance.OnShootAlt;
            @shootAlt.canceled += instance.OnShootAlt;
            @restart.started += instance.OnRestart;
            @restart.performed += instance.OnRestart;
            @restart.canceled += instance.OnRestart;
            @start_practice.started += instance.OnStart_practice;
            @start_practice.performed += instance.OnStart_practice;
            @start_practice.canceled += instance.OnStart_practice;
            @end_practice.started += instance.OnEnd_practice;
            @end_practice.performed += instance.OnEnd_practice;
            @end_practice.canceled += instance.OnEnd_practice;
            @mouse.started += instance.OnMouse;
            @mouse.performed += instance.OnMouse;
            @mouse.canceled += instance.OnMouse;
        }

        private void UnregisterCallbacks(IGameActions instance)
        {
            @axis.started -= instance.OnAxis;
            @axis.performed -= instance.OnAxis;
            @axis.canceled -= instance.OnAxis;
            @shoot.started -= instance.OnShoot;
            @shoot.performed -= instance.OnShoot;
            @shoot.canceled -= instance.OnShoot;
            @shootAlt.started -= instance.OnShootAlt;
            @shootAlt.performed -= instance.OnShootAlt;
            @shootAlt.canceled -= instance.OnShootAlt;
            @restart.started -= instance.OnRestart;
            @restart.performed -= instance.OnRestart;
            @restart.canceled -= instance.OnRestart;
            @start_practice.started -= instance.OnStart_practice;
            @start_practice.performed -= instance.OnStart_practice;
            @start_practice.canceled -= instance.OnStart_practice;
            @end_practice.started -= instance.OnEnd_practice;
            @end_practice.performed -= instance.OnEnd_practice;
            @end_practice.canceled -= instance.OnEnd_practice;
            @mouse.started -= instance.OnMouse;
            @mouse.performed -= instance.OnMouse;
            @mouse.canceled -= instance.OnMouse;
        }

        public void RemoveCallbacks(IGameActions instance)
        {
            if (m_Wrapper.m_GameActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameActions instance)
        {
            foreach (var item in m_Wrapper.m_GameActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameActions @game => new GameActions(this);
    public interface IGameActions
    {
        void OnAxis(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnShootAlt(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
        void OnStart_practice(InputAction.CallbackContext context);
        void OnEnd_practice(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
    }
}
