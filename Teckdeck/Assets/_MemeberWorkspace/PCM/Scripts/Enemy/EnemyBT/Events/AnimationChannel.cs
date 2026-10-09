using AnimatorSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimationChannel", message: "Play [Hash]", category: "Event/Param", id: "c3be3fc262ddbad7b460bc5248f8721d")]
public sealed partial class AnimationChannel : EventChannel<HashDataSO> { }

