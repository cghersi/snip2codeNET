//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.IO;
using Snip2Code.Utils;
using System.Drawing.Drawing2D;
using System.Net;
using Snip2Code.Model.Client.WSFramework;

namespace Snip2Code.Utils
{
    public class PictureManager
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static long expirationTime = 15; //15 days

        public static string UserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Snip2CodeBaseDir = System.IO.Path.Combine(UserAppData, "Snip2Code");
        public static string ImagesBaseDir = System.IO.Path.Combine(Snip2CodeBaseDir, "ImagesCache");

        public static Image emptyProfilePic = null;

        /// <summary>
        /// Sets the profile picture for default users that don't have their picture
        /// </summary>
        /// <param name="absolutePath">absolute path of the directory containing emptyProfilePic50x50.png</param>
        public static void SetEmptyProfilePic(string absolutePath)
        {
            try
            {
                emptyProfilePic = Image.FromFile(System.IO.Path.Combine(absolutePath, "emptyProfilePic50x50.png"));
            }
            catch { }
        }


        /// <summary>
        /// Retrieves the image of the user: if available, searches it locally,
        /// otherwise issues a remote call and stores the result on local file system
        /// </summary>
        /// <param name="userID">ID of the user whose picture is asked</param>
        /// <param name="size">the preferred size in output</param>
        /// <param name="thumbUploadNum">thumbUploadNum incremental number of thumbanil (prevents caching problem on refresh)</param>
        /// <returns>the picture with the empty profile if not found or any 
        /// error occurred; the user picture otherwise</returns>
        public static Image GetImageOfUser(int userID, Size size, int thumbUploadNum)
        {
            if (userID <= 0)
                return emptyProfilePic;

            // try to get the image from local cache:
            // the name of the file is in the form pic_<userID>_<thumbUploadNum>.png
            String localCacheName = "pic_" + userID + "_" + thumbUploadNum + ".png";

            String localCacheName_FullPath = System.IO.Path.Combine(ImagesBaseDir, localCacheName);

            DateTime pictureModifiedTime = new DateTime(1970, 1, 1);

            if (System.IO.File.Exists(localCacheName_FullPath)) 
            {
                System.IO.FileInfo pictureInfo = new System.IO.FileInfo(localCacheName_FullPath);
                pictureModifiedTime = pictureInfo.LastWriteTime.ToUniversalTime();
            }
            
            // file exist and is recent
            if (DateTime.Compare(pictureModifiedTime.AddDays(expirationTime), DateTime.UtcNow) >= 0)
            {
                //get the image from file:
                try 
                {
                    Image res = Image.FromFile(localCacheName_FullPath);
                    if (res == null)
                        return emptyProfilePic;
                    
                    //resize the image if needed:
                    if ((res.Width != size.Width) || (res.Height != size.Height)) 
                    {
                        try 
                        {
                            return ResizeImage(res, size);
                        }
                        catch (Exception e)
                        {
                            log.Error("Error resizing the image of user " + userID, e);
                            return emptyProfilePic;
                        }
                    } 
                    else
                        return res;
                } 
                catch(Exception e) 
                {
                    log.Error("Error reading user image from filesystem", e);
                    return emptyProfilePic;
                }
            } 
            else 
            {
                //if the image is not yet stored locally, or too old, retrieve from remote server and store it:
                try 
                {
                    return SaveImageToFile(userID, size, localCacheName_FullPath, thumbUploadNum);
                } 
                catch (Exception e) 
                {
                    log.Error("Cannot write the image of user " + userID, e);
                    return emptyProfilePic;
                }
            }
        }

        private static Image SaveImageToFile(int userID, Size size, String nameOfStoredImage, int thumbUploadNum) 
        {
            //fetch from remote server:
            String imgUrl = BaseWS.Server + "Images/Get?id=" + userID + "&size=" + size + "&count=" + thumbUploadNum;

            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(imgUrl);
            HttpWebResponse aResponse = (HttpWebResponse)aRequest.GetResponse();

            if (aResponse.ContentType.StartsWith("text/") || aResponse.ContentLength <= 0) 
                throw new IOException("This is not a valid binary file.");

            Stream s = null;
            try
            {
                s = aResponse.GetResponseStream();
                Image im = Image.FromStream(s);

                // create Snip2Code Base Directory for current user, if does not exist on filesystem
                if (!System.IO.Directory.Exists(Snip2CodeBaseDir))
                    System.IO.Directory.CreateDirectory(Snip2CodeBaseDir);
                // create Images Base Directory for current user, if does not exist on filesystem
                if (!System.IO.Directory.Exists(ImagesBaseDir))
                    System.IO.Directory.CreateDirectory(ImagesBaseDir);

                // resize and save the image onto file system:
                Image resized = ResizeImage(im, size);
                resized.Save(nameOfStoredImage, System.Drawing.Imaging.ImageFormat.Png);

                s.Dispose();
                return resized;
            }
            catch(Exception e)
            {
                log.Error("Error saving the image of user " + userID, e);
                s.Dispose();
            }

            return emptyProfilePic;
        }


        private static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
              nPercent = nPercentH;
            else
              nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }
    }
}
