using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ideative.Domain.Models;
using Ideative.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string GetLocalUrl(this IUrlHelper urlHelper, string localUrl)
        {
            if (!urlHelper.IsLocalUrl(localUrl))
            {
                return urlHelper.Page("/Index");
            }

            return localUrl;
        }

        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { userId, code },
                protocol: scheme);
        }
    }

    public static class IdentityExtensions
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string getUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return string.Empty;
            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        #region Resim Ýþlemleri ...

     

        public static string getImageFileExtension()
        {
            return ".jpg";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cno"></param>
        /// <param name="WebRootPath"></param>
        /// <returns></returns>
        public static string toImagePath(this int? cno, string WebRootPath)
        {
            return cno.ToString().toImagePath(WebRootPath);
        }
        public static string toImagePath(this int cno, string WebRootPath)
        {
            return cno.ToString().toImagePath(WebRootPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cno"></param>
        /// <param name="WebRootPath"></param>
        /// <returns></returns>
        public static string toImagePath(this string cno, string WebRootPath)
        {
            if (string.IsNullOrEmpty(cno.ToString()) || cno.ToString().Length < 9)
                throw new Exception("ERROR-CH001");
            var len = cno.ToString().Length;
            var cnos = cno.ToString();
            var folder = "";
            if (!string.IsNullOrEmpty(WebRootPath))
            {
                folder = WebRootPath.Replace("\\", "/") + "/";
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder.Replace("\\", "/"));
            }
            folder +=
                /*cnos.Substring(len - 8, 2) + "/" +*/
                cnos.Substring(len - 6, 2) + "/" +
                cnos.Substring(len - 4, 2) + "/" +
                cnos.Substring(len - 2, 2);

            return folder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="cno"></param>
        /// <param name="WebRootPath"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        public static List<string> imageList(this Guid cid, int? cno, string WebRootPath, bool full = false)
        {

            string targetFolder = "uploads";
            if (!WebRootPath.Contains("uploads")) { targetFolder = "thumbs"; }

            string folder = cno.toImagePath(WebRootPath).Replace("\\", "/");
            if (!Directory.Exists(folder))
                return new List<string>();
            //Directory.CreateDirectory(folder.Replace("\\", "/"));

            if (full)
                return Directory.GetFiles(folder)
                    .Where(t => t.Contains(cid.ToString()))
                    .Select(t => ("/" + targetFolder + t.Replace(WebRootPath.Replace("\\", "/"), "")).Replace("\\", "/")).ToList();
            else
                return Directory.GetFiles(folder)
                    .Where(t => t.Contains(cid.ToString()))
                    .Select(t => ("/" + targetFolder + t.Replace(WebRootPath.Replace("\\", "/"), "")).Replace("\\", "/")).ToList();
        }
        #endregion


        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }
}
