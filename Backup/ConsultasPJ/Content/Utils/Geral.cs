using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
//using Oracle.DataAccess.Client;

namespace ConsultasPJ.Content.Utils
{
    public class Geral
    {
        public static void AbreConexaoOracle(OracleConnection banco)
        {
            try
            {
                banco = new OracleConnection
                {
                    ConnectionString =
                        "Data Source=profdev; Persist Security Info=True; User ID=profarma; Password=forp2002; Unicode=True; Connection Lifetime=180; Max Pool Size=50"
                };
                banco.Open();
            }
            catch (Exception)
            {
                if (banco.State != 0)
                {
                    banco.Close();
                }
            }
        }

        public static void FechaConexaoOracle(OracleConnection banco)
        {
            banco.Close();
            banco.Dispose();
        }

        public partial class ClientePJ
        {
            public string Documento { get; set; }
            public string RazaoSocial { get; set; }
            public string NomeFantasia { get; set; }
            public string DataFundacao { get; set; }
            public string CodigoNaturezaJuridica { get; set; }
            public string SituacaoRFB { get; set; }
            public string DataConsultaRFB { get; set; }
            public string EnderecoTipo { get; set; }
            public string Logradouro { get; set; }
            public string Numero { get; set; }
            public string Complemento { get; set; }
            public string Cidade { get; set; }
            public string Estado { get; set; }
            public string Bairro { get; set; }
            public string CEP { get; set; }
            public string Telefone { get; set; }
            public string InscricaoEstadual { get; set; }
            public string CodigoAtividadeEconomica { get; set; }

        }
    }
}