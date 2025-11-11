using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Cmune image
    /// </summary>
    public static class CmuneImage
    {
        /// <summary>
        /// This function allows us to write a JPEG file comming from the client on the server
        /// WARNING: If the image already exists, it will be replaced
        /// If the directory can not be reach, this function will throw an exception
        /// Previously called: uploadImage
        /// </summary>
        /// <param name="name">The future name of the picture on the server</param>
        /// <param name="directory">The directory where to write the image</param>
        /// <param name="maxPixelDimension">The max dimension in pixels of height AND width</param>
        /// <param name="inputStream">The uploaded file</param>
        public static void UploadImage(string name, string directory, int maxPixelDimension, Stream inputStream)
        {
            string tmpName = name + ".jpg";
            string tmpSavePath = directory + tmpName;
            int MaxSideSize = maxPixelDimension;
            int intNewWidth;
            int intNewHeight;

            using (System.Drawing.Image imgInput = System.Drawing.Image.FromStream(inputStream))
            {

                // We will only write in JPEG
                ImageFormat fmtImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

                //get image original width and height
                int intOldWidth = imgInput.Width;
                int intOldHeight = imgInput.Height;

                //determine if landscape or portrait
                int intMaxSide;

                if (intOldWidth >= intOldHeight)
                {
                    intMaxSide = intOldWidth;
                }
                else
                {
                    intMaxSide = intOldHeight;
                }

                if (intMaxSide > MaxSideSize)
                {
                    //set new width and height
                    double dblCoef = MaxSideSize / (double)intMaxSide;
                    intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                    intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
                }
                else
                {
                    intNewWidth = intOldWidth;
                    intNewHeight = intOldHeight;
                }

                //create new bitmap
                using (Bitmap bmpResized = new Bitmap(imgInput, intNewWidth, intNewHeight))
                {
                    //save bitmap to disk
                    // TODO try catch if the dir is not existing on the server
                    bmpResized.Save(tmpSavePath, fmtImageFormat);
                }
            }
        }

        /// <summary>
        /// This function allows us to write a JPEG file comming from the client on the server
        /// WARNING: If the image already exists, it will be replaced
        /// If the directory can not be reach, this function will throw an exception
        /// The image will be resize to the specified size
        /// Previously called: uploadImageFixedDimension
        /// </summary>
        /// <param name="name">The future name of the picture on the server</param>
        /// <param name="directory">The directory where to write the image</param>
        /// <param name="width">The dimension in pixels of width</param>
        /// <param name="height">The dimension in pixels of height</param>
        /// <param name="inputStream">The uploaded file</param>
        public static void UploadImageFixedDimension(string name, string directory, int width, int height, Stream inputStream)
        {
            string tmpName = name + ".jpg";
            string tmpSavePath = directory + tmpName;
            using (System.Drawing.Image imgInput = System.Drawing.Image.FromStream(inputStream))
            {
                // We will only write in JPEG
                ImageFormat fmtImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

                // Create new bitmap
                using (Bitmap bmpResized = new Bitmap(imgInput, width, height))
                {
                    // Save bitmap to disk
                    // TODO try catch if the dir is not existing on the server
                    bmpResized.Save(tmpSavePath, fmtImageFormat);
                }
            }
        }

        /// <summary>
        /// Previously called: deleteUploadedImage
        /// </summary>
        /// <param name="name"></param>
        /// <param name="directory"></param>
        public static void DeleteUploadedImage(string name, string directory)
        {
            if (File.Exists(directory + name))
            {
                File.Delete(directory + name);
            }
        }
    }
}
