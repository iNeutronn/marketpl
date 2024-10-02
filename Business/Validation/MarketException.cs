using System;

namespace Business.Validation
{
    public class MarketException : Exception
    {
      
        public MarketException(string message) : base(message)
        {
        }
        public MarketException(string message,  Exception innerExeption) : base(message,innerExeption)
        {
          
        }
    }
}
