using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Aurabox
{
    public class TransitionToSceneAfterTime : MonoBehaviour
    {
        [SerializeField, InspectorName("Time (in seconds) until scene transition after Start")]
        private float _timeUntilTransition;

        [SerializeField]
        private SceneSO _transitionTo = null!;
        
        [Inject]
        private readonly SceneManagementSystem _sceneManagementSystem = null!;

        private CancellationTokenSource? _tokenSource;
        
        // ReSharper disable once UnusedMember.Local
        private async UniTaskVoid Start()
        {
            _tokenSource = new CancellationTokenSource();
            await UniTask.Delay(TimeSpan.FromSeconds(_timeUntilTransition), cancellationToken: _tokenSource.Token);
            _tokenSource.Dispose();
            _tokenSource = null;
            
            _sceneManagementSystem.SetActiveScene(_transitionTo);
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }
    }
}
