using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCamera
{
    public class Photo : IPhoto
    {

        /// ===============================================================================================
        /// <summary>
        /// Représente une photo qui est caractérisée par sa taille en pixels (largeur, hauteur), le flash activé ou non,  la sensibilité, le facteur de compression ainsi que  la date et l’heure où elle a été prise.
        /// </summary>
        #region CONTANTES
        /// <summary>
        /// Représente taile pixel
        /// </summary>
        public const int TAILLE_PIXEL = 3;
        /// <summary>
        /// Représente largeur min de la photo
        /// </summary>
        public const int LARGEUR_MIN = 640;
        /// <summary>
        /// Représente largeur max de la photo
        /// </summary>
        public const int LARGEUR_MAX = 8192;
        /// <summary>
        /// Représente hauteur min de la photo
        /// </summary>
        public const int HAUTEUR_MIN = 480;
        /// <summary>
        /// Représente hauteur max de la photo
        /// </summary>
        public const int HAUTEUR_MAX = 8192;
        /// <summary>
        /// Représente compression min de la photo
        /// </summary>
        public const int COMPRESSION_MIN = 1;
        /// <summary>
        /// Représente compression max de la photo
        /// </summary>
        public const int COMPRESSION_MAX = 500;

        #endregion

        #region CHAMPS
        /// ===============================================================================================
        /// <summary>
        ///Représente la date et l’heure
        /// </summary>
        private DateTime m_date;
        /// ===============================================================================================
        /// <summary>
        ///Représente le facteur de compression de la photo.
        /// </summary>
        private int m_facteurCompression;
        /// ===============================================================================================
        /// <summary>
        ///Représente si le flash est activé ou désactivé sur la photo.
        /// </summary>
        private enuFlash m_flash;
        /// ===============================================================================================
        /// <summary>
        ///Représente la sensibilité utilisée pour  la photo.
        /// </summary>
        private enuSensibilite m_sensibilite;
        /// ===============================================================================================
        /// <summary>
        ///Représente la taille en pixels de la photo
        /// </summary>
        private Size m_tailleEnPixels;

        #endregion
        #region PROPRIETES
        /// ===============================================================================================
        /// <summary>
        ///Obtient la date et l’heure où la photo a été prise.
        /// </summary>
        public DateTime Date
        {
            get { return m_date; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient le facteur de compression de la photo.
        /// </summary>
        public int FacteurCompression
        {
            get { return m_facteurCompression; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient la sensibilité utilisée pour  la photo.
        /// </summary>
        public enuFlash Flash
        {
            get { return m_flash; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient la taille en pixels de la photo
        /// </summary>
        public enuSensibilite Sensibilite
        {
            get { return m_sensibilite; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Calcule la taille en octets de la photo
        /// </summary>
        public int TailleEnOctets
        {
            get { return m_tailleEnPixels.Height * m_tailleEnPixels.Width * TAILLE_PIXEL / m_facteurCompression; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient la taille en pixels de la photo
        /// </summary>
        public Size TailleEnPixels
        {
            get { return m_tailleEnPixels; }
        }
        #endregion

        #region CONSTRUCTEUR
        ///==============================================================
        /// <summary>
        ///  Initialise une nouvelle instance de la classe Photo 
        ///  avec une chaine d'informations de chacun des champs séparés
        ///  (largeur et hauteur en pixel, flash, sensibilité, facteur de compression, date hre )
        /// </summary>
        /// --------------------------------------------------------------
        public Photo(Size pTailleEnPixels, int pFacteurCOmpression, enuSensibilite pSensibilite, enuFlash pFlash)
        {
            m_tailleEnPixels = pTailleEnPixels;
            if (pTailleEnPixels.Width < LARGEUR_MIN || pTailleEnPixels.Width > LARGEUR_MAX) throw new ArgumentOutOfRangeException();
            if (pTailleEnPixels.Height < HAUTEUR_MIN || pTailleEnPixels.Height > HAUTEUR_MAX) throw new ArgumentOutOfRangeException();

            m_facteurCompression = pFacteurCOmpression;
            if (pFacteurCOmpression < COMPRESSION_MIN || pFacteurCOmpression > COMPRESSION_MAX) throw new ArgumentOutOfRangeException();

            m_sensibilite = pSensibilite;
            m_flash = pFlash;
            m_date = DateTime.Now;
        }

        ///==============================================================
        /// <summary>
        ///  Initialise une nouvelle instance de la classe Photo 
        ///  avec une chaine d'informations de chacun des champs séparés par une virgule
        ///  (largeur et hauteur en pixel, flash, sensibilité, facteur de compression, date hre )
        /// </summary>
        /// --------------------------------------------------------------
        public Photo(string pInfosPhoto)
        {
            string[] tabInfosPhoto = pInfosPhoto.Split(',');
            m_tailleEnPixels.Width = int.Parse(tabInfosPhoto[0]);
            if (int.Parse(tabInfosPhoto[0]) < LARGEUR_MIN || int.Parse(tabInfosPhoto[0]) > LARGEUR_MAX) throw new ArgumentOutOfRangeException();

            m_tailleEnPixels.Height = int.Parse(tabInfosPhoto[1]);
            if (int.Parse(tabInfosPhoto[1]) < HAUTEUR_MIN || int.Parse(tabInfosPhoto[1]) > HAUTEUR_MAX) throw new ArgumentOutOfRangeException();
            m_flash = (enuFlash)int.Parse(tabInfosPhoto[2]);
            m_sensibilite = (enuSensibilite)int.Parse(tabInfosPhoto[3]);
            m_facteurCompression = int.Parse(tabInfosPhoto[4]);
            m_date = DateTime.Parse(tabInfosPhoto[5]);
        }
        #endregion

        #region METHODES 
        ///======================================================================
        /// <summary>
        ///  Permet former une chaine de caractères contenant les informations de chancune des variables membres séparés par une virgule. 
        /// </summary>
        /// ---------------------------------------------------------------------

        public override string ToString()
        {
            return TailleEnPixels.Width + "," + TailleEnPixels.Height + "," + (int)Flash + "," + 
                (int)Sensibilite + "," + FacteurCompression + "," + Date.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion
    }
}
