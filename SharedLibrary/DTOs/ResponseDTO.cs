using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.DTOs
{
    //serviste geri döneceğimiz response sınıfı başarılı ve başarısız durumu içeriyor.
    public class ResponseDTO<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }

        [JsonIgnore] //responseDTO json çevirilirken Issuccess ignore etsin clientlere göstermicez bu UI için gösterilebilir.
        public bool IsSuccess { get; private set; }

        public ErrorDTO Error { get; private set; }

        public static ResponseDTO<T> Success(int statusCode, T data)
        {
            return new ResponseDTO<T> { StatusCode = statusCode, Data = data, IsSuccess = true };
        }

        public static ResponseDTO<T> Success(int statusCode)
        {
            return new ResponseDTO<T> { StatusCode = statusCode, Data = default, IsSuccess = true };
        }

        public static ResponseDTO<T> Fail(int statusCode, ErrorDTO errorDTO)
        {
            return new ResponseDTO<T> { StatusCode = statusCode, Error = errorDTO, IsSuccess = false };
        }

        public static ResponseDTO<T> Fail(int statusCode, string errorMessage, bool isShow)
        {
            var error = new ErrorDTO(errorMessage, isShow);  //diğer metodda is show zaten  ErrorDTO içinde gönderilecek
            return new ResponseDTO<T> { StatusCode = statusCode, Error = error, IsSuccess = false };
        }
    }
}