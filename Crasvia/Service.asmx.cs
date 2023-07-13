using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Crasvia
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://crasvia.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {

        Image image, img;
        MemoryStream m;
        byte[] imageBytes = null;

        SqlCommand cmd2;
        SqlCommand cmd;
        SqlConnection cc;
        SqlConnection conn;

        private string resposta = "", Foto = "", ret = "", retorno = "", geral = "", base64String = "";

        [WebMethod]
        public string EnviarFoto(string FotosString, string NomeFotos, string nomePasta)
        {

            try
            {
                img = Base64ToImage(FotosString);
                img.Save(@"C:\Users\Administrator\Dropbox\Tablet\Fotos\" + nomePasta + "\\" + NomeFotos);  
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Ok";
        }

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[] 
            imageBytes = Convert.FromBase64String(base64String);
            m = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image 
            m.Write(imageBytes, 0, imageBytes.Length);
            image = Image.FromStream(m, true);

            m = null;
            imageBytes = null;

            return image;
        }

        [WebMethod]
        public string SincronizaDados(string dados)
        {
            String codAuto = "";

            SqlConnection connection01 = new SqlConnection();

            SqlCommand command01;


            try
            {   
                connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                connection01.Open();
              

                string[] registro = dados.Split('¬');

                for (int i = 0; i < registro.Length; i++)
                {
                    if (i == registro.Length - 1)
                    {

                    }
                    else
                    {
                        string dadosregistro = registro[i].ToString();

                        command01 = new SqlCommand
                        {
                            Connection = connection01,
                            CommandText = dadosregistro
                        };

                        command01.ExecuteNonQuery();



                        /*cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = dadosregistro;
                        cmd.ExecuteNonQuery();

                        cmd = null;*/
                    }
                }

                connection01.Close();

                /*cc = new SqlConnection();
                cc.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString"].ToString();
                cc.Open();*/
               
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Error Message = {1}", e.Message);

                /*cmd2 = null;
                cmd = null;
                conn.Close();
                cc.Close();*/

                connection01.Close();
                command01 = null;

                return "";
            }

            return "OK";
        }

        [WebMethod]
        public string Login(String usuario, String senha, String monitoracao)
        {
            String codUsuario = "N";
            String usuarioAtivo = "N";
            String ConceAtiva = "N";
            String codCSM = "N";
            String nomeUsuario = "";
            String codConcessonaria = "N";
            String resposta = "N";
            String Rodovia = "N";            

            SqlConnection connection01 = new SqlConnection();

            SqlCommand command01;           

            try
            {

                connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                connection01.Open();

                command01 = new SqlCommand
                {
                    Connection = connection01,
                    CommandText = "select * from Usuario WHERE ID = '" + usuario + "' and senha = '" + senha + "'"
                };

                command01.ExecuteNonQuery();

                using (SqlDataReader reader01 = command01.ExecuteReader())
                {
                    while (reader01.Read())
                    {
                        codUsuario = reader01["codseq"].ToString();
                        resposta = reader01["nome"].ToString();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error Message = {1}", e.Message);

                connection01.Close();
                command01 = null;

                return "Usuario ou senha incorretos!";                
            }



            connection01.Close();
            command01 = null;
            cmd = null;

            //Verifica se o usuário esta ativo
            if (!codUsuario.Equals("N"))
            {
                try
                {
                    connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                    connection01.Open();

                    command01 = new SqlCommand
                    {
                        Connection = connection01,
                        CommandText = "select * from usuario WHERE codSeq = '" + codUsuario + "' and excluido = 0"
                    };

                    command01.ExecuteNonQuery();

                    using (SqlDataReader reader01 = command01.ExecuteReader())
                    {
                        while (reader01.Read())
                        {
                            usuarioAtivo = reader01["codSeq"].ToString();
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error Message = {1}", e.Message);

                    connection01.Close();
                    command01 = null;

                    return "Usuario não ativado! Contate o Adminstrador!";
                }

            } else
            {
                resposta = "Usuário ou senha incorretos!";
                return resposta;
            }

            connection01.Close();
            command01 = null;
            cmd = null;

            //Verifica se o usuário esta vinculado para alguma monitoração
            if (!usuarioAtivo.Equals("N")){
                try
                {


                    connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                    connection01.Open();

                    command01 = new SqlCommand
                    {
                        Connection = connection01,
                        CommandText = "select * from usuarioCSM u" +
                        " INNER JOIN ConcessionariaMonitoracao cm ON cm.codSeq = u.codCSM" +
                        " WHERE  u.codUsuario = '" + usuarioAtivo + "' and u.excluido = 0 and cm.excluido = 0" +
                        " and cm.monitoracao = '"+monitoracao+"'"

                    };
                   
                    command01.ExecuteNonQuery();

                    using (SqlDataReader reader01 = command01.ExecuteReader())
                    {
                        while (reader01.Read())
                        {
                            codConcessonaria = reader01["codConcessionaria"].ToString();                            
                        }
                    }
                } catch (IOException e)
                {
                    Console.WriteLine("Error Message = {1}", e.Message);

                    connection01.Close();
                    command01 = null;

                    return "Monitoração de '"+monitoracao+"' Não ativa para este usuário! Contate o adminstrador!";
                }

            } 
            else
            {
                resposta = "Usuário desativado!";
                return resposta;
            }

            connection01.Close();
            command01 = null;
            cmd = null;
            
            //Busca a concessonaria
            if (!codConcessonaria.Equals("N"))
            {
                try
                {
                    resposta += "¢";

                    connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                    connection01.Open();

                    command01 = new SqlCommand
                    {
                        Connection = connection01,
                        CommandText = "select * from Concessionaria C " +
                        "INNER JOIN ConcessionariaMonitoracao CM ON C.codseq = CM.codConcessionaria WHERE C.codseq = '" + codConcessonaria+"' AND C.excluido = 0 AND CM.monitoracao = '"+ monitoracao + "'"

                    };

                    command01.ExecuteNonQuery();

                    using (SqlDataReader reader01 = command01.ExecuteReader())
                    {
                        while (reader01.Read())
                        {
                            ConceAtiva = reader01["codSeq"].ToString();
                            resposta += "INSERT INTO Concessonaria (codAuto, nome, ano, semestre) " +
                                "VALUES ('" + reader01["codSeq"].ToString() + "', '" + reader01["nome"].ToString() + "', '" + reader01["ano"].ToString() + "', '" + reader01["semestre"].ToString() + "')¬¬";
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error Message = {1}", e.Message);

                    connection01.Close();
                    command01 = null;

                    return "Concessonaria não ativa! Contate o adminstrador!";
                }
            } else
            {
                resposta = "Usuário não vinculado!";
                return resposta;
            }

            connection01.Close();
            command01 = null;
            cmd = null;

            //Busca as rodovias
            if (!ConceAtiva.Equals("N"))
            {
                try
                {
                    resposta += "¢";

                   connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                    connection01.Open();

                    command01 = new SqlCommand
                    {
                        Connection = connection01,
                        CommandText = "select * from RodoviaConcessionaria WHERE IdConcessionaria = '" + ConceAtiva + "'"

                    };

                    command01.ExecuteNonQuery();

                    using (SqlDataReader reader01 = command01.ExecuteReader())
                    {
                        while (reader01.Read())
                        {

                            Rodovia = reader01["codSeq"].ToString();                            
                            resposta += "INSERT INTO Rodovia (codAuto, nome, tipo, Norte, Sul, Leste, Oeste, CanteiroCentral, Interno, Externo, NorteSul, LesteOeste,InternoExterno, Transversal) " +
                                "VALUES ('" + reader01["codSeq"].ToString() + "', '" + reader01["Rodovia"].ToString() + "', '" + reader01["Tipo"].ToString() + "', '" + reader01["Norte"].ToString() + "'," +
                                "'" + reader01["Sul"].ToString() + "', '" + reader01["Leste"].ToString() + "', '" + reader01["Oeste"].ToString() + "','" + reader01["CanteiroCentral"].ToString() + "'," +
                                "'" + reader01["Interno"].ToString() + "', '" + reader01["Externo"].ToString() + "', '" + reader01["NorteSul"].ToString() + "', '" + reader01["LesteOeste"].ToString() + "', " +
                                "'" + reader01["InternoExterno"].ToString() + "', '" + reader01["Transversal"].ToString() + "')¬¬";
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error Message = {1}", e.Message);

                    connection01.Close();
                    command01 = null;

                    return "Rodovias não encontradas!";
                }
            } else
            {
                resposta = "Concessonaria não encontrada!";
                return resposta;
            }
            if (Rodovia.Equals("N"))
            {
                return "Rodovia não cadastrada!";
            }

            connection01.Close();
            command01 = null;
            cmd = null;


            return resposta;
        }

        [WebMethod]
        public string Carregar(string banco)
        {
            String codUsuario = "N";
            String usuarioAtivo = "N";
            String ConceAtiva = "N";
            String codCSM = "N";
            String nomeUsuario = "";
            String codConcessonaria = "N";
            String resposta = "";
            string Rodovia = "N";

            SqlConnection connection01 = new SqlConnection();

            SqlCommand command01;

            try
            {

                connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();

                //resposta = " KmFim: '" + kmFinal + "'";
                connection01.Open();

                //resposta = "select * from " + banco + " WHERE KM_ini > '" + KmInicia + "' and KM_ini < '" + KmInicia + "' AND Rodovia = '" + rodovia + "'";

                command01 = new SqlCommand
                {
                    Connection = connection01,
                    CommandText = "select * from " + banco + ""                   
                };

                command01.ExecuteNonQuery();

                //resposta = "teste23";
                //resposta = (String) command01.CommandText.Trim();

                using (SqlDataReader reader01 = command01.ExecuteReader())
                {
                    while (reader01.Read())
                    {
                        resposta += "INSERT INTO EPS (codAnterior, Sincronizado, Dispositivo, Posicionamento, TipoRodovia, Rodovia, Pista, Via, Ramo, Sentido, KmInicial," +
                            "KmFinal, Lado, Compr, Fixacao, Nivelado, Tipo, Modelo, Perfil, NivelContencao, EspacoTrabalho, Altura, EspPostes, QtLaminas, QtPostes, DispCompl," +
                            "TerminalEntrada, TipoTAE, ComprTerminalEntrada, InstEntrada, TerminalSaida, TipoTAESaida, ComprTerminalSaida, InstSaida, DistBorda, DistEntradaBorda," +
                            "DistSaidaBorda, AltEspelho1, AltEspelho2, PresencaDelienadores, DelienadoresRefletivos, TipoRefletor, RefBranco, RefAmarelo, RefVermelho, RefLima," +
                            "ProblemaElemRefle, EspRefletivos, AltRefletor, QntdRefletores, ConsEle, SitDisDrenagem, Obs, Enviado, FotoAnterior, X1Anterior, Y1Anterior) " +
                            "VALUES ('" + reader01["codAnterior"].ToString() + "', '1', '" + reader01["Dispositivo"].ToString() + "'," +
                            "'" + reader01["Posicionamento"].ToString() + "', '" + reader01["TipoRod"].ToString() + "', '" + reader01["Rodovia"].ToString() + "'," +
                            "'" + reader01["Pista"].ToString() + "', '" + reader01["Via"].ToString() + "', '" + reader01["Ramo"].ToString() + "'," +
                            "'" + reader01["Sentido"].ToString() + "', '" + reader01["KM_ini"].ToString() + "', '" + reader01["KM_Fim"].ToString() + "'," +
                            "'" + reader01["Lado"].ToString() + "', '" + reader01["comprimento"].ToString() + "', '" + reader01["fixacao"].ToString() + "'," +
                            "'" + reader01["nivelado"].ToString() + "', '" + reader01["tipo"].ToString() + "', '" + reader01["modelo"].ToString() + "'," +
                            "'" + reader01["perfil"].ToString() + "', '" + reader01["NvContencao"].ToString() + "', '" + reader01["EspacoTrabalho"].ToString() + "'," +
                            "'" + reader01["Altura"].ToString() + "', '" + reader01["EspacamentoPoste"].ToString() + "', '" + reader01["qtdLamina"].ToString() + "'," +
                            "'" + reader01["qtdPoste"].ToString() + "', '" + reader01["DispComplementar"].ToString() + "', '" + reader01["TerminalEntrada"].ToString() + "'," +
                            "'" + reader01["TipoTAEEntrada"].ToString() + "', '" + reader01["CumprTermEntrada"].ToString() + "', '" + reader01["InstalacaoEntrada"].ToString() + "'," +
                            "'" + reader01["TermSaida"].ToString() + "', '" + reader01["TipoTAESaida"].ToString() + "', '" + reader01["CumprTermSaida"].ToString() + "'," +
                            "'" + reader01["InstalacaoSaida"].ToString() + "', '" + reader01["DistBordaFxRolamento"].ToString() + "', '" + reader01["DistTermEntrFxRolamento"].ToString() + "'," +
                            "'" + reader01["DistTermSaidFxRolamento"].ToString() + "', '" + reader01["AlturaEspelhoF1"].ToString() + "', '" + reader01["AlturaEspelhoF2"].ToString() + "'," +
                            "'" + reader01["PresDelinMA"].ToString() + "', '" + reader01["PresEleRefletivCat"].ToString() + "', '" + reader01["TipoRefletor"].ToString() + "'," +
                            "'" + reader01["Branco"].ToString() + "', '" + reader01["Amarelo"].ToString() + "', '" + reader01["Vermelho"].ToString() + "'," +
                            "'" + reader01["LimaLimao"].ToString() + "', '" + reader01["ProblemaEleRefletivo"].ToString() + "', '" + reader01["EspacRefletiv"].ToString() + "'," +
                            "'" + reader01["AltRefletor"].ToString() + "', '" + reader01["QtdRefletor"].ToString() + "', '" + reader01["ConservEleReflet"].ToString() + "'," +
                            "'" + reader01["SitDispDrenagem"].ToString() + "', '" + reader01["Observacao"].ToString() + "', 'N', '" + reader01["FotoAnterior"].ToString() + "'," +
                            "'" + reader01["Lat_ini_Anterior"].ToString().Replace(",", ".") + "', '" + reader01["Long_ini_Anterior"].ToString().Replace(",", ".") + "')¬¬";
                    }
                }
                if (resposta.Length < 1)
                {
                    resposta = "N";
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error Message = {1}", e.Message);

                connection01.Close();
                command01 = null;

                return e.Message;
            }



            connection01.Close();
            command01 = null;
            cmd = null;
            
            return resposta;
        }

        [WebMethod]
        public string CarregarHorizontal(string banco)
        {
            String codUsuario = "N";
            String usuarioAtivo = "N";
            String ConceAtiva = "N";
            String codCSM = "N";
            String nomeUsuario = "";
            String codConcessonaria = "N";
            String resposta = "";
            string Rodovia = "N";

            SqlConnection connection01 = new SqlConnection();

            SqlCommand command01;

            try
            {

                connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();
                
                connection01.Open();

                command01 = new SqlCommand
                {
                    Connection = connection01,
                    CommandText = "select * from " + banco + ""
                };

                command01.ExecuteNonQuery();              

                using (SqlDataReader reader01 = command01.ExecuteReader())
                {
                    while (reader01.Read())

                    {
                        resposta += "INSERT INTO Horizontal (Removido, codAnterior, dataLigacao, Sincronizado, dispositivo, tipo, qtd, Data, cor, comp," +
                            "larg, reprova, Obs, TipoRodovia, Rodovia, Pista, Via, Ramo, Sentido, KmInicial, KmSPD, enviado, posicaofaixa, FotoAnterior," +
                            "X1Anterior, Y1Anterior, codEasyLux) " +
                            "VALUES ('" + reader01["Removido"].ToString() + "', '" + reader01["codAnterior"].ToString() + "'," +
                            "'" + reader01["dataligacao"].ToString() + "', '1', '" + reader01["dispositivo"].ToString() + "'," +
                            "'" + reader01["tipo"].ToString() + "', '" + reader01["qtd"].ToString() + "', '" + reader01["DataInspec"].ToString() + "'," +
                            "'" + reader01["cor"].ToString() + "', '" + reader01["comprimento"].ToString() + "', '" + reader01["largura"].ToString() + "'," +
                            "'" + reader01["ReprovaVisual"].ToString() + "', '" + reader01["observacao"].ToString() + "', '" + reader01["tipoRodovia"].ToString() + "'," +
                            "'" + reader01["nomeRodovia"].ToString() + "', '" + reader01["pista"].ToString() + "', '" + reader01["via"].ToString() + "'," +
                            "'" + reader01["ramo"].ToString() + "', '" + reader01["sentido"].ToString() + "', '" + reader01["km"].ToString() + "'," +
                            "'" + reader01["km_SPD"].ToString() + "', 'N', '" + reader01["posicaoFaixa"].ToString() + "'," +
                            "'" + reader01["fotoAnterior"].ToString() + "', '" + reader01["lat1_Anterior"].ToString().Replace(",", ".") + "', '" + reader01["long1_Anterior"].ToString().Replace(",", ".") + "'," +
                            "'" + reader01["codEasyLux"].ToString() + "')¬¬";
                    }                    
                }
                if (resposta.Length < 1)
                {
                    resposta = "N";
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error Message = {1}", e.Message);

                connection01.Close();
                command01 = null;

                return e.Message;
            }



            connection01.Close();
            command01 = null;
            cmd = null;

            return resposta;
        }



        [WebMethod]
        public string CarregarKML(string banco, string tipo)
        {
            String codUsuario = "N";
            String usuarioAtivo = "N";
            String ConceAtiva = "N";
            String codCSM = "N";
            String nomeUsuario = "";
            String codConcessonaria = "N";
            String resposta = "";
            string Rodovia = "N";
            String SQL = "";

            SqlConnection connection01 = new SqlConnection();

            SqlCommand command01;

            try
            {

                connection01.ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString1"].ToString();

                //resposta = " KmFim: '" + kmFinal + "'";
                connection01.Open();

                //resposta = "select * from " + banco + " WHERE KM_ini > '" + KmInicia + "' and KM_ini < '" + KmInicia + "' AND Rodovia = '" + rodovia + "'";

                if (tipo.Equals("TODOS"))
                {
                    SQL = "select * from " + banco + "_Tablet";
                }
                else
                {
                    SQL = "select c1.* from " + banco + "_Tablet c1 " +
                    "LEFT JOIN " + banco + " c2 ON c1.codAnterior = c2.codAnterior " +
                    "WHERE c2.codAnterior IS NULL";
                }

                command01 = new SqlCommand
                {
                    Connection = connection01,
                    CommandText = SQL
                };

                command01.ExecuteNonQuery();

                //resposta = "teste23";
                //resposta = (String) command01.CommandText.Trim();

                using (SqlDataReader reader01 = command01.ExecuteReader())
                {
                    while (reader01.Read())
                    {
                        String teste = reader01["Long_ini_Anterior"].ToString().Replace(",", ".");
                        resposta += "INSERT INTO KML (codCadastro, codAnterior, X1, Y1, dispositivo, sentido, Km, comprimento) " +
                            "VALUES ('" + reader01["codCadastro"].ToString() + "', '" + reader01["codAnterior"].ToString() + "', '" + reader01["Lat_ini_Anterior"].ToString().Replace(",", ".") + "', " +
                            "'" + reader01["Long_ini_Anterior"].ToString().Replace(",", ".") + "', '" + reader01["Dispositivo"].ToString() + "', '" + reader01["Sentido"].ToString() + "', '" + reader01["KM_ini"].ToString() + "', '" + reader01["comprimento"].ToString() + "')¬¬";
                    }
                }
                if (resposta.Length < 1)
                {
                    resposta = "N";
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error Message = {1}", e.Message);

                connection01.Close();
                command01 = null;

                return e.Message;
            }



            connection01.Close();
            command01 = null;
            cmd = null;

            return resposta;
        }



        [WebMethod]       
        public string transformaFotoEmLotes(string foto, string pasta, string tabela)
        {
            SqlConnection connec;
            SqlCommand comma;
            //string resposta = "";
            StringBuilder respostaBuilder = new StringBuilder();

            try
            {
                connec = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["webConnectionString"].ToString()
                };

                connec.Open();

                comma = new SqlCommand
                {
                    Connection = connec,
                    CommandText = "SELECT " + foto + " FROM " + tabela
                };

                using (SqlDataReader reader = comma.ExecuteReader())
                {
                    const int batchSize = 100; // Tamanho do lote
                    int count = 0;
                    List<string> fotosBatch = new List<string>();

                    while (reader.Read())
                    {
                        string fotoAtual = reader[foto].ToString();
                        fotosBatch.Add(fotoAtual);
                        count++;

                        if (count % batchSize == 0)
                        {
                            respostaBuilder.Append(ProcessarLoteDeFotos(pasta, fotosBatch));
                            //respostaBuilder.Append("\n");
                            fotosBatch.Clear();
                        }
                    }

                    // Processar o lote final se houver fotos restantes
                    if (fotosBatch.Count > 0)
                    {
                        respostaBuilder.Append(ProcessarLoteDeFotos(pasta, fotosBatch));
                        //respostaBuilder.Append("\n");
                    }
                }

                if (respostaBuilder.Length < 1)
                {
                    //respostaBuilder.Append("N");
                }

                connec.Close();
                comma = null;
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error Message = {0}", exception.Message);                
                respostaBuilder.Append(exception.Message);
                return respostaBuilder.ToString();
            }
            catch (OutOfMemoryException exce)
            {
                Console.WriteLine("Error Message = {0}", exce.Message);
                respostaBuilder.Append(exce.Message);
                return respostaBuilder.ToString();
            }

            return respostaBuilder.ToString();
        }

        [WebMethod]
        public string ProcessarLoteDeFotos(string pasta, List<string> fotos)
        {
            StringBuilder respostaBuilder = new StringBuilder();

            foreach (string foto in fotos)
            {
                string fotoPath = @"C:\Users\Administrator\Dropbox\Tablet\MaterialImportacao\" + pasta + "\\" + foto;                

                if (File.Exists(fotoPath))
                {
                    using (Image image = Image.FromFile(fotoPath))
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, image.RawFormat);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        string ret = foto + "¢" + base64String + "¢";                       
                        respostaBuilder.Append(ret);
                        //respostaBuilder.Append("\n");
                    }
                }
            }

            return respostaBuilder.ToString();
        }

        }
}
