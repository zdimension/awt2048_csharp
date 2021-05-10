using System;
using java.io;
using java.util;
using boolean = System.Boolean;

namespace test_vjs.package2048
{
    /**
 * Une classe pour représenter le Jeu 2048 (<em>Le modèle</em>).<br/>
 *
 * Le jeu se joue sur une grille de Cases (par défaut 4×4). 
 * Chaque case a une valeur entière : zéro pour représenter un emplacement vide,
 * ou une puissances de deux pour les non vides. <br/>
 * Les cases peuvent être décalées quand le joueur appuie sur 
 * une des fléchées de son clavier ou en cliquant avec la souris.<br/>
 * Deux cases non-vides de même nombre, peuvent fusionner si elles sont côte à côte
 * (horizontalement ou verticalement) ou séparées par des vides.<br/>
 * À chaque décalage, une case portant un 2 ou un 4 apparaît dans un emplacement 
 * vide de manière aléatoire.<br/>
 * 
 * La partie se termine si le jeu est bloqué ou si l'utilisateur obtient une case 
 * portant la valeur « 2048 » (valeur paramétrable).<br />
 * Remarques : 
 * <ul><li>La classe hérite de Observable. <br />
 * Cela signifie que tout changement de la grille
 * entraine la notification du changement à tous les "observateurs".</li> 
 * <li>La classe implémente l'interface Serializable, nécessaire si on
 * désire sauvegarder l'état du jeu.</li>
 * <li>La grille peut être visualisée comme un tableau de deux dimensions.
 * Le premier indice (dimension) represente les lignes de la grille,
 * le deuxième les colonnes. 
 * La grille peut être obtenue avec sous la forme d'un tableau de String ou sous
 * la forme d'un tableau de int avec : getGrilleString() ou avec getGrilleInt().<br/> 
 * La grille peut aussi être modifiée avec setGrilleInt(int[,] tab).
 * Ces méthodes peuvent rendre service si vous désirez sauvegarder l'état du jeu. </li>
 * </ul>
 * @author  Miguel Tomasena
 * @see #decaler(int)
 * @see #getGrilleString()
 * @see #getGrilleInt()
 * @see #setGrilleInt(int[,])
 */
    public class Jeu2048 : Observable, Serializable
    {
        private const long serialVersionUID = 1L;

        /**
	 *  Entier représentant le nombre de lignes de la grille (par défaut 4).
	 */
        private int nbLignes;

        /**
	 *  Entier représentant le nombre de colonnes de la grille (par défaut 4).
	 */
        private int nbCols;

        /**
	*  Entier représentant le nombre (puissance de 2) à obtenir pour gagner le jeu
	*  (par défaut 2048).
	*/
        private int nbBut;

        /**
	*  Entier représentant l'addition des fusions depuis le debut du jeu.
	*/
        private int score;

        /**
	*  Entier représentant le meilleur des scores.
	*/
        private int bestScore;

        /**
	 *  Constante servant à désigner le décalage des colonnes vers le haut.	 */
        public const int HAUT = 0;

        /**
	 *  Constante servant à désigner le décalage des lignes vers la droite.	 */
        public const int DROITE = 1;

        /**
	 *  Constante servant à désigner le décalage des colonnes vers le bas.	 */
        public const int BAS = 2;

        /**
	 *  Constante servant à désigner le décalage des lignes vers la gauche.	 */
        public const int GAUCHE = 3;

        private Case[,] grille;
        private Case[,] grilleReverse;
        private Case[,] grilleTrans;
        private Case[,] grilleTransReverse;

        /**
	 * Constructeur par défaut du jeu.
	 *
	 * On construit une grille de 4 x 4, le but à atteindre est 2048.
	 */
        public Jeu2048() : this(4, 4, 2048)
        {
        }

        /**
	 * Création et initialisation d'une grille de nbLignes x nbCols. Le but à atteindre est 
	 * nbBut.
	 * @param nbLignes : nombre de lignes de la grille
	 * @param nbCols : nombre de colonnes de la grille
	 * @param nbBut : nombre à atteindre pour gagner, 
	 */
        public Jeu2048(int nbLignes, int nbCols, int nbBut)
        {
            this.nbLignes = nbLignes;
            this.nbCols = nbCols;
            this.nbBut = nbBut;
            this.setScore(0);
            this.grille = new Case[nbLignes,nbCols];
            this.grilleReverse = new Case[nbLignes,nbCols];
            this.grilleTrans = new Case[nbCols,nbLignes];
            this.grilleTransReverse = new Case[nbCols,nbLignes];

            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
            {
                grille[l,c] = new Case(0);
                grilleReverse[nbLignes - l - 1,c] = grille[l,c];
                grilleTrans[c,l] = grille[l,c];
                grilleTransReverse[nbCols - c - 1,l] = grilleTrans[c,l];
            }

            addNum(2);
            addRandomNum();
        }

        /**
	 * La méthode initialise la grille de jeu.
	 * La taille reste la même.
	 * La grille sera vide sauf deux de cases contenant un 2 et un
	 * 2 ou 4 placés aléatoirement.
	 */
        public void nouveauJeu()
        {
            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
            {
                grille[l,c].setValue(0);
                grille[l,c].setaEteFusione(false);
            }

            setScore(0);
            addNum(2);
            addRandomNum();
        }

        /**
	 * On initialise la grille de jeu à partir d'un tableau d'entiers.
	 * @param tab : tableau d'entiers.
	 */
        public void setGrilleInt(int[,] tab)
        {
            this.nbLignes = tab.GetLength(0);
            this.nbCols = tab.GetLength(1);
            this.grille = new Case[nbLignes,nbCols];
            this.grilleReverse = new Case[nbLignes,nbCols];
            this.grilleTrans = new Case[nbCols,nbLignes];
            this.grilleTransReverse = new Case[nbCols,nbLignes];

            int scoreTab = -4; // il y a au moins 4 avant les fussions
            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
            {
                grille[l,c] = new Case(tab[l,c]);
                grilleReverse[nbLignes - l - 1,c] = grille[l,c];
                grilleTrans[c,l] = grille[l,c];
                grilleTransReverse[nbCols - c - 1,l] = grilleTrans[c,l];
                scoreTab = scoreTab + tab[l,c];
            }

            setScore(scoreTab);
            setChanged();
            notifyObservers();
        }

        /**
	 * Méthode importante du jeu.<br/>
	 * La méthode décale la grille dans la direction signalée par le paramètre.
	 * S'il y a décalage ou fusion, alors un nouveau nombre (2 ou 4) est placé aléatoirement
	 * dans la grille et la méthode retourne true.<br/>	
	 * Si le décalage ne peut pas s'effectuer et il n'y a pas de fusion, alors la grille reste 
	 * inchangée et la méthode retourne false.
	 * @param direction : entier (constante) représentant une des quatre directions
	 * (Jeu2048.HAUT, Jeu2048.BAS, Jeu2048.GAUCHE ou Jeu2048.DROITE).
	 * @return true si un nouveau nombre est placé dans la grille.
	 * @see #tableauFusions()
	 */
        public boolean decaler(int direction)
        {
            if (!estTermine())
                switch (direction)
                {
                    case HAUT: return decaler(grille);
                    case BAS: return decaler(grilleReverse);
                    case GAUCHE: return decaler(grilleTrans);
                    case DROITE: return decaler(grilleTransReverse);
                }

            return false;
        }

        private boolean decaler(Case[,] grille)
        {
            // decale toujours vers le haut
            boolean ilYADecalageOuFusion = false;
            for (int c = 0; c < grille.GetLength(1); c++)
            {
                // fusions à false
                for (int l = 0; l < grille.GetLength(0); l++)
                    grille[l,c].setaEteFusione(false);
                // pour chaque colonne, on cherche à fusioner deux identiques
                for (int l1 = 0; l1 < grille.GetLength(0) - 1; l1++)
                {
                    if (grille[l1,c].getValue() != 0)
                    {
                        int l2 = l1 + 1;
                        while (l2 < grille.GetLength(0) && grille[l2,c].getValue() == 0)
                            l2 = l2 + 1;
                        if (l2 < grille.GetLength(0) && grille[l2,c].getValue() == grille[l1,c].getValue())
                        {
                            // identiques, on fusione
                            int k = 2 * grille[l1,c].getValue();
                            grille[l1,c].setValue(k);
                            grille[l1,c].setaEteFusione(true);
                            grille[l2,c].setValue(0);
                            ilYADecalageOuFusion = true;
                            setScore(getScore() + k);
                            if (getScore() > getBestScore())
                                setBestScore(getScore());
                        }
                    }
                }

                // pour chaque colonne, on cherche à decaler vers le haut
                for (int l1 = 0; l1 < grille.GetLength(0) - 1; l1++)
                {
                    if (grille[l1,c].getValue() == 0)
                    {
                        // on va combler le vide
                        int l2 = l1 + 1;
                        while (l2 < grille.GetLength(0) && grille[l2,c].getValue() == 0)
                        {
                            l2 = l2 + 1;
                        }

                        if (l2 < grille.GetLength(0) && grille[l2,c].getValue() != 0)
                        {
                            // on decale
                            grille[l1,c].setValue(grille[l2,c].getValue());
                            grille[l1,c].setaEteFusione(grille[l2,c].isaEteFusione());
                            grille[l2,c].setValue(0);
                            grille[l2,c].setaEteFusione(false);
                            ilYADecalageOuFusion = true;
                        }
                    }
                }
            }

            if (ilYADecalageOuFusion)
            {
                addRandomNum();
            }

            return ilYADecalageOuFusion;
        }

        /**
	 * Après avoir fait un décalage, il peut y avoir des fusions.
	 * La méthode vous retourne un tableau signalant les cases qui viennent d'être
	 * fusionnées.<br/>
	 * Cette méthode peut être utile si l'on souhaite par exemple faire "flasher" ou
	 * indiquer avec une couleur particulière les cases résultants d'une fusion lors
	 * du dernier décalage.
	 * @return un tableau de boolean.
	 */
        public boolean[,] tableauFusions()
        {
            int lignes = grille.GetLength(0);
            int cols = grille.GetLength(1);
            boolean[,] tab = new boolean[lignes,cols];
            for (int l = 0; l < lignes; l++)
            for (int c = 0; c < cols; c++)
                tab[l,c] = grille[l,c].isaEteFusione();
            return tab;
        }

        private void addRandomNum()
        {
            if (java.lang.Math.random() < 0.9)
                addNum(2);
            else
                addNum(4);
        }

        private void addNum(int num)
        {
            if (ilYaVides())
            {
                int place = (int) (java.lang.Math.random() * nbVides());
                int k = 0; // compteur nb vides
                int l = 0;
                while (l < nbLignes && k <= place)
                {
                    int c = 0;
                    while (c < nbCols && k <= place)
                    {
                        if (grille[l,c].getValue() == 0)
                        {
                            if (k == place)
                            {
                                grille[l,c].setValue(num);
                                setChanged();
                                notifyObservers();
                            }

                            k++;
                        }

                        c++;
                    }

                    l++;
                }
            }
        }

        private boolean ilYaVides()
        {
            return nbVides() > 0;
        }

        private int nbVides()
        {
            int vides = 0;
            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
                if (grille[l,c].getValue() == 0)
                    vides++;
            return vides;
        }

        /**
	 * Pour vérifier si le jeu est terminé.
	 * C'est à dire le joueur a gagné ou la grille est pleine et 
	 * il n'y a pas de fusion potentiel.
	 * @return true si le jeu est terminé.
	 */
        public boolean estTermine()
        {
            if (estVainquer()) return true;
            else if (ilYaVides()) return false;
            else if (ilYAFusionPotentiel()) return false;
            else return true;
        }

        /**
	 * Permets de vérifier s'il y a au moins une fusion
	 * possible.
	 * @return true s'il y a une fusion potentielle.
	 */
        public boolean ilYAFusionPotentiel()
        {
            for (int l = 1; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
                if (grille[l,c].getValue() == grille[l - 1,c].getValue())
                    return true;
            for (int l = 0; l < nbLignes; l++)
            for (int c = 1; c < nbCols; c++)
                if (grille[l,c].getValue() == grille[l,c - 1].getValue())
                    return true;
            return false;
        }

        /**
	 * Pour vérifier si le joueur a gagné.
	 * C'est à dire il a atteint nbBut (2048 par défaut).
	 * @return true si le joueur a gagné.
	 */
        public boolean estVainquer()
        {
            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
                if (grille[l,c].getValue() == nbBut)
                    return true;
            return false;
        }

        /**
	 * Nombre à atteindre pour gagner.
	 * @return entier
	 */
        public int getNbBut()
        {
            return nbBut;
        }

        /**
	 * Fixe le score à attendre pour gagner le jeu.
	 * @param nbBut : Entier nouveau score à attendre pour gagner le jeu
	 */
        public void setNbBut(int nbBut)
        {
            this.nbBut = nbBut;
        }

        /**
	 * Pour obtenir le grille sous la forme d'un tableau de String.
	 * @return String[,] tableau de chaines de caratères représentant les cases de la grille.
	 * @see #getGrilleInt()
	 */
        public String[,] getGrilleString()
        {
            String[,] t = new String[nbLignes,nbCols];
            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
                t[l,c] = grille[l,c].toString();
            return t;
        }

        /**
	 * Pour obtenir le grille sous la forme d'un tableau de int.
	 * @return int[,] tableau d'entiers représentant les cases de la grille.<br/>
	 * @see #getGrilleString()
	 * @see #setGrilleInt(int[,])
	 */
        public int[,] getGrilleInt()
        {
            int[,] t = new int[nbLignes,nbCols];
            for (int l = 0; l < nbLignes; l++)
            for (int c = 0; c < nbCols; c++)
                t[l,c] = grille[l,c].getValue();
            return t;
        }

        /**
	 * Nombre de lignes de la grille
	 * @return entier
	 */
        public int getNbLignes()
        {
            return nbLignes;
        }

        /**
	 * Nombre de colonnes de la grille
	 * @return entier
	 */
        public int getNbCols()
        {
            return nbCols;
        }

        /**
	 * Nombre de points accumulés depuis le début du jeu.
	 * @return entier
	 */
        public int getScore()
        {
            return score;
        }

        /**
	 * Meilleur score.
	 * @return entier
	 */
        public int getBestScore()
        {
            return bestScore;
        }

        /**
	 * Fixe le meilleur score. Utile pour restaurer une sauvegarde.
	 * @param bestScore : entier nouvelle valeur du meulleur score
	 */
        public void setBestScore(int bestScore)
        {
            this.bestScore = bestScore;
            setChanged();
            notifyObservers();
        }

        /**
	 * Fixe le score. Utile pour restaurer une sauvegarde.
	 * @param score : entier nouvelle valeur du score
	 */
        public void setScore(int score)
        {
            this.score = score;
            setChanged();
            notifyObservers();
        }
    }
}