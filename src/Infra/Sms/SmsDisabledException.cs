using System;
using System.Runtime.Serialization;

namespace MockExams.Infra.Sms;

[Serializable]
public class SmsDisabledException : Exception
{
    public SmsDisabledException(string message) : base(message)
    {
    }

    private SmsDisabledException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

