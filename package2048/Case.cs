using System;
using java.io;
using java.lang;
using boolean = System.Boolean;

namespace test_vjs.package2048
{
    public class Case : Serializable
    {
        /**
	 * 
	 */
        private const long serialVersionUID = -145130772363511365L;

        private int value;
        private boolean aEteFusione;

        public boolean isaEteFusione()
        {
            return aEteFusione;
        }

        public void setaEteFusione(boolean aEteModifie)
        {
            this.aEteFusione = aEteModifie;
        }

        public int getValue()
        {
            return value;
        }

        public void setValue(int value)
        {
            this.value = value;
        }

        public Case(int value)
        {
            this.value = value;
            this.aEteFusione = false;
        }

        public String toString()
        {
            return new Integer(value).ToString();
        }
    }
}