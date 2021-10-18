using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace GFrame
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _sceneLoader;

        public static SceneLoader Get()
        {
            if (_sceneLoader != null) return _sceneLoader;
            GameObject root = new GameObject("SceneLoader");
            _sceneLoader = root.AddComponent<SceneLoader>();
            return _sceneLoader;
        }

        public static void DestroyLoader()
        {
            if (_sceneLoader != null)
                Object.Destroy(_sceneLoader.gameObject);
            _sceneLoader = null;
        }


        private readonly Dictionary<AsyncOperation, SceneLoadData> _asyncLoadData =
            new Dictionary<AsyncOperation, SceneLoadData>();

        private readonly List<string> _loadingSceneList = new List<string>();
        private readonly List<SceneLoadData> _waitLoadList = new List<SceneLoadData>();
        private const int MaxLoadNum = 4;
        private bool _isLoadingSceneAsync;

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName, Action onLoadDone, Action<float> onLoading,
            LoadSceneMode mode = LoadSceneMode.Single, bool allowSceneActivation = true)
        {
            InternalLoadSceneAsync(sceneName, onLoadDone, onLoading, mode, allowSceneActivation);
        }

        public void LoadSceneAsync(string sceneName, Action onLoadDone,
            LoadSceneMode mode = LoadSceneMode.Single, bool allowSceneActivation = true)
        {
            InternalLoadSceneAsync(sceneName, onLoadDone, null, mode, allowSceneActivation);
        }

        private void InternalLoadSceneAsync(string sceneName, Action onLoadDone, Action<float> onLoading,
            LoadSceneMode mode = LoadSceneMode.Single, bool allowSceneActivation = true)
        {
            if (_loadingSceneList.Contains(sceneName)) return;
            SceneLoadData data = new SceneLoadData(sceneName, mode, allowSceneActivation, onLoadDone, onLoading);
            if (_loadingSceneList.Count >= MaxLoadNum)
            {
                _waitLoadList.Add(data);
                return;
            }

            StartLoadSceneAsync(data);
        }

        private void StartLoadSceneAsync(SceneLoadData data)
        {
            _loadingSceneList.Add(data.Name);
            data.LoadSceneAsync(OnLoadSceneCompleted);
            _asyncLoadData.Add(data.Operation, data);
            if (!_isLoadingSceneAsync)
                StartCoroutine(OnLoadingSceneAsync());
        }

        private IEnumerator OnLoadingSceneAsync()
        {
            _isLoadingSceneAsync = true;
            while (_isLoadingSceneAsync)
            {
                _isLoadingSceneAsync = _asyncLoadData.Count > 0;
                foreach (var kv in _asyncLoadData)
                    kv.Value.UpdateProgress(kv.Key.progress);
                yield return null;
            }
        }

        private void OnLoadSceneCompleted(AsyncOperation handle)
        {
            if (!_asyncLoadData.TryGetValue(handle, out var data))
            {
                GameLog.LogError("加载场景异常");
                return;
            }

            _loadingSceneList.Remove(data.Name);
            _asyncLoadData.Remove(handle);
            data.OnLoadSceneComplete();

            int waitLoadNum = _waitLoadList.Count;
            if (waitLoadNum == 0 || waitLoadNum >= MaxLoadNum) return;
            int max = MaxLoadNum - _loadingSceneList.Count;
            for (int i = 0; i < max; i++)
            {
                if (i >= _waitLoadList.Count) break;
                StartLoadSceneAsync(_waitLoadList[i]);
            }
        }


        private class SceneLoadData
        {
            private readonly string _name;
            private readonly LoadSceneMode _mode;
            private readonly bool _allowSceneActivation;
            private Action<float> _onLoading;
            private Action _onLoadDone;
            private AsyncOperation _operation;

            public string Name => _name;
            public AsyncOperation Operation => _operation;

            public SceneLoadData(string name, LoadSceneMode mode, bool allowSceneActivation,
                Action onLoadDone, Action<float> onLoading)
            {
                _name = name;
                _onLoading = onLoading;
                _onLoadDone = onLoadDone;
                _mode = mode;
                _allowSceneActivation = allowSceneActivation;
            }

            public void LoadSceneAsync(Action<AsyncOperation> onLoadSceneDone)
            {
                _operation = SceneManager.LoadSceneAsync(_name, _mode);
                _operation.allowSceneActivation = _allowSceneActivation;
                _operation.completed += onLoadSceneDone;
            }

            public void UpdateProgress(float percent)
            {
                _onLoading?.Invoke(percent);
                GameLog.Log($"load scene {Name} percent: {_operation.progress}");
            }

            public void OnLoadSceneComplete()
            {
                GameLog.Log($"load scene {Name} Complete");
                if (!_operation.allowSceneActivation)
                    _operation.allowSceneActivation = true;
                UpdateProgress(1.0f);
                _onLoadDone?.Invoke();
                _operation = null;
                _onLoading = null;
                _onLoadDone = null;
            }
        }
    }
}