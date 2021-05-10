using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.awt;
using java.awt.@event;
using test_vjs.package2048;

namespace test_vjs
{
    public class Program : Frame
    {
        private JeuCanvas _grille;

        // Paramètres de la partie
        private const int NB_LIG = 4;
        private const int NB_COL = 4;
        private const int NB_BUT = 2048;

        /**
     * Initialise la fenêtre principale
     *
     * @param model modèle de 2048 à utiliser
     */
        public Program(Jeu2048 model)
        {
            // paramètres graphiques
            setTitle("2048 - INFO403 - Tom Niget");
            setLayout(new BorderLayout());
            setSize(500, 680);

            // zone de dessin
            _grille = new JeuCanvas(this, model);
            add(_grille, BorderLayout.CENTER);

            // fermeture de la fenêtre
            addWindowListener(new window(this));
    
            // activation des évènements d'entrée clavier
            /*grille.requestFocusInWindow();
            grille.setFocusable(true);*/
        }

        public class window : WindowAdapter
        {
            private Program program;

            public window(Program program)
            {
                this.program = program;
            }
            
            public override void windowClosing(WindowEvent e)
            {
                Logger.INSTANCE.log(Logger.INFO, "Fermeture de la fenêtre et enregistrement du score");
                program._grille.dispose();
                program.dispose();
            }
        }

        public static void Main(String[] args)
        {
            Logger.INSTANCE.log(Logger.INFO, "Démarrage");

            // évite le scintillement
            //setProperty("sun.awt.noerasebackground", "true");

            Jeu2048 model;

            try
            {
                model = ModelIO.charger("_auto.sav");
            }
            catch (Exception e)
            {
                // initialisation d'une nouvelle partie
                model = new Jeu2048(NB_LIG, NB_COL, NB_BUT);
                model.nouveauJeu();
            }

            new Program(model).setVisible(true);

            Logger.INSTANCE.log(Logger.INFO, "Fin");
        }
    }
}