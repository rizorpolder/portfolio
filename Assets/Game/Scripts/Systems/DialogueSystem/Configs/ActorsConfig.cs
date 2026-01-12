using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.DialogueSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ActorsConfig", menuName = "Project/Dialogue System/Actors Config")]
public class ActorsConfig : ScriptableObject
{
	[SerializeField] public List<ActorConfig> Actors;

	
}