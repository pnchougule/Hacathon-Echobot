using AdaptiveCards;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System;
using System.Text;

namespace EchoBotDemo.Models
{
    public class AdaptiveCardGenerator
    {
      
        public async Task<string> GenerateHtmlTable<T>(List<dynamic> items)
        {
            if (items == null || items.Count == 0)
            {
                return "<p>No data available.</p>";
            }

            var sb = new StringBuilder();

            // Start the table
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");

            // Create table headers
            sb.AppendLine("<tr>");
            foreach (var key in ((IDictionary<string, object>)items[0]).Keys)
            {
                sb.AppendLine($"<th>{System.Web.HttpUtility.HtmlEncode(key)}</th>");
            }
            sb.AppendLine("</tr>");

            // Create table rows
            foreach (var item in items)
            {
                sb.AppendLine("<tr>");
                foreach (var key in ((IDictionary<string, object>)item).Keys)
                {
                    var value = ((IDictionary<string, object>)item)[key];
                    sb.AppendLine($"<td>{System.Web.HttpUtility.HtmlEncode(value?.ToString())}</td>");
                }
                sb.AppendLine("</tr>");
            }

            // End the table
            sb.AppendLine("</table>");

            return sb.ToString();

        }

    }
}
