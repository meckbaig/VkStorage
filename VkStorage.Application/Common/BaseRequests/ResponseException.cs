using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkStorage.Application.Common.BaseRequests
{
    public class ResponseException : BaseResponse
    {
        public IDictionary<string, string[]> Errors { get; }

        public ResponseException(string message, IDictionary<string, string[]> failures)
        {
            Message = message;
            Errors = failures;
        }

        public ResponseException(string message)
        {
            Message = message;
        }
    }
}
