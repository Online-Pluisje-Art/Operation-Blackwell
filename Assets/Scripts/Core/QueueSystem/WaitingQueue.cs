using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class WaitingQueue<T> where T : IQueueItem {
		private List<T> queue_;
		public int head { get; private set; }
		public int tail { get; private set; }

		public WaitingQueue() {
			queue_ = new List<T>();
			head = 0;
			tail = 0;
		}

		public void Enqueue(T item) {
			queue_.Add(item);
			tail++;
		}

		public void Dequeue() {
			queue_.RemoveAt(head);
		}

		public T Peek() {
			return queue_[head];
		}

		public List<T> GetQueue() {
			return queue_;
		}

		public void Clear() {
			queue_.Clear();
		}

		public int Count() {
			return queue_.Count;
		}

		public bool IsEmpty() {
			return queue_.Count == 0;
		}

		public bool Contains(T item) {
			return queue_.Contains(item);
		}

		public void Sort() {
			queue_.Sort((x , y) => x.GetInitiative().CompareTo(y.GetInitiative()));
		}
	}
}