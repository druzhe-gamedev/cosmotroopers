using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Application.Common.UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        public abstract UniTask Show(CancellationToken cancellationToken);
        public abstract UniTask Hide(CancellationToken cancellationToken);
    }
}