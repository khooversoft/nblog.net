﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Tools;

namespace nBlog.Store.Services
{
    public class TelemetryBuffer
    {
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private readonly ActionBlock<string> _block;
        private readonly int _maxSize;

        public TelemetryBuffer(int maxSize = 1000)
        {
            maxSize.VerifyAssert(x => x > 0, $"{nameof(maxSize)} must be greater then 0");

            _maxSize = maxSize;
            _block = new ActionBlock<string>(Receiver);
        }

        public ITargetBlock<string> TargetBlock => _block;

        private void Receiver(string value)
        {
            _queue.Enqueue(value);

            while (_queue.Count > _maxSize) _queue.TryDequeue(out string _);
        }

        public IReadOnlyList<string> GetFirst(int size = 100) => _queue.Take(size).ToList();
    }
}
