using Microsoft.AspNetCore.Html;

namespace ClearAcceptDemo.Models
{
    public class ProcessViewModel
    {
        public HtmlString Result { get; set; }
        public HtmlString Request { get; set; }
        public HtmlString Response { get; set; }
       
    }
}
