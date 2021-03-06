﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BotBits.Events;
using BotBits.SendMessages;
using JetBrains.Annotations;
using Nito;

namespace BotBits
{
    public class MessageQueue<T> : IMessageQueue where T : SendMessage<T>
    {
        private readonly ManualResetEvent _finishEvent = new ManualResetEvent(true);
        private readonly Deque<T> _queue = new Deque<T>();
        private double _lastTicks;

        public int Count
        {
            get
            {
                lock (this._queue)
                {
                    return this._queue.Count;
                }
            }
        }
  
        public double TicksPerMessage { get; set; } = 1;

        void IMessageQueue.SendTicks(long ticks, long maxTicks, BotBitsClient client)
        {
            if (this._lastTicks > ticks) return;
            this._lastTicks = ticks - (long)Math.Min(maxTicks, ticks - this._lastTicks);

            for (; this._lastTicks < ticks; this._lastTicks += this.TicksPerMessage)
            {
                var msg = this.Dequeue();
                if (msg == null)
                {
                    this._lastTicks = ticks;
                    break;
                }
                if (!this.SendMessage(msg, client))
                {
                    this._lastTicks--;
                    new SendCancelledEvent<T>(msg)
                        .RaiseIn(client);
                }
            }
        }

        [CanBeNull]
        private T Dequeue()
        {
            lock (this._queue)
            {
                if (this._queue.Count == 0) return null;

                var first = this._queue.RemoveFromFront();

                if (this._queue.Count == 0) this._finishEvent.Set();

                return first;
            }
        }

        internal void Enqueue(T sendMessage, BotBitsClient client)
        {
            new SendQueueEvent<T>(sendMessage)
                .RaiseIn(client);

            lock (this._queue)
            {
                if (sendMessage.SkipsQueue) this._queue.AddToFront(sendMessage);
                else this._queue.AddToBack(sendMessage);

                this._finishEvent.Reset();
            }
        }

        internal bool SendMessage(T msg, BotBitsClient client)
        {
            var e = new SendingEvent<T>(msg);
            e.RaiseIn(client);
            if (e.Cancelled) return false;


            var con = ConnectionManager.Of(client).Connection;
            if (con == null) return false;

            new SentEvent<T>(msg)
                .RaiseIn(client);
            e.Message.Send(con);
            return true;
        }

        public void ClearQueue()
        {
            lock (this._queue)
            {
                this._queue.Clear();
                this._finishEvent.Set();
            }
        }

        public Task FinishQueueAsync()
        {
            return this._finishEvent.AsTask();
        }
    }
}