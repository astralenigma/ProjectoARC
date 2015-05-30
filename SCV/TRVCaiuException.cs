using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCV
{
    [Serializable]
    public class TRVCaiuException : Exception
    {
        public TRVCaiuException() { }
        public TRVCaiuException(string message) : base(message) { }
        public TRVCaiuException(string message, Exception inner) : base(message, inner) { }
        protected TRVCaiuException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
