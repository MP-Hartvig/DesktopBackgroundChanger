using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DesktopBackgroundChanger
{
    internal class Image
    {
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private BitmapImage bitMapImage;

        public BitmapImage BitMapImage
        {
            get { return bitMapImage; }
            set { BitmapImage bitMapImage = value; }
        }

        public Image(string path, BitmapImage bitMapImage)
        {
            this.path = path;
            this.bitMapImage = bitMapImage;
        }


        public string GetImageName(string mainPath, string defaultImageName)
        {
            string trueFileName = path.Substring(mainPath.Length);

            return trueFileName.Split(".")[0];
        }

        public int GetFileNumber(string mainPath, string defaultImageName)
        {
            string trueFileName = path.Substring(mainPath.Length);

            string fileNameNoExt = trueFileName.Split(".")[0];

           return Convert.ToInt16(fileNameNoExt.Substring(defaultImageName.Length));
        }


    }
}
