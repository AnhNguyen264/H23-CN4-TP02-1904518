using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCamera
{
    public class CarteMemoire : ICarteMemoire
    {
        /// ===============================================================================================
        /// <summary>
        /// Représente une carte ménoire qui est est définie par les caractéristiques: nom,taille en octets,l’espace utilisé en octets,la collection de photos stockées sur la carte. 
        /// </summary>
        #region CONTANTES
        /// <summary>
        /// Représente minimum de taille de carte mémoire
        /// </summary>
        public const int TAILLE_MIN = 8; //  8 Mo
        /// <summary>
        /// Représente maximum de taille de carte mémoire
        /// </summary>
        public const int TAILLE_MAX = 8192; // 8192 Mo
        /// <summary>
        /// Représente message erreur
        /// </summary>
        public const string ERR_MSG_ESPACE_INSUFFISANT = "Espace insuffisant";

        #endregion

        #region CHAMPS
        /// ===============================================================================================
        /// <summary>
        ///Représente la collection sur le liste IPhoto
        /// </summary>
        private List<IPhoto> m_colPhotos = new List<IPhoto>();
        /// ===============================================================================================
        /// <summary>
        ///Représente l’espace utilisé en octets.
        /// </summary>
        private long m_espaceUtilise;
        /// ===============================================================================================
        /// <summary>
        ///Représente le nom de la carte mémoire.
        /// </summary>
        private string m_nom;
        /// ===============================================================================================
        /// <summary>
        ///Représente la taille en octets de la carte mémoire. 
        /// </summary>
        private long m_tailleEnOctets;

        #endregion


        #region PROPRIETES
        /// ===============================================================================================
        /// <summary>
        ///Calcule l’espace en octets disponible sur la carte mémoire.
        /// </summary>
        public long EspaceDisponible
        { get { return m_tailleEnOctets- m_espaceUtilise; } }
        /// ===============================================================================================
        /// <summary>
        ///Obtient l’espace utilisé en octets.
        /// </summary>
        public long EspaceUtilise
        {
            get { return m_espaceUtilise; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient le nombre de photos actuellement stockées dans la collection.
        /// </summary>
        public int NbPhotos
        {
            get { return m_colPhotos.Count; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient le nom de la carte mémoire.
        /// </summary>
        public string Nom
        {
            get { return m_nom; }
        }
        /// ===============================================================================================
        /// <summary>
        ///Obtient la taille en octets de la carte mémoire. 
        /// </summary>
        public long TailleEnOctets
        {
            get { return m_tailleEnOctets; }
        }

        #endregion

        #region CONSTRUCTEURS
        ///==============================================================
        /// <summary>
        ///  Initialise une nouvelle instance de la classe CarteMemoire 
        ///  préciser le nom de la carte et sa taille en MegaOctets. 
        /// </summary>
        /// --------------------------------------------------------------
        public CarteMemoire(string pNom, int pTailleEnMegaOctets)
        {
            
            if (pTailleEnMegaOctets < TAILLE_MIN  || pTailleEnMegaOctets > TAILLE_MAX ) throw new ArgumentOutOfRangeException(ERR_MSG_ESPACE_INSUFFISANT);

            m_nom = pNom;
            m_tailleEnOctets = pTailleEnMegaOctets * 1024L * 1024;
            
            ChargerPhotos();
        }
        #endregion

        #region METHODES
        ///======================================================================
        /// <summary>
        ///  Permet ajouter ajoutez la photo à la collection de photos ,
        /// </summary>
        /// ---------------------------------------------------------------------
        public void Ajouter(IPhoto pobjPhoto)
        {
         if (pobjPhoto == null) throw new ArgumentNullException();
            if(!PeutAjouter(pobjPhoto))
            {
                throw new InvalidOperationException(ERR_MSG_ESPACE_INSUFFISANT);
            }
            m_colPhotos.Add(pobjPhoto);
            m_espaceUtilise += pobjPhoto.TailleEnOctets;
        }

        ///======================================================================
        /// <summary>
        ///  Permet de stocker dans la collection de photos, 
        ///  les données qui sont dans le fichier dont le nom est formé à l’aide du nom de la carte et du suffixe .txt 
        /// </summary>
        /// ---------------------------------------------------------------------
        public void ChargerPhotos()
        {

            Vider();
            if(File.Exists(Nom + ".txt"))
            {
                StreamReader objFichier = new StreamReader(Nom + ".txt");
                while (!objFichier.EndOfStream)
                {
                    string nom = objFichier.ReadLine();
                    Photo unePhoto = new Photo(nom);
                    Ajouter(unePhoto);
                }
                objFichier.Close();
            }
           

        }
        ///======================================================================
        /// <summary>
        ///  Permet de de sauvegarder les photos de la collection vers un fichier dont le nom est formé à l’aide du nom de la carte et du suffixe .txt
        /// </summary>
        /// ---------------------------------------------------------------------
        public void EnregistrerPhotos()
        {

         StreamWriter objFichier = new StreamWriter(Nom + ".txt");
           // Photo unePhoto = new Photo(Nom);
            
            foreach(Photo photo in m_colPhotos)
            {
               objFichier.WriteLine(photo.ToString());
            }
            objFichier.Close();
            Vider();
        }
        ///======================================================================
        /// <summary>
        ///  Indiquer oui ou non, , l’espace disponible et la photo qu’on veut ajouter n’est pas null.
        /// </summary>
        /// ---------------------------------------------------------------------
        public bool PeutAjouter(IPhoto pobjPhoto)
        {
            if (pobjPhoto != null && pobjPhoto.TailleEnOctets <= EspaceDisponible)
            {
                return true;
            }
            else
                return false;
           // return pobjPhoto.TailleEnOctets <= EspaceDisponible && pobjPhoto != null;
        }
        ///======================================================================
        /// <summary>
        /// Permet d’obtenir l’objet photo dont la position est indiquée en paramètre
        /// </summary>
        /// ---------------------------------------------------------------------
        public IPhoto PhotoAt(int pPosition)
        {
            if (pPosition < 0 || pPosition > m_colPhotos.Count -1) throw new ArgumentOutOfRangeException();

            return m_colPhotos[pPosition];
        }
        ///======================================================================
        /// <summary>
        /// Supprime de la collection, la photo dont la position est indiquée en paramètre
        /// </summary>
        /// ---------------------------------------------------------------------
        public void SupprimerAt(int pPosition)
        {
            if (pPosition < 0 || pPosition > m_colPhotos.Count - 1)
                throw new ArgumentOutOfRangeException();

            m_espaceUtilise -= m_colPhotos[pPosition].TailleEnOctets;
            m_colPhotos.RemoveAt(pPosition);
           
        }
        ///======================================================================
        /// <summary>
        ///  Vide complètement la collection de photos et ajuste l’espace utilisé sur la carte mémoire
        /// </summary>
        /// ---------------------------------------------------------------------
        public void Vider()
        {
           /* foreach (Photo photo in m_colPhotos)
            {
                m_colPhotos.Remove(photo);
            }*/
           m_colPhotos.Clear();
           m_espaceUtilise = 0;
         
        }
        #endregion
    }
}
