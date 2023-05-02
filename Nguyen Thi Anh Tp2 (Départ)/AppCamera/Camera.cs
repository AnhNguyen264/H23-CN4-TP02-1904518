using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCamera
{  /// ===============================================================================================
   /// <summary>
   /// Représente La caméra possède des paramètres avec lesquels seront prises les photos. 
   /// Ces paramètres pourront être lus ou sauvegardés dans le fichier dont le nom est  « parametres.txt ».  Les caractéristiques de la caméra sont:
   /// la dimension , la qualite , la sensibilité et le flash est activé ou désactivé. 
    public class Camera : ICamera
    {
        #region CONSTANTS
        /// <summary>
        /// Représente message erreur si carte manquante
        /// </summary>
        public const string ERR_MSG_CARTE_MANQUANTE = "Carte manquante";
        /// <summary>
        /// Représente message erreur si carte déjà présente
        /// </summary>
        public const string ERR_MSG_CARTE_PRÉSENTE = "Carte déjà présente";
        /// <summary>
        /// Représente message erreur si carte vide
        /// </summary>
        public const string ERR_MSG_CARTE_VIDE = "Carte vide";
        /// <summary>
        /// Représente nom de fichier parametres.txt
        /// </summary>
        private const string NOM_FICHIER_PARAMETRES = "parametres.txt";

        #endregion
        #region CHAMPS
        /// ===============================================================================================
        /// <summary>
        ///Représente le paramètre dimension de la caméra.
        /// </summary>
        private enuDimension m_dimension;
        /// ===============================================================================================
        /// <summary>
        ///Représente le paramètre flash de la caméra.
        /// </summary>
        private enuFlash m_flash;
        /// ===============================================================================================
        /// <summary>
        ///Représente la carte mémoire qui est actuellement dans la caméra
        /// </summary>
        private ICarteMemoire m_objCarteMemoireCourante;
        /// ===============================================================================================
        /// <summary>
        ///Représente l’indice de la photo actuellement affichée sur la caméra.
        /// </summary>
        private int m_posPhotoCourante;
        /// ===============================================================================================
        /// <summary>
        ///Représente le paramètre qualite de la caméra.
        /// </summary>
        private enuQualite m_qualite;
        /// ===============================================================================================
        /// <summary>
        ///Représente le paramètre sensibilite de la caméra.
        /// </summary>
        private enuSensibilite m_sensibilite;

        #endregion

        #region PROPRIETES
        /// ===============================================================================================
        /// <summary>
        ///Obtient et définit le paramètre dimension de la caméra.
        /// </summary>
        public enuDimension Dimension
        {
            get { return m_dimension; }
            set { m_dimension = value; }
        }

        /// ===============================================================================================
        /// <summary>
        ///Obtient et définit le paramètre qualité de la caméra.
        /// </summary>
        public enuQualite Qualite {
            get { return m_qualite; }
            set { m_qualite = value; } }

        /// ===============================================================================================
        /// <summary>
        ///Obtient et définit le paramètre sensibilité de la caméra.
        /// </summary>
        public enuSensibilite Sensibilite {
            get { return m_sensibilite; }
            set { m_sensibilite=value; }
                }

        /// ===============================================================================================
        /// <summary>
        ///Obtient et définit le paramètre flash de la caméra.
        /// </summary>
        public enuFlash Flash
        {
            get { return m_flash; }
            set { m_flash = value; }
        }

        /// ===============================================================================================
        /// <summary>
        ///Obtient la carte mémoire qui est actuellement dans la caméra. 
        /// </summary>
        public ICarteMemoire CarteMemoireCourante
        { get { return m_objCarteMemoireCourante; }
        }

        /// ===============================================================================================
        /// <summary>
        ///Indique l’indice de la photo actuellement affichée sur la caméra
        /// </summary>
        public int PosPhotoCourante
        {
            get { return m_posPhotoCourante; }
        }

        /// ===============================================================================================
        /// <summary>
        ///Retournera l’objet photo contenu dans la collection de la carte mémoire courante à la position indiquée par la position courante. 
        /// </summary>
        public IPhoto PhotoCourante
        {
            get
            {
                if (m_objCarteMemoireCourante == null || m_objCarteMemoireCourante.NbPhotos == 0)
                    return null;
                else
                    return m_objCarteMemoireCourante.PhotoAt(m_posPhotoCourante);
            }
        }


        /// ===============================================================================================
        /// <summary>
        ///Obtient le facteur de compression qui est déterminé à partir de la qualité sélectionnée de la caméra. 
        /// </summary>
        public int FacteurCompression
        {
            get {
                int[] tabFacteur = { 4, 12, 20, 50 };
                return tabFacteur[(int)Qualite];
               ; }
        }

        /// ===============================================================================================
        /// <summary>
        ///Obtient la taille en pixels selon la dimension est obtenue grâce la largeur et la hauteur d'un object Size
        /// </summary>
        public Size TailleEnPixelsSelonDimension
        { get
            {
                Size[] tabSize  = { new Size() { Width= 4000,Height= 3000},new Size { Width = 3840, Height = 2160 }
                ,new Size { Width = 3000, Height = 2000 } ,new Size { Width = 1600, Height = 1200 },new Size { Width = 640, Height = 480 }};
                return tabSize[(int)Dimension];
            }
        }

        /// ===============================================================================================
        /// <summary>
        ///Calculez la taille de la photo 
        /// </summary>
        public long TailleEnOctetsEstimee
        {
            get { return TailleEnPixelsSelonDimension.Height * TailleEnPixelsSelonDimension.Width *Photo.TAILLE_PIXEL /FacteurCompression; }
        }



        #endregion

        #region CONSTRUCTEUR
        ///==============================================================
        /// <summary>
        ///  Initialise une nouvelle instance de la classe Camera 
        ///  les données sont: La position de la photo courante, la dimension, qualité , sensibilité, et le flash
        /// </summary>
        /// --------------------------------------------------------------
        public Camera()
        {
            m_posPhotoCourante = -1;
            m_dimension = enuDimension.Maximale;
            m_qualite = enuQualite.Excellente;
            m_sensibilite = enuSensibilite.ISO_64;
            m_flash = enuFlash.Activé;
            m_objCarteMemoireCourante = null;
        }
        #endregion


        #region METHODES

        ///======================================================================
        /// <summary>
        ///  Permet de lire les valeurs des paramètres dans le fichier et d'initialiser les 4 variables membres suivantes : dimension, qualité, sensibilité et flash. 
        /// </summary>
        /// ---------------------------------------------------------------------
        public void ChargerParametres()
        {
            
            StreamReader objFichier = new StreamReader(NOM_FICHIER_PARAMETRES);
           
            
                Dimension = (enuDimension)int.Parse(objFichier.ReadLine());
                Qualite = (enuQualite)int.Parse(objFichier.ReadLine());
                Sensibilite = (enuSensibilite)int.Parse(objFichier.ReadLine());
                Flash = (enuFlash)int.Parse(objFichier.ReadLine());
            
            objFichier.Close();
        }


        ///======================================================================
        /// <summary>
        ///  Permet de sauvegarder les 4 paramètres de la caméra dans le fichier. 
        /// </summary>
        /// ---------------------------------------------------------------------
        public void EnregistrerParametres()
        {
           
           StreamWriter objFichier = new StreamWriter(NOM_FICHIER_PARAMETRES);
            objFichier.WriteLine((int)Dimension);
            objFichier.WriteLine((int)Qualite);
            objFichier.WriteLine((int)Sensibilite);
            objFichier.WriteLine((int)Flash);

            objFichier.Close();

        }


        ///======================================================================
        /// <summary>
        ///  Définit la référence de la carte mémoire courante
        /// </summary>
        /// ---------------------------------------------------------------------
        public void InsererCarteMemoire(ICarteMemoire pCarteMemoire)
        {
            if (pCarteMemoire == null) throw new ArgumentNullException(ERR_MSG_CARTE_MANQUANTE);
            if (pCarteMemoire == m_objCarteMemoireCourante) throw new InvalidOperationException(ERR_MSG_CARTE_PRÉSENTE);
            m_objCarteMemoireCourante = pCarteMemoire;
            m_posPhotoCourante = 0;
        }

        ///======================================================================
        /// <summary>
        ///  Permet d’éjection de la carte mémoire
        /// </summary>
        /// ---------------------------------------------------------------------
        public void EjecterCarteMemoire()
        {
            if (CarteMemoireCourante == null) throw new InvalidOperationException(ERR_MSG_CARTE_MANQUANTE);
            m_objCarteMemoireCourante = null;
            m_posPhotoCourante = -1;
        }


        ///======================================================================
        /// <summary>
        ///  Permet de vider la carte mémoire actuellement dans la caméra.
        /// </summary>
        /// ---------------------------------------------------------------------
        public void ViderLaCarte()
        {
            if (m_objCarteMemoireCourante == null) throw new InvalidOperationException(ERR_MSG_CARTE_MANQUANTE);
            CarteMemoireCourante.Vider();
        }

        ///======================================================================
        /// <summary>
        ///  Indique s’il est possible de prendre une photo actuellement avec les paramètres courant de la caméra.
        /// </summary>
        /// ---------------------------------------------------------------------
        public bool PeutPrendreUnePhoto()
        {
            if(m_objCarteMemoireCourante != null && TailleEnOctetsEstimee <= CarteMemoireCourante.EspaceDisponible)
            {
                return true;
            } else
            return false;
        }

        ///======================================================================
        /// <summary>
        /// Crée une photo avec les paramètres actuels de la caméra. 
        /// </summary>
        /// ---------------------------------------------------------------------
        public void PrendrePhoto()
         {
            if (m_objCarteMemoireCourante == null) throw new InvalidOperationException(ERR_MSG_CARTE_MANQUANTE);
            if (!PeutPrendreUnePhoto())
            {
                throw new InvalidOperationException(CarteMemoire.ERR_MSG_ESPACE_INSUFFISANT);
            }
            Photo NouvellePhoto = new Photo(TailleEnPixelsSelonDimension,FacteurCompression,Sensibilite,Flash);
            CarteMemoireCourante.Ajouter(NouvellePhoto);
            m_posPhotoCourante = CarteMemoireCourante.NbPhotos-1;
        }

        ///======================================================================
        /// <summary>
        ///  Permet de supprimer la photo de la carte mémoire courante si il y a une carte dans la camera
        /// </summary>
        /// ---------------------------------------------------------------------
        public void SupprimerPhotoCourante()
        {
            if (m_objCarteMemoireCourante == null) throw new InvalidOperationException(ERR_MSG_CARTE_MANQUANTE);
            if(CarteMemoireCourante.NbPhotos ==0) throw new InvalidOperationException(ERR_MSG_CARTE_VIDE);
            CarteMemoireCourante.SupprimerAt(PosPhotoCourante);
            m_posPhotoCourante = CarteMemoireCourante.NbPhotos - 1;
        }

        ///======================================================================
        /// <summary>
        ///  Permet de passer à la prochaine valeur de qualité dans leurs émum respectifs. 
        ///Lorsqu’on arrive au bout, on revient à la 1ère.
        /// </summary>
        /// ---------------------------------------------------------------------
        public void QualiteSuivante()
        {
            if(m_qualite == enuQualite.Faible)
            {
                m_qualite = enuQualite.Excellente;
            }else
            m_qualite++;
        }

        ///======================================================================
        /// <summary>
        ///  Permet de passer à la prochaine valeur de dimension dans leurs émum respectifs. 
        ///Lorsqu’on arrive au bout, on revient à la 1ère.
        /// </summary>
        /// ---------------------------------------------------------------------
        public void DimensionSuivante()
        {
            if(m_dimension ==enuDimension.Petite)
            {
                m_dimension = enuDimension.Maximale;
            }else
            m_dimension++;
        }

        ///======================================================================
        /// <summary>
        ///  Permet de passer à la prochaine valeur de sensibilité dans leurs émum respectifs. 
        ///Lorsqu’on arrive au bout, on revient à la 1ère.
        /// </summary>
        /// ---------------------------------------------------------------------
        public void SensibiliteSuivante()
        {
            if(m_sensibilite==enuSensibilite.ISO_800)
            {
                m_sensibilite = enuSensibilite.ISO_64;
            }else
            m_sensibilite++;
        }

        ///======================================================================
        /// <summary>
        ///  Recule à la photo suivante ou recule à la photo précédente. Se fait de façon circulaire…
        /// </summary>
        /// ---------------------------------------------------------------------
        public void PhotoPrecedente()
        {
            if (m_objCarteMemoireCourante == null) throw new InvalidOperationException(ERR_MSG_CARTE_MANQUANTE);
            if (CarteMemoireCourante.NbPhotos == 0) throw new InvalidOperationException(ERR_MSG_CARTE_VIDE);
            if(PosPhotoCourante > 0)
            m_posPhotoCourante--;
           else m_posPhotoCourante = CarteMemoireCourante.NbPhotos-1;
        }

        ///======================================================================
        /// <summary>
        ///  Avance à la photo suivante ou recule à la photo précédente. Se fait de façon circulaire…
        /// </summary>
        /// ---------------------------------------------------------------------
        public void PhotoSuivante()
        {
            if (m_objCarteMemoireCourante == null) throw new InvalidOperationException(ERR_MSG_CARTE_MANQUANTE);
            if (CarteMemoireCourante.NbPhotos == 0) throw new InvalidOperationException(ERR_MSG_CARTE_VIDE);
            if (PosPhotoCourante < CarteMemoireCourante.NbPhotos - 1)
                m_posPhotoCourante++;
            else m_posPhotoCourante = 0;

        }


        #endregion

    }
}
