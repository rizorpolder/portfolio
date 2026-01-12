using System;
using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.RestApi;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Network
{
	public class RequestsFactory
	{
		private readonly DiContainer _container;

		private string _cookie;
		public string Cookie => _cookie;

		public RequestsFactory(DiContainer container)
		{
			_container = container;
		}

		public TRequest CreateRequest<TRequest, TRequestData>(TRequestData requestData) where TRequestData : new()
		{
			var result = (TRequest) Activator.CreateInstance(typeof(TRequest), requestData);
			_container.Inject(result);
			return result;
		}

		public void UpdateCookie(string cookie, bool needSave = false)
		{
			_cookie = cookie;
			PlayerPrefs.SetString("Cookie", cookie);
		}
	}
}