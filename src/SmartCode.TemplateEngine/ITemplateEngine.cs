using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.TemplateEngine
{
    public interface ITemplateEngine : IPlugin
    {
        Task<String> Render(BuildContext context);
    }
}
