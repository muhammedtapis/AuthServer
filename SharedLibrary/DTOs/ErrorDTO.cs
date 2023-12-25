using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTOs
{
    //tüm apiler kullancak bu sınıfı o yüzden sharedLibrary de oluşturduk.
    public class ErrorDTO
    {
        //bu propertyleri set etmek için mutlaka constructor kullanılmalı.
        public List<string> Errors { get; private set; } = new List<string>();

        public bool IsShow { get; private set; } //hatayı gösterip göstermemek için

        public ErrorDTO(string error, bool isShow)
        {
            Errors.Add(error);
            IsShow = isShow;
        }

        public ErrorDTO(List<string> errors, bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }
    }
}