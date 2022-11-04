using System;
namespace Celemp
{
    public class CommandParseException: Exception
    {
        public CommandParseException(string message) : base(message)
        {
        }
    }
}

