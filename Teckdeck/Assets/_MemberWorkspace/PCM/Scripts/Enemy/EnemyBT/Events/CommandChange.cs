using Enemy.BT;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Enemy Command Change")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Enemy Command Change", message: "Change [Command]", category: "Events", id: "d5e051b6c65a50b4fedc1d3a65efdd17")]
public sealed partial class CommandChange : EventChannel<StateCommands> { }

