namespace SupplyChain.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message=null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }


        public int StatusCode{set;get;}
        public string Message{set;get;}
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400=>"A Bad Request, you have made",
                401=>"Authorize ,you are not",
                404=>"Resource found, it was not",
                500=>"Error are pth to dark side.Errors lead to anger.Anger leads to Hate.Hate leads to carrier change",
                _=>null

            };
        }
    }
}