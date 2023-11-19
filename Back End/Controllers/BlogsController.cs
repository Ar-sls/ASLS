using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Services;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private IBlogsService _IBlogsService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public BlogsController(IBlogsService IBlogsService, IHostingEnvironment hostingEnvironment)
        {
            _IBlogsService = IBlogsService;
            _hostingEnvironment = hostingEnvironment;
        }
     
        
        
        [HttpGet("GetBlogs")]        
        public IActionResult GetBlogs(string word)
        {
           
            var BlogsTable = _IBlogsService.GetBlogs(word);
            return Ok(BlogsTable);
        }

        [HttpGet("GetCleanBlogResult")]        
        public IActionResult GetCleanBlogResult(string GUID)
        {
            var BlogsTable = _IBlogsService.GetCleanBlogs(GUID);
            return Ok(BlogsTable);
        }

        [HttpGet("StemBlogResult")]        
        public IActionResult GetStemBlogResult(string GUID)
        {
            var BlogsTable = _IBlogsService.GetStemBlogs(GUID);
            return Ok(BlogsTable);
        }
    }

}