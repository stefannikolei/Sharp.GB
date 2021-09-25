using System;

namespace Sharp.GB.Gpu
{
    public class IntQueue
    {
        private readonly int[] array;

        private int size;

        private int offset = 0;

        public IntQueue(int capacity)
        {
            this.array = new int[capacity];
            this.size = 0;
            this.offset = 0;
        }

        public int Size()
        {
            return size;
        }

        public void enqueue(int value)
        {
            if (size == array.Length)
            {
                throw new ArgumentOutOfRangeException("Queue is full");
            }

            array[(offset + size) % array.Length] = value;
            size++;
        }

        public int dequeue()
        {
            if (size == 0)
            {
                throw new ArgumentNullException("Queue is empty");
            }

            size--;
            int value = array[offset++];
            if (offset == array.Length)
            {
                offset = 0;
            }

            return value;
        }

        public int get(int index)
        {
            if (index >= size)
            {
                throw new IndexOutOfRangeException();
            }

            return array[(offset + index) % array.Length];
        }

        public void set(int index, int value)
        {
            if (index >= size)
            {
                throw new IndexOutOfRangeException();
            }

            array[(offset + index) % array.Length] = value;
        }

        public void clear()
        {
            size = 0;
            offset = 0;
        }
    }
}
