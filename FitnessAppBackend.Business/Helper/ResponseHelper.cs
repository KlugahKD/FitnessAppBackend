using FitnessAppBackend.Business.Common;

namespace FitnessAppBackend.Business.Helper;

public static class ResponseHelper
{
    public static ServiceResponse<T> OkResponse<T>(T data = default!)
    {
        return new ServiceResponse<T>
        {
            Data = data,
            IsSuccessful = true,
            Message = "Operation successful",
            Code = 200,
            Errors = null
        };
    }
    
    public static ServiceResponse<T> CreatedResponse<T>(T data, string message = "Resource created successfully")
    {
        return new ServiceResponse<T>
        {
            Data = data,
            IsSuccessful = true,
            Message = message,
            Code = 201,
            Errors = null
        };
    }


    public static ServiceResponse<T> BadRequestResponse<T>(string message,  List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = false,
            Message = message,
            Code = 400,
            Errors = errors
        };
    }

    public static ServiceResponse<T> NotFoundResponse<T>(string message,  List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = false,
            Message = message,
            Code = 404,
            Errors = errors
        };
    }

    public static ServiceResponse<T> FailedDependencyResponse<T>(string message,  List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = false,
            Message = message,
            Code = 424,
            Errors = errors
        };
    }
    public static ServiceResponse<T> InternalServerErrorResponse<T>(string message,  List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = false,
            Message = message,
            Code = 500,
            Errors = errors
        };
    }
    
    public static ServiceResponse<T> ForbiddenResponse<T>(string message,  List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccessful = false,
            Message = message,
            Code = 403,
            Errors = errors
        };
    }
}