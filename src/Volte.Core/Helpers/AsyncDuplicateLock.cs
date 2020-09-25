using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Volte.Core.Helpers
{
    // https://stackoverflow.com/a/31194647
    public sealed class AsyncDuplicateLock<T>
    {
        private sealed class RefCounted<TR>
        {
            public RefCounted([NotNull] TR value)
            {
                RefCount = 1;
                Value = value;
            }

            public int RefCount { get; set; }
            public TR Value { get; }
        }

        private readonly Dictionary<T, RefCounted<SemaphoreSlim>> _semaphoreSlims
            = new Dictionary<T, RefCounted<SemaphoreSlim>>();

        [NotNull]
        private SemaphoreSlim GetOrCreate(T key)
        {
            RefCounted<SemaphoreSlim> item;
            lock (_semaphoreSlims)
            {
                if (_semaphoreSlims.TryGetValue(key, out item))
                {
                    ++item.RefCount;
                }
                else
                {
                    item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    _semaphoreSlims[key] = item;
                }
            }

            return item.Value;
        }

        public IDisposable Lock(T key)
        {
            GetOrCreate(key).Wait();
            return new Releaser(key, _semaphoreSlims);
        }

        public async Task<IDisposable> LockAsync(T key)
        {
            await GetOrCreate(key).WaitAsync().ConfigureAwait(false);
            return new Releaser(key, _semaphoreSlims);
        }

        private readonly struct Releaser : IDisposable
        {
            private readonly Dictionary<T, RefCounted<SemaphoreSlim>> _semaphoreSlims;

            public T Key { get; }

            public Releaser(T key, Dictionary<T, RefCounted<SemaphoreSlim>> semaphoreSlims)
            {
                _semaphoreSlims = semaphoreSlims;
                Key = key;
            }

            public void Dispose()
            {
                RefCounted<SemaphoreSlim> item;
                lock (_semaphoreSlims)
                {
                    item = _semaphoreSlims[Key];
                    --item.RefCount;
                    if (item.RefCount == 0)
                        _semaphoreSlims.Remove(Key);
                }

                item.Value.Release();
            }
        }
    }
}