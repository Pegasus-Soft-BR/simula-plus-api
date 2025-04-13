using System;
using System.Runtime.Serialization;

namespace Infra.IA;

[Serializable]
public class IADisabledException : Exception
{
    public IADisabledException(string message) : base(message)
    {
    }

    private IADisabledException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

