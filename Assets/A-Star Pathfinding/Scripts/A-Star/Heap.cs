using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	public class Heap<T> where T : IHeapable<T> {

		T[] items;                  //The item arraylist
		int currentItemCount = 0;   //How many items their are in the list


		public Heap(int maxHeapSize) {
			items = new T[maxHeapSize];
		}


		//Adds a new item to the list and sorts it properly
		public void Add(T item) {
			//Adds the item to the end of the list
			items[currentItemCount] = item;
			item.heapIndex = currentItemCount;//SWITCHED ^^^

			//Sorts the items
			SortUp(item);
			//Size must be increased to match total size
			currentItemCount++;
		}

		//Removes the first item from the list and sorts it properly, returns the item removed
		public T RemoveFirst() {
			//Saves the first item to return later
			T firstItem = items[0];
			//Size must be shrunk down to size
			currentItemCount--;

			//Take the last item in the list and move it to the front
			items[0] = items[currentItemCount];
			items[0].heapIndex = 0;

			//Sorts the front number properly
			SortDown(items[0]);

			//Return the first item removed
			return firstItem;
		}

		//Sorts the item passed in upwards
		private void SortUp(T item) {
			int parentIndex = (item.heapIndex - 1) / 2; //parent index = (n - 1) / 2

			while(true) {
				T parentItem = items[parentIndex];
				if(item.CompareTo(parentItem) > 0) {
					Swap(item, parentItem);
				} else {
					break;
				}

				parentIndex = (item.heapIndex - 1) / 2;
			}
		}

		//Sorts the item passed in downwards
		private void SortDown(T item) {
			while(true) {
				int childIndexLeft = (item.heapIndex * 2) + 1;  //left child index = 2 * n + 1
				int childIndexRight = (item.heapIndex * 2) + 2; //right child index = 2 * n + 2
				int swapIndex = 0;

				if(childIndexLeft < currentItemCount) {
					swapIndex = childIndexLeft;

					if(childIndexRight < currentItemCount) {
						if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
							swapIndex = childIndexRight;
						}
					}

					if(item.CompareTo(items[swapIndex]) < 0) {
						Swap(item, items[swapIndex]);
					} else {
						return;
					}

				} else {
					return;
				}
			}
		}

		//Swaps the two items
		private void Swap(T itemA, T itemB) {
			items[itemA.heapIndex] = itemB;
			items[itemB.heapIndex] = itemA;

			int itemAIndex = itemA.heapIndex;
			itemA.heapIndex = itemB.heapIndex;
			itemB.heapIndex = itemAIndex;
		}

		//Updates the item (SortsUp)
		public void UpdateItem(T item) {
			SortUp(item);
		}


		public bool Contains(T item) {
			return Equals(items[item.heapIndex], item);
		}

		public int Count() {
			return currentItemCount;
		}



	}

	//Genaric T must implement IHeapItem
	public interface IHeapable<T> : IComparable<T> {
		int heapIndex {
			get;
			set;
		}
	}

}
