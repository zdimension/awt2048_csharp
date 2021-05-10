using java.awt;
using java.lang;
using System;
using java.awt.@event;
using boolean = System.Boolean;
namespace test_vjs
{
    /**
 * Bouton cliquable affiché à l'écran sur un {@link JeuCanvas}
 */
public class JeuCanvasBouton
{
    /**
     * Texte
     */
    private String text;

    /**
     * Position sur l'écran
     */
    private Point position;

    /**
     * Taille
     */
    private Dimension size;

    /**
     * Composant parent
     */
    private Component parent;

    /**
     * Fonction à appeler lors du clic
     */
    private Action handler;

    /**
     * Drapeau de survol du bouton
     */
    private boolean hover = false;

    /**
     * Drapeau de pression de souris
     */
    private boolean down = false;

    /**
     * Crée un bouton.
     * @param text texte à afficher
     * @param position position sur l'écran
     * @param size taille sur l'écran
     * @param parent composant parent
     * @param handler fonction à appeler lors du clic
     */
    public JeuCanvasBouton(String text, Point position, Dimension size, Component parent, Action handler)
    {
        this.text = text;
        this.position = position;
        this.size = size;
        this.parent = parent;
        this.handler = handler;
    }

    public String getText()
    {
        return text;
    }

    public void setText(String text)
    {
        this.text = text;
    }

    public Point getPosition()
    {
        // corrige les positions négatives en rebouclant
        int x = position.x;
        int y = position.y;

        if (x < 0)
            x += parent.getWidth();

        if (y < 0)
            y += parent.getHeight();

        return new Point(x, y);
    }

    public void setPosition(Point position)
    {
        this.position = position;
    }

    public Dimension getSize()
    {
        return size;
    }

    public void setSize(Dimension size)
    {
        this.size = size;
    }

    public Component getParent()
    {
        return parent;
    }

    public void setParent(Component parent)
    {
        this.parent = parent;
    }

    public Action getHandler()
    {
        return handler;
    }

    public void setHandler(Action handler)
    {
        this.handler = handler;
    }

    /**
     * Détermine si la position spécifiée se trouve sur le bouton
     *
     * @param x position X du curseur
     * @param y position Y du curseur
     * @return <code>true</code> si le curseur se trouve au-dessus du bouton, <code>false</code> sinon
     */
    private boolean inButton(int x, int y)
    {
        Point pos = getPosition();
        return x > pos.x && x < pos.x + size.width && y > pos.y && y < pos.y + size.height;
    }

    /**
     * Met à jour les drapeaux de statut du bouton
     *
     * @param ev       évènement souris
     * @param released drapeau spécifiant si le bouton de souris a été relâché
     */
    public void setButtonInfo(MouseEvent ev, boolean released)
    {
        var old = hover;
        var old2 = down;

        // vérifie si le curseur se trouve dans la zone du bouton
        hover = inButton(ev.getX(), ev.getY());

        // si la souris n'y est pas, ou si le bouton a été relâche
        // alors le bouton n'est plus enfoncé
        if (!hover || released)
            down = false;
        else
            down = (ev.getModifiers() & MouseEvent.BUTTON1_MASK) != 0;

        // si le bouton n'était pas enfoncé, mais l'est à présent
        // -> un clic a été effectué
        // -> nouvelle partie
        if (old2 && !down && hover)
            handler.Invoke();

        // si il y a eu changement d'état du bouton
        // -> actualisation de la zone de dessin
        if (old != hover || old2 != down)
            parent.repaint();
    }

    /**
     * Dessine le bouton
     * @param g cible de rendu
     */
    public void draw(Graphics g)
    {
        Point pos = getPosition();

        // fond du bouton
        g.setColor(hover
            ? (down ? new Color(100, 86, 72) : new Color(172, 147, 123))
            : new Color(143, 122, 102));
        g.fillRoundRect(pos.x, pos.y, size.width, size.height, 10, 10);

        // texte du bouton
        g.setFont(new Font("Arial", Font.BOLD, 18 * size.height / 40));
        g.setColor(Color.white);
        DessinUtils.drawStringCentered(g, text, pos.x, pos.y, size.width, size.height);
    }
}
}