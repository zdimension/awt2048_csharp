using java.lang;
using System;
using com.ms.vjsharp.lang;
using java.io;

namespace test_vjs
{
    public class Logger
    {
        /**
     * Tous les messages
     */
        private const int ALL = 0;

        /**
     * Messages de débogage
     */
        public const int DEBUG = 100;

        /**
     * Informations pertinentes
     */
        public const int INFO = 500;

        /**
     * Informations importantes
     */
        public const int IMPORTANT = 900;

        /**
     * Rien
     */
        private const int OFF = Integer.MAX_VALUE;

        private int level;
        private PrintWriter writer;

        /**
     * Crée une instance de {@link Logger} avec le niveau {@link Logger#ALL} dans le flux {@link System#out}
     */
        private Logger() : this(ALL, java.lang.System.@out)
        {
        }

        /**
     * Crée une instance de {@link Logger} avec le niveau et le flux spécifiés
     * @param level niveau de journalisation
     * @param str flux dans lequel écrire
     */
        private Logger(int level, OutputStream str) : this(level, new PrintWriter(str, true))
        {
        }

        /**
     * Crée une instance de {@link Logger} avec le niveau et le {@link PrintWriter} spécifiés
     * @param level niveau de journalisation
     * @param writer {@link PrintWriter} dans lequel écrire
     */
        private Logger(int level, PrintWriter writer)
        {
            this.level = level;
            this.writer = writer;
        }

        /**
     * Crée une instance de {@link Logger} à l'aide du fichier de configuration spécifié
     * @param configPath chemin relatif ou absolu du fichier de configuration à charger
     * @throws FileNotFoundException si le fichier n'est pas trouvé
     * @throws IOException si une erreur survient durant la lecture
     * @throws IllegalArgumentException si le fichier de configuration est invalide
     */
        private Logger(String configPath)
        {
            var rd = new BufferedReader(new FileReader(configPath));
            {
                var conf = rd.readLine().Split(' ');

                switch (conf[0].Trim().ToUpper())
                {
                    case "ALL":
                        this.level = ALL;
                        break;
                    case "DEBUG":
                        this.level = DEBUG;
                        break;
                    case "INFO":
                        this.level = INFO;
                        break;
                    case "IMPORTANT":
                        this.level = IMPORTANT;
                        break;
                    case "OFF":
                        this.level = OFF;
                        break;
                    default:
                        throw new IllegalArgumentException("Valeur invalide pour le niveau de débogage : " + conf[0]);
                }

                OutputStream str;

                switch (conf[1].Trim())
                {
                    case "System.out":
                        str = java.lang.System.@out;
                        break;
                    case "System.err":
                        str = java.lang.System.@err;
                        break;
                    default:
                        str = new BufferedOutputStream(new FileOutputStream(conf[1]));
                        break;
                }

                this.writer = new PrintWriter(str, true);
            }
        }

        public int getLevel()
        {
            return level;
        }

        public void setLevel(int level)
        {
            this.level = level;
        }

        public PrintWriter getWriter()
        {
            return writer;
        }

        public void setWriter(PrintWriter writer)
        {
            this.writer = writer;
        }

        /**
     * Écrit un message dans le journal
     *
     * @param level niveau de journalisation
     * @param message message à écrire
     */
        public void log(int level, String message)
        {
            if (level >= this.level)
            {
                writer.println(message);
            }
        }

        public static readonly Logger INSTANCE;

        static Logger()
        {
            Logger res;

            try
            {
                res = new Logger("config.txt");
            }
            catch (IOException e)
            {
                java.lang.System.err.println(
                    "ERREUR : la configuration du Logger n'a pu être chargée. Retour à la configuration par défaut.");
                res = new Logger();
            }

            INSTANCE = res;
        }
    }
}