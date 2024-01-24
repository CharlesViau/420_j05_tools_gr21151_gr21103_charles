using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise8
{
	/// <summary>
	/// SinglyLinkedList class for generic implementation of LinkedList. 
	/// Again, avoiding boxing unboxing here and using ICollection interface members. 
	/// Believe this can be useful when applying other 
	/// operations such as sorting, searching etc. 
	/// </summary>
	/// <typeparam name="T"></typeparam>

	public class SinglyLinkedList<T> : ICollection<T>
	{
		#region private variables

		private ICollection<T> _collectionImplementation;

		#endregion

		/// <summary>
		/// Property to hold first node in the list
		/// </summary>

		public ListNode<T> FirstNode { get; private set; }

		/// <summary>
		/// Property to hold last node in the list
		/// </summary>

		public ListNode<T> LastNode { get; private set; }

		/// <summary>
		/// Indexer to iterate through the list and fetch the item
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>

		public T this[int index]
		{
			get
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException();
				var currentNode = FirstNode;
				for (var i = 0; i < index; i++)
				{
					currentNode = currentNode.Next ?? throw new ArgumentOutOfRangeException();
				}

				return currentNode.Item;
			}
		}

		/// <summary>
		/// Property to hold count of items in the list
		/// </summary>

		public int Count { get; private set; }

		public bool IsReadOnly { get; }

		/// <summary>
		/// Property to determine if the list is empty or contains any item
		/// </summary>

		public bool IsEmpty
		{
			get
			{
				lock (this)
				{
					return FirstNode == null;
				}
			}
		}

		/// <summary>
		/// Constructor initializing list with a provided list name
		/// </summary>
		/// <param name="strListName"></param>

		public SinglyLinkedList(string strListName, bool isReadOnly, ICollection<T> collectionImplementation)
		{
			IsReadOnly = isReadOnly;
			_collectionImplementation = collectionImplementation;
			Count = 0;
			FirstNode = LastNode = null;
		}

		/// <summary>
		/// default constructor initializing list with a default name 'MyList'
		/// </summary>

		public SinglyLinkedList(bool isReadOnly, ICollection<T> collectionImplementation) : this("MyList", isReadOnly, collectionImplementation)
		{
		}

		/// <summary>
		/// Operation ToString overridden to get the contents from the list
		/// </summary>
		/// <returns></returns>

		public override string ToString()
		{
			if (IsEmpty)
				return string.Empty;
			var returnString = new StringBuilder();
			foreach (var item in this)
			{
				if (returnString.Length > 0)
					returnString.Append("->");
				returnString.Append(item);
			}

			return returnString.ToString();
		}

		/// <summary>
		/// Operation inserts item at the front of the list
		/// </summary>
		/// <param name="item"></param>

		public void InsertAtFront(T item)
		{
			lock (this)
			{
				if (IsEmpty)
					FirstNode = LastNode = new ListNode<T>(item);
				else
					FirstNode = new ListNode<T>(item, FirstNode);
				Count++;
			}
		}

		/// <summary>
		/// Operation inserts item at the back of the list
		/// </summary>
		/// <param name="item"></param>

		public void InsertAtBack(T item)
		{
			lock (this)
			{
				if (IsEmpty)
					FirstNode = LastNode = new ListNode<T>(item);
				else
					LastNode = LastNode.Next = new ListNode<T>(item);
				Count++;
			}
		}

		/// <summary>
		/// Operation removes item from the front of the list
		/// </summary>
		/// <returns></returns>

		public object RemoveFromFront()
		{
			lock (this)
			{
				if (IsEmpty)
					throw new ApplicationException("list is empty");
				object removedData = FirstNode.Item;
				if (FirstNode == LastNode)
					FirstNode = LastNode = null;
				else
					FirstNode = FirstNode.Next;
				Count--;
				return removedData;
			}
		}

		/// <summary>
		/// Operation removes item from the back of the list
		/// </summary>
		/// <returns></returns>

		public object RemoveFromBack()
		{
			lock (this)
			{
				if (IsEmpty)
					throw new ApplicationException("list is empty");
				object removedData = LastNode.Item;
				if (FirstNode == LastNode)
					FirstNode = LastNode = null;
				else
				{
					var currentNode = FirstNode;
					while (currentNode.Next != LastNode)
						currentNode = currentNode.Next;
					LastNode = currentNode;
					currentNode.Next = null;
				}

				Count--;
				return removedData;
			}
		}

		/// <summary>
		/// Operation inserts item at the specified index in the list
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>

		public void InsertAt(int index, T item)
		{
			lock (this)
			{
				if (index > Count || index < 0)
					throw new ArgumentOutOfRangeException();
				if (index == 0)
					InsertAtFront(item);
				else if (index == (Count - 1))
					InsertAtBack(item);
				else
				{
					var currentNode = FirstNode;
					for (var i = 0; i < index - 1; i++)
					{
						currentNode = currentNode.Next;
					}

					var newNode = new ListNode<T>(item, currentNode.Next);
					currentNode.Next = newNode;
					Count++;
				}
			}
		}

		/// <summary>
		/// Operation removes item from the specified index in the list
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>

		public object RemoveAt(int index)
		{
			lock (this)
			{
				if (index > Count || index < 0)
					throw new ArgumentOutOfRangeException();
				object removedData;
				if (index == 0)
					removedData = RemoveFromFront();
				else if (index == (Count - 1))
					removedData = RemoveFromBack();
				else
				{
					var currentNode = FirstNode;
					for (var i = 0; i < index; i++)
					{
						currentNode = currentNode.Next;
					}

					removedData = currentNode.Item;
					currentNode.Next = currentNode.Next.Next;
					Count--;
				}

				return removedData;
			}
		}


		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the input item if exists and returns true else false
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>

		public bool Remove(T item)
		{
			if (item != null && FirstNode.Item.ToString().Equals(item.ToString()))
			{
				RemoveFromFront();
				return true;
			}
			else if (item != null && LastNode.Item.ToString().Equals(item.ToString()))
			{
				RemoveFromBack();
				return true;
			}
			else
			{
				var currentNode = FirstNode;
				while (currentNode.Next != null)
				{
					if (currentNode.Next.Item.ToString().Equals(item.ToString()))
					{
						currentNode.Next = currentNode.Next.Next;
						Count--;
						if (currentNode.Next == null)
							LastNode = currentNode;
						return true;
					}

					currentNode = currentNode.Next;
				}
			}

			return false;
		}


		/// <summary>
		/// Operation updates an item provided as an input with a new item 
		/// (also provided as an input)
		/// </summary>
		/// <param name="oldItem"></param>
		/// <param name="newItem"></param>
		/// <returns></returns>

		public bool Update(T oldItem, T newItem)
		{
			lock (this)
			{
				var currentNode = FirstNode;
				while (currentNode != null)
				{
					if (currentNode.ToString().Equals(oldItem.ToString()))
					{
						currentNode.Item = newItem;
						return true;
					}

					currentNode = currentNode.Next;
				}

				return false;
			}
		}

		/// <summary>
		/// Returns true if list contains the input item else false
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>

		public bool Contains(T item)
		{
			lock (this)
			{
				var currentNode = FirstNode;
				while (currentNode != null)
				{
					if (item != null && currentNode.Item.ToString().Equals(item.ToString()))
					{
						return true;
					}

					currentNode = currentNode.Next;
				}

				return false;
			}
		}

		public void Add(T item)
		{
			InsertAtBack(item);
		}

		/// <summary>
		/// Operation resets the list and clears all its contents
		/// </summary>

		public void Clear()
		{
			FirstNode = LastNode = null;
			Count = 0;
		}
		
		/// <summary>
		/// Operation to reverse the contents of the linked list 
		/// by resetting the pointers and swapping the contents
		/// </summary>

		public void Reverse()
		{
			if (FirstNode == null || FirstNode.Next == null)
				return;
			LastNode = FirstNode;
			ListNode<T> prevNode = null;
			var currentNode = FirstNode;
			var nextNode = FirstNode.Next;

			while (currentNode != null)
			{
				currentNode.Next = prevNode;
				if (nextNode == null)
					break;
				prevNode = currentNode;
				currentNode = nextNode;
				nextNode = nextNode.Next;
			}
			FirstNode = currentNode;
		}

		/// <summary>
		/// Operation to find if the linked list contains a circular loop
		/// </summary>
		/// <returns></returns>

		public bool HasCycle()
		{
			var currentNode = FirstNode;
			var iteratorNode = FirstNode;
			for (; iteratorNode is {Next: { }}; 
			     iteratorNode = iteratorNode.Next )
			{
				if (currentNode.Next?.Next == null) return false;
				if (currentNode.Next == iteratorNode || 
				    currentNode.Next.Next == iteratorNode) return true;
				currentNode = currentNode.Next.Next;
			}
			return false;
		}

		/// <summary>
		/// Operation to find the midpoint of a list 
		/// </summary>
		/// <returns></returns>

		public ListNode<T> GetMiddleItem()
		{
			var currentNode = FirstNode;
			var iteratorNode = FirstNode;
			for (; iteratorNode is {Next: { }}; 
			     iteratorNode = iteratorNode.Next)
			{
				if (currentNode.Next?.Next == null) return iteratorNode;
				if (currentNode.Next == iteratorNode || 
				    currentNode.Next.Next == iteratorNode) return null;
				currentNode = currentNode.Next.Next;
			}
			return FirstNode;
		}
		
		#region IEnumerable<T> Members
		/// <summary>
		/// Custom GetEnumerator method to traverse through the list and 
		/// yield the current value
		/// </summary>
		/// <returns></returns>

		public IEnumerator<T> GetEnumerator()
		{
			var currentNode = FirstNode;
			while (currentNode != null)
			{
				yield return currentNode.Item;
				currentNode = currentNode.Next;
			}
		}

		#endregion

		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		
		/// <summary>
		/// Operation creates a circular loop in the linked list for testing purpose. 
		/// Once this loop is created, other operations would probably fail.
		/// </summary>

		public void CreateCycleInListToTest()
		{
			var currentNode = FirstNode;
			var midNode = GetMiddleItem();
			LastNode.Next = midNode;
		}
	}
	
}