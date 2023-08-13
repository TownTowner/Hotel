using System.Collections.Generic;

namespace Hotel.Configuration;

public class BaseResult
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "success";
    public List<string> Errors { get; set; }

    public virtual BaseResult Fail(string message = "")
    {
        Success = false;
        if (string.IsNullOrEmpty(message))
            Message = message;
        return this;
    }

    public virtual BaseResult FailAndError(string message = "")
    {
        Success = false;
        if (string.IsNullOrEmpty(message))
            Message = message;
        AddErrors(message);
        return this;
    }

    public virtual BaseResult Successed(string message = "")
    {
        Success = true;
        if (string.IsNullOrEmpty(message))
            Message = message;
        return this;
    }

    public virtual BaseResult AddErrors(string error)
    {
        if (string.IsNullOrEmpty(error))
            return this;

        Errors = Errors ?? new List<string>();
        Errors.Add(error);
        return this;
    }

    public virtual BaseResult AddErrors(List<string> errors)
    {
        if (errors == null || errors.Count == 0)
            return this;

        Errors = Errors ?? new List<string>();
        Errors.AddRange(errors);
        return this;
    }
}
