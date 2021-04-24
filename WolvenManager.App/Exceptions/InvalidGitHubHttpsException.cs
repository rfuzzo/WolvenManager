using System;

namespace WolvenManager.App.Exceptions
{
    public class InvalidGitHubHttpsException : Exception
    {
        public InvalidGitHubHttpsException()
        {
            
        }

        public InvalidGitHubHttpsException(string message) : base(message)
        {
            
        }
    }
}
