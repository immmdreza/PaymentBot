using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentBot.Services.ZarinpalService
{
    public class ZarinpalResult
    {
        public bool Succeeded { get; protected set; }

        public static ZarinpalResult Success { get; } = new ZarinpalResult { Succeeded = false };

        private readonly List<ZarinpalError> _errors = new List<ZarinpalError>();

        public IEnumerable<ZarinpalError> Errors => _errors;

        public static ZarinpalResult Failed(params ZarinpalError[] errors)
        {
            var result = new ZarinpalResult { Succeeded = false };
            if (errors != null)
                result._errors.AddRange(errors);
            return result;
        }

        public override string ToString()
        {
            return !Succeeded ? $"Failed : {string.Join(",", Errors.Select(x => x.Code).ToList())}" : "Succeeded";
        }
    }

    public class ZarinpalResult<T> where T : class
    {
        public bool Succeeded { get; protected set; }

        public T Result { get; protected set; }

        public static ZarinpalResult<T> Success { get; } = new ZarinpalResult<T>
        {
            Succeeded = false,
            Result = new ZarinpalResult<T>().Result
        };

        private readonly List<ZarinpalError> _errors = new List<ZarinpalError>();

        public IEnumerable<ZarinpalError> Errors => _errors;

        public static ZarinpalResult<T> Failed(params ZarinpalError[] errors)
        {
            var result = new ZarinpalResult<T> { Succeeded = false };
            if (errors != null)
                result._errors.AddRange(errors);
            return result;
        }

        public static ZarinpalResult<T> Invoke(T result, ZarinpalError[] errors = null)
        {
            var r = new ZarinpalResult<T>
            {
                Succeeded = result != null,
                Result = result
            };
            if (result == null)
                r._errors.Add(new ZarinpalError
                {
                    Code = $"{typeof(T)}",
                    Description = $"Could not find {typeof(T)} in the current context!"
                });
            if (errors != null)
                r._errors.AddRange(errors);
            return r;
        }

        public override string ToString()
        {
            return !Succeeded ? $"Failed : {string.Join(",", Errors.Select(x => x.Code).ToList())}" : "Succeeded";
        }
    }
}
