using Hotel.Configuration;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hotel.Models.Responses;

public class CommonResponse : CommonResult
{
    public static CommonResponse Create(object data)
    {
        return new CommonResponse()
        {
            Data = data
        };
    }

    public CommonResponse Fill(object data)
    {
        Data = data;
        return this;
    }

    public override CommonResponse Fail(string message = "")
    {
        return base.Fail(message) as CommonResponse;
    }

    public override CommonResponse FailAndError(string message = "")
    {
        return base.FailAndError(message) as CommonResponse;
    }

    public override CommonResponse Successed(string message = "")
    {
        return base.Successed(message) as CommonResponse;
    }

    public override CommonResponse AddErrors(string error)
    {
        return base.AddErrors(error) as CommonResponse;
    }

    public override CommonResponse AddErrors(List<string> errors)
    {
        return base.AddErrors(errors) as CommonResponse;
    }
}
