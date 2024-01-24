namespace Exercise8
{
    /// <summary>
    /// Generic ListNode class - avoiding boxing unboxing here by using generic implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListNode<T> 
    {
        private T _item;
        /// <summary>
        /// Property to hold pointer to next ListNode - Self containing object
        /// </summary>

        public ListNode<T> Next { get; set; }

        /// <summary>
        /// Property to hold value into the Node
        /// </summary>

        public T Item
        {
            get => _item;
            set => _item = value;
        }

        /// <summary>
        /// Constructor with item and the next node specified
        /// </summary>
        /// <param name="item"></param>
        /// <param name="next"></param>

        public ListNode(T item, ListNode<T> next = null)
        {
            this._item = item;
            this.Next = next;
        }

        /// <summary>
        /// Overriding ToString to return a string value for the item in the node
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return _item == null ? string.Empty : _item.ToString();
        }
    }
}
