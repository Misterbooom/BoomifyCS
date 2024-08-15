using System;
using System.Collections.Generic;
using System.Text;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
namespace BoomifyCS.Objects
{
    public class BifyArray(List<BifyObject> bifyObjects) : BifyObject(new Token(TokenType.ARRAY, "array"))
    {
        private readonly List<BifyObject> _bifyObjects = bifyObjects ?? throw new ArgumentNullException(nameof(bifyObjects));

        public BifyObject Get(int index)
        {
            if (index < 0 || index >= _bifyObjects.Count)
            {
                throw new BifyIndexError("Index is out of range.");
            }
            return _bifyObjects[index];
        }

        public void Append(BifyObject item)
        {
            if (item == null)
            {
                throw new BifyNullError(nameof(item));
            }
            _bifyObjects.Add(item);
        }

        public int Count => _bifyObjects.Count;

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
        public override BifyString Repr()
        {
            return new BifyString($"BifyArray({ToString()})");
        }

    }
}
