using System;
using java.io;
using test_vjs.package2048;

namespace test_vjs
{
    public class ModelIO
    {
        /**
     * Constructeur. DessinUtils est une classe utilitaire. Cette fonction ne doit jamais être appelée.
     */
        private ModelIO()
        {
            //throw new AssertionError();
        }

        public static void enregistrer(String fn, Jeu2048 model)
        {
            var @out = new FileOutputStream(new File(fn));
            var buf = new BufferedOutputStream(@out);
            var ois = new ObjectOutputStream(buf);
            ois.writeObject(model);
        }

        public static Jeu2048 charger(String fn)
        {
            var inp = new FileInputStream(new File(fn));
            var buf = new BufferedInputStream(inp);
            var ois = new ObjectInputStream(buf);
            return (Jeu2048) ois.readObject();
        }
    }
}