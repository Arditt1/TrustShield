#pragma checksum "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "05a8d05d24c294afffd2d663f9aedc68ea12d4f5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Covers), @"mvc.1.0.view", @"/Views/Home/Covers.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\_ViewImports.cshtml"
using db_tsh;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\_ViewImports.cshtml"
using db_tsh.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"05a8d05d24c294afffd2d663f9aedc68ea12d4f5", @"/Views/Home/Covers.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"da91ff249365bb3ab707d27079c0cbae2e416585", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Covers : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<db_tsh.Models.Covers>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Covers", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
  
    ViewData["Title"] = "Create Cover";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 6 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "05a8d05d24c294afffd2d663f9aedc68ea12d4f54690", async() => {
                WriteLiteral(@"
    <div class=""form-group"">
        <label for=""cov_amount"">Cover Amount</label>
        <input type=""text"" class=""form-control"" id=""cov_amount"" name=""cov_amount"" required />
    </div>

    <div class=""form-group"">
        <label for=""cov_type"">Cover Type</label>
        <input type=""text"" class=""form-control"" id=""cov_type"" name=""cov_type"" required />
    </div>

    <div class=""form-group"">
        <label for=""package_code"">Package</label>
        <select id=""package_code"" name=""package_code"" class=""form-control"" required>
            ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "05a8d05d24c294afffd2d663f9aedc68ea12d4f55538", async() => {
                    WriteLiteral("Select Package");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_0.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 23 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
             foreach (var package in (List<SelectListItem>)ViewData["Packages"])
            {

#line default
#line hidden
#nullable disable
                WriteLiteral("                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "05a8d05d24c294afffd2d663f9aedc68ea12d4f57077", async() => {
#nullable restore
#line 25 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
                                          Write(package.Text);

#line default
#line hidden
#nullable disable
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 25 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
                   WriteLiteral(package.Value);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 26 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
            }

#line default
#line hidden
#nullable disable
                WriteLiteral("        </select>\r\n    </div>\r\n\r\n    <button type=\"submit\" class=\"btn btn-primary\">Create Cover</button>\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

<hr />

<h3>Existing Covers</h3>

<table class=""table"">
    <thead>
        <tr>
            <th>Cover Amount</th>
            <th>Package</th>
            <th>Cover Type</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
");
#nullable restore
#line 47 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
         if (Model.Any())
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
             foreach (var cover in Model)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <!-- Form for editing the existing cover -->\r\n                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "05a8d05d24c294afffd2d663f9aedc68ea12d4f511272", async() => {
                WriteLiteral("\r\n                    <td>\r\n                        <input type=\"number\" class=\"form-control\" name=\"cov_amount\"");
                BeginWriteAttribute("value", " value=\"", 1657, "\"", 1682, 1);
#nullable restore
#line 55 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
WriteAttributeValue("", 1665, cover.cov_amount, 1665, 17, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" required />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" name=\"package_code\"");
                BeginWriteAttribute("value", " value=\"", 1833, "\"", 1859, 1);
#nullable restore
#line 58 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
WriteAttributeValue("", 1841, cover.PackageName, 1841, 18, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" required  readonly/>\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" name=\"cov_type\"");
                BeginWriteAttribute("value", " value=\"", 2015, "\"", 2038, 1);
#nullable restore
#line 61 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
WriteAttributeValue("", 2023, cover.cov_type, 2023, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" required />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"hidden\" name=\"cov_id\"");
                BeginWriteAttribute("value", " value=\"", 2164, "\"", 2185, 1);
#nullable restore
#line 64 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
WriteAttributeValue("", 2172, cover.cov_id, 2172, 13, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n                        <button type=\"submit\" class=\"btn btn-success\">Save</button>\r\n                    </td>\r\n                    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                </tr>\r\n");
#nullable restore
#line 69 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 69 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
             
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td colspan=\"4\">No covers available.</td>\r\n            </tr>\r\n");
#nullable restore
#line 76 "C:\Users\ardit\Desktop\Ardit\Bazi\db_tsh_final\db_tsh\Views\Home\Covers.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<db_tsh.Models.Covers>> Html { get; private set; }
    }
}
#pragma warning restore 1591
