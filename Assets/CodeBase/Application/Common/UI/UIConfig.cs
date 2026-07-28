using System;
using UnityEngine;

namespace CodeBase.Application.Common.UI
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "UI/UI Config")]
    public class UIConfig : ScriptableObject
    {
        [field: SerializeField] public HUD HUD { get; private set; }
        [SerializeField] private BaseWindow[] _windows;

        public T GetWindow<T>() where T : BaseWindow
        {
            foreach (BaseWindow t in _windows)
            {
                if (t is T window)
                    return window;
            }

            throw new NullReferenceException($"No window with type {typeof(T)} found");
        }
    }
}