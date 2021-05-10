using System;
using java.io;
using java.util;
using test_vjs.package2048;

namespace test_vjs
{
    class ModelProxy : Serializable
    {
        private Jeu2048 model;

        public ModelProxy(Jeu2048 model)
        {
            this.model = model;
        }

        public Jeu2048 getBackend()
        {
            return model;
        }

        public void decaler(int direction)
        {
            Logger.INSTANCE.log(Logger.DEBUG,
                "Décalage " + ((direction >= 0 && direction < 4)
                    ? new String[] { "HAUT", "DROITE", "BAS", "GAUCHE" }[direction]
                    : direction.ToString()));

            if (model.decaler(direction))
            {
                var fus = model.tableauFusions();
                var gri = model.getGrilleInt();

                for (var y = 0; y < model.getNbLignes(); y++)
                for (var x = 0; x < model.getNbCols(); x++)
                    if (fus[y,x])
                        Logger.INSTANCE.log(Logger.INFO,
                            gri[y,x] + " résultat d'une fusion dans (" + x + ", " + y + ")");
            }
            else
            {
                Logger.INSTANCE.log(Logger.IMPORTANT, "Décalage impossible !");
            }
        }

        public void nouveauJeu()
        {
            Logger.INSTANCE.log(Logger.DEBUG,
                "Nouvelle partie - " + model.getNbLignes() + " lignes par " + model.getNbCols() +
                " colonnes ; objectif = " + model.getNbBut());
            model.nouveauJeu();
        }

        public void addObserver(Observer o)
        {
            model.addObserver(o);
        }

        public int getBestScore()
        {
            return model.getBestScore();
        }

        public int getScore()
        {
            return model.getScore();
        }

        public int[,] getGrilleInt()
        {
            return model.getGrilleInt();
        }

        public int getNbLignes()
        {
            return model.getNbLignes();
        }

        public int getNbCols()
        {
            return model.getNbCols();
        }

        public bool estTermine()
        {
            return model.estTermine();
        }

        public bool estVainquer()
        {
            return model.estVainquer();
        }
    }
}