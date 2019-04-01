using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Web.Mvc;
using ConsultasPJ.homolog.br.com.soawebservices.www;
using Microsoft.Ajax.Utilities;
//using Oracle.DataAccess.Client;
using ConsultasPJ.Content.Utils;

namespace ConsultasPJ.Controllers
{
    public class NFEPJController : Controller
    {
        //
        // GET: /NFEPJ/
        protected OracleConnection banco;

        [HttpGet]
        public ActionResult Homologacao(string cnpj = "99.999.999/9999-62")
        {
            if (string.IsNullOrEmpty(cnpj))
                Response.Write("Null");
            var sql = string.Format("select * from ad_base_pj where cnpj = '{0}' and (trunc(sysdate) - TO_DATE(DATACONSULTARFB)) <= 30", cnpj);
            var dt = new DataTable();

            try
            {
                //banco = new OracleConnection
                //{
                    //ConnectionString =
                    //  "Data Source=profdev; Persist Security Info=True; User ID=profarma; Password=forp2002; Unicode=True; Connection Lifetime=180; Max Pool Size=50"

                    //"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=yourhostname )(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XE)));User Id=system ;Password=yourpasswrd"
                    //ConnectionString = "Data Source=PROFDEV; User Id=profarma; Password=forp2002"
                //};

                //banco.Open();
                //var adapter = new OracleDataAdapter(sql, banco);
                //adapter.Fill(dt);

                if (dt.Rows.Count <= 0)
                {
                    var ws = new CDC();
                    var credenciais = new Credenciais
                    {
                        Email = ConfigurationManager.AppSettings["UsuarioWS"],
                        Senha = ConfigurationManager.AppSettings["SenhaWS"]
                    };

                    System.Net.ServicePointManager.Expect100Continue = false;

                    var resultadoBusca = ws.PessoaJuridicaEstendida(credenciais, cnpj);

                    if (resultadoBusca != null)
                    {
                        var resultClient = new Geral.ClientePJ
                        {
                            Documento = resultadoBusca.Documento,
                            RazaoSocial = resultadoBusca.RazaoSocial,
                            NomeFantasia = resultadoBusca.NomeFantasia,
                            DataFundacao = resultadoBusca.DataFundacao.ToString().Substring(0, 10),
                            CodigoNaturezaJuridica = resultadoBusca.CodigoNaturezaJuridica,
                            SituacaoRFB = resultadoBusca.SituacaoRFB,
                            DataConsultaRFB = resultadoBusca.DataConsultaRFB.ToString().Substring(0, 10),
                            EnderecoTipo = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Tipo.ToString() : "",
                            Logradouro = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Logradouro.ToString() : "",
                            Numero = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Numero.ToString() : "",
                            Complemento = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Complemento.ToString() : "",
                            Bairro = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Bairro.ToString() : "",
                            Cidade = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Cidade.ToString() : "",
                            Estado = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].Estado.ToString() : "",
                            CEP = resultadoBusca.Enderecos != null ? resultadoBusca.Enderecos[0].CEP.ToString() : "",
                            Telefone = resultadoBusca.Telefones != null ? resultadoBusca.Telefones[0].Numero.ToString() : "",
                            CodigoAtividadeEconomica = resultadoBusca.CodigoAtividadeEconomica,
                            InscricaoEstadual = resultadoBusca.InscricaoEstadual
                        };

                        sql =
                        string.Format(
                            "insert into ad_base_pj(CNPJ, RazaoSocial, NomeFantasia, DataFundacao, CodigoNaturezaJuridica, SituacaoRFB, DataConsultaRFB, EnderecoTipo, Logradouro, Numero, Complemento, Bairro, Cidade, Estado, CEP, Telefone, CodigoAtividadeEconomica, InscricaoEstadual) values('{0}','{1}','{2}', '{3}','{4}','{5}', '{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                            resultClient.Documento, resultClient.RazaoSocial, resultClient.NomeFantasia,
                            resultClient.DataFundacao.Substring(0, 10), resultClient.CodigoNaturezaJuridica, resultClient.SituacaoRFB,
                            resultClient.DataConsultaRFB.Substring(0, 10), resultClient.EnderecoTipo ?? "",
                            resultClient.Logradouro ?? "", resultClient.Numero ?? "",
                            resultClient.Complemento ?? "", resultClient.Bairro ?? "",
                            resultClient.Cidade ?? "", resultClient.Estado ?? "",
                            resultClient.CEP ?? "", resultClient.Numero, resultClient.CodigoAtividadeEconomica, resultClient.InscricaoEstadual);

                        return Json(new JsonResponseModel { Success = true, Message = sql, Telefone = resultadoBusca.Telefones[0].Numero, Inscricao = resultadoBusca.InscricaoEstadual }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new JsonResponseModel { Success = false, Message = sql, Telefone = "", Inscricao = "" }, JsonRequestBehavior.AllowGet);
                    }

                    //Geral.AbreConexaoOracle(banco);
                    // var cmd = new OracleCommand(sql, banco);
                    //Geral.FechaConexaoOracle(banco);
                }
                else
                {
                    return Json(new JsonResponseModel { Success = true, Message = "Ok", Telefone = dt.Rows[0]["telefone"].ToString(), Inscricao = dt.Rows[0]["InscricaoEstadual"].ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonResponseModel { Success = false, Message = ex.ToString(), Telefone = "", Inscricao = "" }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }


        private class JsonResponseModel
        {
            public bool Success { get; set; }

            public string Telefone { get; set; }

            public string Inscricao { get; set; }

            public string Message { get; set; }

        }

        public ActionResult Producao()
        {
            return View();
        }

    }
}
