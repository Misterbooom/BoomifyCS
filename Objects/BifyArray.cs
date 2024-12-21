using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using System.Collections.Generic;
using System.Text;
using System;
using System.Numerics;

namespace BoomifyCS.Objects
{
    public class BifyArray : BifyObject
    {
        private readonly List<BifyObject> _bifyObjects;

        // Constructor
        public BifyArray(List<BifyObject> bifyObjects) : base() => _bifyObjects = bifyObjects ?? throw new ArgumentNullException(nameof(bifyObjects));

        // Get an element by index
        public BifyObject Get(int index)
        {
            if (index < 0 || index >= _bifyObjects.Count)
            {
                throw new BifyIndexError("Index is out of range.");
            }
            return _bifyObjects[index];
        }

        // Append an item to the array
        public void Append(BifyObject item)
        {
            if (item == null)
            {
                throw new BifyNullError(nameof(item));
            }
            _bifyObjects.Add(item);
        }

        // Get the count of objects in the array
        public BifyInteger Count => new(_bifyObjects.Count);

        // Convert the array to string
        public override BifyString ObjectToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < _bifyObjects.Count; i++)
            {
                sb.Append(_bifyObjects[i].ToString());
                if (i < _bifyObjects.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(']');
            return new BifyString(sb.ToString());
        }

        // Handle array indexing with support for integers and ranges
        public override BifyObject Index(BifyObject other)
        {

            if (other is BifyInteger bifyInteger)
            {
                if (_bifyObjects.Count == 0)
                {
                    throw new BifyNullError(ErrorMessage.ArrayIsEmpty());
                }

                

                int index = bifyInteger.Value < 0 ? _bifyObjects.Count + bifyInteger.Value : bifyInteger.Value;
                if (!IsInBound(index))
                {
                    throw new BifyIndexError(ErrorMessage.InvalidIndex(bifyInteger.Value, _bifyObjects.Count));
                }
                if (index < 0 || index >= _bifyObjects.Count)
                {
                    throw new BifyIndexError(ErrorMessage.InvalidIndex(bifyInteger.Value, _bifyObjects.Count));
                }

                return _bifyObjects[index];
            }

            else if (other is BifyRange bifyRange)
            {
                BifyInteger start = bifyRange.First;
                BifyInteger end = bifyRange.Second;

                int startIndex = start.Value < 0 ? _bifyObjects.Count + start.Value : start.Value;
                int endIndex = end.Value < 0 ? _bifyObjects.Count + end.Value  : end.Value + 1;

                if (!IsInBound(startIndex))
                {
                    throw new BifyIndexError(ErrorMessage.InvalidIndex(start.Value, _bifyObjects.Count));
                }

                if (!IsInBound(endIndex))
                {
                    throw new BifyIndexError(ErrorMessage.InvalidIndex(end.Value, _bifyObjects.Count));
                }
                return new BifyArray(_bifyObjects[startIndex..endIndex]);
            }


            return new BifyNull(); 
        }
        private bool IsInBound(int value)
        {
            return value < _bifyObjects.Count;
            
        }

        // String representation of the array
        public override BifyString Repr()
        {
            return new BifyString($"BifyArray({ToString()})");
        }

        // Override ToString to use ObjectToString
        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }

}
