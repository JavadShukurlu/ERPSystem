using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Common
{
    public class ResultDto<T>
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public static ResultDto<T> Success(T data, string? message = null)
        {
            return new ResultDto<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ResultDto<T> Failure(string message)
        {
            return new ResultDto<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            };
        }
    }
}
