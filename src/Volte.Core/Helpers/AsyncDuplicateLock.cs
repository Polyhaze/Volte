using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Volte.Core.Helpers
{
    // https://stackoverflow.com/a/31194647
    public sealed class AsyncDuplicateLock<T>
    {
        private sealed class RefCounted<TR>
        {
            public RefCounted(TR value)
            {
                RefCount = 1;
                Value = value;
            }

            public int RefCount { get; set; }
            public TR Value { get; }
        }

        private static readonly Dictionary<T, RefCounted<SemaphoreSlim>> SemaphoreSlims
            = new Dictionary<T, RefCounted<SemaphoreSlim>>();

        private static SemaphoreSlim GetOrCreate(T key)
        {
            RefCounted<SemaphoreSlim> item;
            lock (SemaphoreSlims)
            {
                if (SemaphoreSlims.TryGetValue(key, out item))
                {
                    ++item.RefCount;
                }
                else
                {
                    item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    SemaphoreSlims[key] = item;
                }
            }

            return item.Value;
        }

        public IDisposable Lock(T key)
        {
            GetOrCreate(key).Wait();
            return new Releaser(key);
        }

        public async Task<IDisposable> LockAsync(T key)
        {
            await GetOrCreate(key).WaitAsync().ConfigureAwait(false);
            return new Releaser(key);
        }

        private readonly struct Releaser : IDisposable
        {
            public T Key { get; }

            public Releaser(T key)
            {
                Key = key;
            }

            public void Dispose()
            {
                RefCounted<SemaphoreSlim> item;
                lock (SemaphoreSlims)
                {
                    item = SemaphoreSlims[Key];
                    --item.RefCount;
                    if (item.RefCount == 0)
                        SemaphoreSlims.Remove(Key);
                }

                item.Value.Release();
            }
        }
    }
}