﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamariners.RestClient.Interfaces;

namespace Xamariners.RestClient.Helpers
{
    public static class ServiceResponseExtensions
    {
        /// <summary>
        /// The as service response.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <typeparam name="TReturnType">
        /// </typeparam>
        /// <returns>
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public static ServiceResponse<TReturnType> AsServiceResponse<TReturnType>(this TReturnType data, int start = 0, int amount = 1, DateTime? processingTimestamp = null, long totalCount = 0, string message = null, string errorMessage = null)
        {
            return new ServiceResponse<TReturnType>
            {
                Data = data,
                Start = start,
                Amount = data == null ? 0 : amount,
                RequestDateTime = DateTime.UtcNow,
                ProcessingTimestamp = processingTimestamp,
                TotalCount = totalCount,
                Message = message,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// The as service response.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <typeparam name="TReturnType">
        /// </typeparam>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public static ServiceResponse<TReturnType> ToServiceResponse<TOriginalType, TReturnType>(
            this ServiceResponse<TOriginalType> originalServiceResponse, string message = "")
        {
            return new ServiceResponse<TReturnType>
            {
                RequestDateTime = DateTime.UtcNow,
                Status = originalServiceResponse.Status,
                ErrorMessage = originalServiceResponse.ErrorMessage,
                Message = string.IsNullOrEmpty(message) ? originalServiceResponse.Message : message,
                ElapsedTime = originalServiceResponse.ElapsedTime,
                Errors = originalServiceResponse.Errors,
                ServiceErrorType = originalServiceResponse.ServiceErrorType,
            };
        }

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetData<T>(this ServiceResponse<T> response, T defaultValue = default(T))
        {
            if (response == null)
            {
                LogServiceException("Null response received from server");
                return defaultValue;
            }

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                string message = string.IsNullOrEmpty(response.Message) ? "Error" : response.Message;
                LogServiceException($"error for type {typeof(T).Name} : {message} - {response.ErrorMessage}");
                return defaultValue;
            }

            return response.Data;
        }


        /// <summary>
        /// The log service exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        private static void LogServiceException(Exception exception)
        {
            // TODO: Log the exception
            throw exception;
        }

        /// <summary>
        /// The log service exception.
        /// </summary>
        /// <param name="exceptionMessage">
        /// The exception message.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        private static void LogServiceException(string exceptionMessage)
        {
            // TODO: Log the exception
            throw new Exception(exceptionMessage);
        }

        /// <summary>
        /// The cast list.
        /// </summary>
        /// <param name="listType">
        /// The cast type.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IServiceResponse CastListAsServiceResponse(Type listType, IEnumerable list)
        {
            var itemType = listType.GenericTypeArguments.First();
            var typedList = CastList(itemType, list);

            var serviceResponselistType = typeof(ServiceResponse<>).MakeGenericType(listType);

            var response = (IServiceResponse)Activator.CreateInstance(serviceResponselistType);

            response.SetData(typedList);
            response.Amount = typedList.Count;

            return response;
        }

        /// <summary>
        /// The cast list.
        /// </summary>
        /// <param name="itemType">
        /// The cast type.
        /// </param>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList CastList(Type itemType, IEnumerable enumerable)
        {
            Type listType = typeof(List<>).MakeGenericType(itemType);
            var list = (IList)Activator.CreateInstance(listType);
            foreach (object item in enumerable)
            {
                list.Add(item);
            }

            return list;
        }

    }
}
