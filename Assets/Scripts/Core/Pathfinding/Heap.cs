using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Heap<T> where T : IHeapItem<T> {
	private T[] items_;
	private int currentItemCount_;

	public Heap(int maxHeapSize) {
		items_ = new T[maxHeapSize];
	}

	public void Add(T item) {
		item.heapIndex = currentItemCount_;
		items_[currentItemCount_] = item;
		SortUp(item);
		currentItemCount_++;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	private void SortUp(T item) {
		int parentIndex = (item.heapIndex - 1) / 2;

		while(true) {
			T parentItem = items_[parentIndex];
			if(item.CompareTo(parentItem) > 0) {
				Swap(item, parentItem);
			} else {
				break;
			}

			parentIndex = (item.heapIndex - 1) / 2;
		}
	}

	private void Swap(T item, T other) {
		items_[item.heapIndex] = other;
		items_[other.heapIndex] = item;
		int itemIndex = item.heapIndex;
		item.heapIndex = other.heapIndex;
		other.heapIndex = itemIndex;
	}

	public T RemoveFirst() {
		T firstItem = items_[0];
		currentItemCount_--;
		items_[0] = items_[currentItemCount_];
		items_[0].heapIndex = 0;
		SortDown(items_[0]);
		return firstItem;
	}

	private void SortDown(T item) {
		while(true) {
			int childIndexLeft = item.heapIndex * 2 + 1;
			int childIndexRight = item.heapIndex * 2 + 2;
			int swapIndex = 0;

			if(childIndexLeft < currentItemCount_) {
				swapIndex = childIndexLeft;

				if(childIndexRight < currentItemCount_) {
					if(items_[childIndexLeft].CompareTo(items_[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if(item.CompareTo(items_[swapIndex]) < 0) {
					Swap(item, items_[swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}
		}
	}

	public bool Contains(T item) {
		return Equals(items_[item.heapIndex], item);
	}

	public int count {
		get {
			return currentItemCount_;
		}
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int heapIndex {
		get;
		set;
	}
}
