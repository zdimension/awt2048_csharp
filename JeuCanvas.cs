using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using com.ms.vjsharp.windowing.win32;
using java.awt;
using java.awt.@event;
using java.lang;
using java.util;
using test_vjs.package2048;
using Exception = System.Exception;
using List = java.awt.List;
using Math = System.Math;

namespace test_vjs
{
    /**
 * Zone d'affichage du jeu
 */
class JeuCanvas : Canvas
{
    /**
     * Couleurs de fond des différentes valeurs de case
     */
    private static readonly Color[] COULEURS = {
        new Color(205, 193, 180), // 0
        new Color(238, 228, 218), // 2
        new Color(237, 224, 200), // 4
        new Color(242, 177, 121), // 8
        new Color(245, 149, 99), // 16
        new Color(246, 124, 95), // 32
        new Color(246, 94, 59), // 64
        new Color(237, 207, 114), // 128
        new Color(237, 204, 97), // 256
        new Color(237, 200, 80), // 512
        new Color(237, 197, 63), // 1024
        new Color(237, 194, 46), // 2048
        new Color(244, 102, 116), // 4096
        new Color(243, 75, 92), // 8192
        new Color(235, 67, 54), // 16384
        new Color(115, 180, 220), // 32768
        new Color(94, 162, 227), // 65536
        new Color(0, 127, 194) // 131072
    };
    /**
     * Couleurs de texte des différentes valeurs de case
     */
    private static readonly Color[] COULEURS_TEXTE = {
        new Color(119, 110, 101), // 0, 2, 4
        new Color(249, 246, 242), // 8, 16, 32, 64, 128, 256, 512, 1024, 2048
    };
    /**
     * Marge générale (sur l'ensemble de la fenêtre)
     */
    private static readonly int MARGE_EXTERIEURE = 10;
    /**
     * Couleur de fond de la fenêtre
     */
    private static readonly Color COULEUR_FOND = new Color(250, 248, 239);
    /**
     * Couleur de fond de la grille de jeu
     */
    private static readonly Color COULEUR_FOND_GRILLE = new Color(187, 173, 160);
    /**
     * Fenêtre parent
     */
    private readonly Frame parent;
    /**
     * Modèle de jeu
     */
    private ModelProxy model;
    /**
     * Taille des cases d'affichage de score
     */
    private static readonly Dimension SCOREBOX_SIZE = new Dimension(100, 62);
    /**
     * Taille verticale des boutons
     */
    private static readonly int BUTTON_HEIGHT = 40;
    /**
     * Taille verticale des boutons secondaires
     */
    private static readonly int BUTTON_HEIGHT_SEC = BUTTON_HEIGHT * 3 / 4;
    /**
     * Hauteur de l'en-tête de la fenêtre
     */
    private static readonly int HAUTEUR_ENTETE = MARGE_EXTERIEURE * 3 + SCOREBOX_SIZE.height + BUTTON_HEIGHT + BUTTON_HEIGHT_SEC;
    /**
     * Image virtuelle de dessin
     */
    private Image dbImage;
    /**
     * Objet <code>Graphics</code> correspond à {@link #dbImage l'image de dessin}
     */
    private Graphics dbGraphics;

    private readonly List<JeuCanvasBouton> boutons;

    private class test1 : Observer
    {
        private JeuCanvas canvas;

        public test1(JeuCanvas canvas)
        {
            this.canvas = canvas;
        }
        
        public void update(Observable o, object arg)
        {
            canvas.repaint();

            // auto-enregistrement
            try
            {
                ModelIO.enregistrer("_auto.sav", canvas.model.getBackend());
            }
            catch (Exception e)
            {
                Logger.INSTANCE.log(Logger.DEBUG, "Une erreur est survenue durant l'enregistrement automatique.");
            }
        }
    }

    public void setModel(Jeu2048 model)
    {
        this.model = new ModelProxy(model);

        // mise à jour de la zone de dessin à chaque changement du jeu
        //noinspection deprecation
        this.model.addObserver(new test1(this));
    }

    public class key : KeyAdapter
    {
        private JeuCanvas canvas;

        public key(JeuCanvas canvas)
        {
            this.canvas = canvas;
        }
        
        public override void keyPressed(KeyEvent e)
        {
            Logger.INSTANCE.log(Logger.DEBUG, "Appui sur touche clavier " + KeyEvent.getKeyText(e.getKeyCode()));

            switch (e.getKeyCode())
            {
                case KeyEvent.VK_UP:
                    canvas.model.decaler(Jeu2048.HAUT);
                    break;

                case KeyEvent.VK_DOWN:
                    canvas.model.decaler(Jeu2048.BAS);
                    break;

                case KeyEvent.VK_LEFT:
                    canvas.model.decaler(Jeu2048.GAUCHE);
                    break;

                case KeyEvent.VK_RIGHT:
                    canvas.model.decaler(Jeu2048.DROITE);
                    break;

                case KeyEvent.VK_R:
                    canvas.model.nouveauJeu();
                    break;

                default:
                    base.keyPressed(e);
                    break;
            }
        }
    }

    public class mousemotion : MouseMotionAdapter
    {
        private JeuCanvas canvas;

        public mousemotion(JeuCanvas canvas)
        {
            this.canvas = canvas;
        }
        
        public override void mouseMoved(MouseEvent e)
        {
            base.mouseMoved(e);

            foreach (JeuCanvasBouton btn in canvas.boutons)
            {
                btn.setButtonInfo(e, false);
            }
        }
    }

    public class mouse : MouseAdapter
    {
        private JeuCanvas canvas;

        public mouse(JeuCanvas canvas)
        {
            this.canvas = canvas;
        }
        
        public override void mousePressed(MouseEvent e)
        {
            base.mousePressed(e);

            foreach (JeuCanvasBouton btn in canvas.boutons)
            {
                btn.setButtonInfo(e, false);
            }
        }

        public override void mouseReleased(MouseEvent e)
        {
            base.mouseReleased(e);

            foreach (JeuCanvasBouton btn in canvas.boutons)
            {
                btn.setButtonInfo(e, true);
            }
        }
    }
    
    /**
     * Initialise la zone d'affichage
     * @param parent fenêtre parent
     * @param model modèle de 2048 à utiliser
     */
    public JeuCanvas(Frame parent, Jeu2048 model)
    {
        this.parent = parent;
        this.setModel(model);

        addKeyListener(new key(this));

        // gestion des déplacements de la souris
        addMouseMotionListener(new mousemotion(this));

        // gestion des clics de souris
        addMouseListener(new mouse(this));

        int x = -2 * MARGE_EXTERIEURE - 2 * SCOREBOX_SIZE.width;
        int y = 2 * MARGE_EXTERIEURE + SCOREBOX_SIZE.height;

        boutons = new List<JeuCanvasBouton>
        {
            new JeuCanvasBouton(
                "RECOMMENCER",
                new Point(x, y),
                new Dimension(MARGE_EXTERIEURE + 2 * SCOREBOX_SIZE.width, BUTTON_HEIGHT),
                this,
                () => this.model.nouveauJeu()),

            new JeuCanvasBouton(
                "CHARGER",
                new Point(x, y + BUTTON_HEIGHT + MARGE_EXTERIEURE),
                new Dimension(SCOREBOX_SIZE.width, BUTTON_HEIGHT * 3 / 4),
                this,
                this.chargerPartie),

            new JeuCanvasBouton(
                "SAUVER",
                new Point(x + SCOREBOX_SIZE.width + MARGE_EXTERIEURE, y + BUTTON_HEIGHT + MARGE_EXTERIEURE),
                new Dimension(SCOREBOX_SIZE.width, BUTTON_HEIGHT_SEC),
                this,
                this.sauverPartie)
        };
    }

    /**
     * Affiche la fenêtre de chargement de fichier de partie
     */
    private void chargerPartie()
    {
        Logger.INSTANCE.log(Logger.DEBUG, "Clic sur bouton 'Charger partie'");

        try
        {
            var fd = new FileDialog(parent, "Charger une partie", FileDialog.LOAD);
            fd.setFile("*.sav");
            fd.setVisible(true);

            if (fd.getDirectory() != null && fd.getFile() != null)
            {
                var best = model.getBestScore();
                /*var read = ModelIO.charger(new File(new File(fd.getDirectory()), fd.getFile()).getAbsolutePath());
                read.setBestScore(best);
                this.setModel(read);

                repaint();*/
            }
        }
        catch (Exception exc)
        {
            Logger.INSTANCE.log(Logger.IMPORTANT, "Erreur lors du chargement : " + exc.ToString());
        }
    }

    /**
     * Affiche la fenêtre d'enregistrement de partie
     */
    private void sauverPartie()
    {
        Logger.INSTANCE.log(Logger.DEBUG, "Clic sur bouton 'Sauver partie'");

        try
        {
            var fd = new FileDialog(parent, "Sauvegarder la partie", FileDialog.SAVE);
            fd.setFile("*.sav");
            fd.setVisible(true);

            if (fd.getDirectory() != null && fd.getFile() != null)
            {
               /* ModelIO.enregistrer(
                    new File(new File(fd.getDirectory()), fd.getFile()).getAbsolutePath(),
                    this.model.getBackend());*/
            }
        }
        catch (Exception exc)
        {
            Logger.INSTANCE.log(Logger.IMPORTANT, "Erreur lors de l'enregistrement : " + exc.ToString());
        }
    }

    public void dispose()
    {
        //Score.sauverScore(this.model.getBestScore());
    }

    /**
     * Renvoie la couleur associée à la valeur
     *
     * @param valeur valeur de la case
     * @return couleur de fond de la case
     */
    private static Color getColor(int valeur)
    {
        return COULEURS[valeur == 0 ? 0 : (int) java.lang.Math.floor(java.lang.Math.log(valeur) / java.lang.Math.log(2))];
    }

    /**
     * Renvoie la couleur de texte associée à la valeur
     *
     * @param valeur valeur de la case
     * @return couleur de texte de la case
     */
    private static Color getTextColor(int valeur)
    {
        return COULEURS_TEXTE[valeur < 8 ? 0 : 1];
    }

    /**
     * Dessine une case d'affichage de score
     *
     * @param g      objet {@link Graphics} sur lequel dessiner
     * @param x      position X de la case
     * @param header en-tête de la case
     * @param val    valeur numérique à afficher
     */
    private void drawScoreBox(Graphics g, int x, String header, int val)
    {
        // fond de la case
        g.setColor(COULEUR_FOND_GRILLE);
        g.fillRoundRect(x, MARGE_EXTERIEURE, SCOREBOX_SIZE.width, SCOREBOX_SIZE.height, 10, 10);

        // en-tête
        g.setFont(new Font("Arial", Font.BOLD, 16));
        g.setColor(COULEURS[1]);
        DessinUtils.drawStringCentered(g, header, x, MARGE_EXTERIEURE - SCOREBOX_SIZE.height / 3, SCOREBOX_SIZE.width, SCOREBOX_SIZE.height);

        // valeur
        g.setFont(new Font("Arial", Font.BOLD, 36));
        g.setColor(Color.white);
        DessinUtils.drawStringCentered(g, Integer.toString(val), x, MARGE_EXTERIEURE + SCOREBOX_SIZE.height / 6, SCOREBOX_SIZE.width, SCOREBOX_SIZE.height);
    }

    /**
     * Renvoie la position la plus à droite de la zone affichable
     *
     * @return coordonnée X de la droite de l'écran moins la {@link #MARGE_EXTERIEURE marge extérieure}
     */
    private int rightPos()
    {
        return this.getWidth() - MARGE_EXTERIEURE;
    }

    /**
     * Dessine l'en-tête de la fenêtre de jeu
     *
     * @param g objet {@link Graphics} sur lequel dessiner
     */
    private void drawHeader(Graphics g)
    {
        // logo du jeu
        g.setColor(COULEURS_TEXTE[0]);
        g.setFont(new Font("Arial", Font.BOLD, 96));
        g.drawString("2048", 15, 110);

        // cases d'affichage du score et du meilleur score
        var xb = rightPos();
        drawScoreBox(g, xb - SCOREBOX_SIZE.width, "MEILLEUR", model.getBestScore());
        drawScoreBox(g, xb - MARGE_EXTERIEURE - 2 * SCOREBOX_SIZE.width, "SCORE", model.getScore());

        foreach (JeuCanvasBouton btn in boutons)
        {
            btn.draw(g);
        }
    }

    /**
     * Appelle la {@link #paintBuffer(Graphics) fonction principale de dessin} et met à jour l'image réelle à partir de la mémoire tampon
     *
     * @param g objet {@link Graphics} sur lequel dessiner
     */
    public override void paint(Graphics g)
    {
        // met à jour l'image virtuelle et la recrée si besoin
        if (dbImage == null ||
            this.getWidth() != dbImage.getWidth(this)
            || this.getHeight() != dbImage.getHeight(this))
        {
            if (dbGraphics != null)
                dbGraphics.dispose();

            if (dbImage != null)
                dbImage.flush();

            java.lang.System.gc();

            dbImage = (Image)typeof(SwingHelper).Assembly.GetType("com.ms.vjsharp.windowing.win32.Win32Image").GetConstructors()[4].Invoke(new object[] { this.getWidth(), this.getHeight(), 24, this });
            dbGraphics = dbImage.getGraphics();
        }

        // met à jour l'affichage réel
        if (dbGraphics != null)
        {
            dbGraphics.clearRect(0, 0, dbImage.getWidth(this), dbImage.getHeight(this));

            paintBuffer(dbGraphics);

            g.drawImage(dbImage, 0, 0, this);
        }
    }

    /**
     * Dessine les différents composants du jeu
     *
     * @param g objet {@link Graphics} sur lequel dessiner
     */
    private void paintBuffer(Graphics g)
    {
        // paramètres d'affichage (anticrénelage)
        /*var g2d = (Graphics2D) g;
        g2d.setRenderingHint(
            RenderingHints.KEY_ANTIALIASING,
            RenderingHints.VALUE_ANTIALIAS_ON);
        g2d.setRenderingHint(
            RenderingHints.KEY_TEXT_ANTIALIASING,
            RenderingHints.VALUE_TEXT_ANTIALIAS_ON);
        g2d.setRenderingHint(
            RenderingHints.KEY_RENDERING,
            RenderingHints.VALUE_RENDER_QUALITY);
        g2d.setRenderingHint(
            RenderingHints.KEY_STROKE_CONTROL,
            RenderingHints.VALUE_STROKE_NORMALIZE);*/

        // effacement de l'écran
        g.setColor(COULEUR_FOND);
        g.fillRect(0, 0, this.getWidth(), this.getHeight());

        drawHeader(g);

        // position de la zone de dessin de la grille du jeu
        var yOff = HAUTEUR_ENTETE;

        drawGrid(g, yOff);

        drawOverlay(g, yOff);
    }

    /**
     * Dessine la grille du jeu
     *
     * @param g    objet {@link Graphics} sur lequel dessiner
     * @param yOff position verticale
     */
    private void drawGrid(Graphics g, int yOff)
    {
        // taille de la zone de dessin de la grille
        var width = this.getWidth() - MARGE_EXTERIEURE * 2;
        var height = this.getHeight() - yOff - MARGE_EXTERIEURE * 2;

        // marge intérieure de la grille
        var marginIn = java.lang.Math.min(width, height) / 32;

        // fond de la grille
        g.setColor(COULEUR_FOND_GRILLE);
        g.fillRoundRect(MARGE_EXTERIEURE, yOff + MARGE_EXTERIEURE, width, height, 15, 15);

        // grille du jeu
        var grille = model.getGrilleInt();

        // taille de la zone de dessin d'une case
        var wc = (width - marginIn * 2) / model.getNbCols();
        var hc = (height - marginIn * 2) / model.getNbLignes();

        // espacement entre les cases
        var spc = java.lang.Math.min(wc, hc) / 2;

        // taille effective d'une case (en prenant en compte l'espacement)
        var w = wc - spc / 2;
        var h = hc - spc / 2;

        for (var i = 0; i < model.getNbLignes(); i++)
        {
            for (var j = 0; j < model.getNbCols(); j++)
            {
                var valeur = grille[i,j];

                // position de la case
                var x = MARGE_EXTERIEURE + marginIn + j * wc + spc / 4;
                var y = MARGE_EXTERIEURE + marginIn + i * hc + spc / 4;

                // fond de la case
                g.setColor(getColor(valeur));
                g.fillRoundRect(x, yOff + y, w, h, 10, 10);

                if (valeur != 0)
                {
                    // texte de la case
                    // la taille dépend de la valeur
                    // car les grandes valeurs prennent plus de place et doivent donc être écrites plus petites
                    int taille = (int) (java.lang.Math.pow(0.8, java.lang.Math.floor(Math.Log10(valeur))) * java.lang.Math.min(wc, hc) / 1.75);
                    g.setFont(new Font("Arial", Font.BOLD, taille));
                    g.setColor(getTextColor(valeur));
                    DessinUtils.drawStringCentered(g, Integer.toString(valeur), x, yOff + y, w, h);
                }
            }
        }
    }

    /**
     * Dessine le voile par dessus la grille contenant les messages de fin de partie
     *
     * @param g    objet {@link Graphics} sur lequel dessiner
     * @param yOff position verticale
     */
    private void drawOverlay(Graphics g, int yOff)
    {
        if (model.estTermine())
        {
            // fond du voile (translucide)
            //g.setColor(new Color(COULEUR_FOND.getRed(), COULEUR_FOND.getGreen(), COULEUR_FOND.getBlue(), 140));
            //g.fillRect(0, yOff, getWidth(), getHeight() - yOff);

            // message de fin de partie
            g.setColor(COULEURS_TEXTE[0]);
            g.setFont(new Font("Arial", Font.BOLD, 32));
            DessinUtils.drawStringCentered(g, model.estVainquer() ? "Victoire" : "Échec cuisant", 0, yOff, getWidth(), getHeight() - yOff);
        }
    }
}

}