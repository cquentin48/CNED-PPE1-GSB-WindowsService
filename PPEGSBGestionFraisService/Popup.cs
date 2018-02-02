using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PPEGSBGestionFraisService
{
    class Popup
    {
        public static readonly string ERROR = "";
        public static readonly string OK = "";

        //Informations du popup
        private string popupTitle;
        private string popupDesc;

        public Popup(string popupTitle, string popupDesc) {
            this.popupTitle = popupTitle;
            this.popupDesc = popupDesc;
        }
    }
}
