using System;
using System.Runtime.Serialization;

namespace ShareBook.Domain.Exceptions;

[Serializable]
public class AwsS3DisabledException : Exception
{
    public AwsS3DisabledException(string message) : base(message)
    {
    }

    private AwsS3DisabledException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

