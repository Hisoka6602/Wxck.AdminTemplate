using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.Application {

    public class JsonResultDto : JsonResult {

        public JsonResultDto(object? value) : base(value) {
        }

        public JsonResultDto(object? value, object? serializerSettings) : base(value, serializerSettings) {
        }

        public static JsonResult Fail(string msg, int statusCode = 200) {
            return new JsonResult(new { Result = false, Msg = msg }) { StatusCode = statusCode };
        }

        public static JsonResult Fail(string msg, object data, int statusCode = 200) {
            return new JsonResult(new { Result = false, Data = data, Msg = msg }) { StatusCode = statusCode };
        }

        public static JsonResult Success(string msg) {
            return new JsonResult(new { Result = true, Msg = msg }) { StatusCode = 200 };
        }

        public static JsonResult Success(string msg, object data) {
            return new JsonResult(new { Result = true, Data = data, Msg = msg }) { StatusCode = 200 };
        }

        public static JsonResult Success(string msg, int total, object data) {
            return new JsonResult(new { Result = true, Data = data, Total = total, Msg = msg }) { StatusCode = 200 };
        }
    }
}