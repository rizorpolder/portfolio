using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Systems.Character.Data
{
	public class CharacterPosition
	{
		public Queue<Vector3> targets;
		public bool instantly;
		public Action<MoveCompleteStatus> action;
	}
}