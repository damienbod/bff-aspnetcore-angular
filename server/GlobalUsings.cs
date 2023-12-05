global using BffMicrosoftEntraID.Server;
global using BffMicrosoftEntraID.Server.Models;
global using BffMicrosoftEntraID.Server.Services;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.OpenIdConnect;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.Graph;
global using Microsoft.Graph.Models;
global using Microsoft.Identity.Client;
global using Microsoft.Identity.Web;
global using Microsoft.Identity.Web.UI;
global using Microsoft.IdentityModel.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Net.Http.Headers;

global using System.Diagnostics;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Security.Claims;

global using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
global using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
