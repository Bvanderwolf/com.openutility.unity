using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace OpenUtility.Samples.Data
{
    public class AddressableContentService : MockAppService
    {
        public void LoadContent(Action<DownloadStatus> callback)
        {
            LoadContentAsync(callback);
        }

        private async Awaitable LoadContentAsync(Action<DownloadStatus> callback)
        {
            byte totalBytes = (byte)Random.Range(5000, 20000);

            float time = Random.Range(5f, 15f);
            float current = 0f;

            while (current < time)
            {
                await Awaitable.NextFrameAsync();

                current += Time.deltaTime;

                float progress = Mathf.Clamp01(current / time);
                byte downloadedBytes = (byte)(totalBytes * progress);
                callback?.Invoke(new DownloadStatus
                {
                    TotalBytes = totalBytes,
                    DownloadedBytes = downloadedBytes,
                    IsDone = progress >= 1f
                });
            }
        }
    }
}
