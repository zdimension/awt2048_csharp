using System;
using java.awt;

namespace test_vjs
{
    public class DessinUtils
    {
        /**
     * Constructeur. DessinUtils est une classe utilitaire. Cette fonction ne doit jamais être appelée.
     */
        private DessinUtils()
        {
            //throw new AssertionError();
        }

        /**
     * Dessine une chaîne alignée au centre du rectangle spécifié
     *
     * @param g objet {@link Graphics} sur lequel dessiner
     * @param s chaîne à afficher
     * @param x position X du rectangle
     * @param y position Y du rectangle
     * @param w largeur du rectangle
     * @param h hauteur du rectangle
     */
        public static void drawStringCentered(Graphics g, String s, int x, int y, int w, int h)
        {
            var metrics = g.getFontMetrics();

            g.drawString(s,
                x + (w - metrics.stringWidth(s)) / 2,
                y + (h - metrics.getHeight()) / 2 + metrics.getAscent());
        }
    }
}