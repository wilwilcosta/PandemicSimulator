using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PandemicSimulator
{
    class Program
    {
        #region "Variaveis publicas"
        public static Random rnd = new Random();
        static public double taxaTransmissaoNoMascara = 0.95;
        static public double taxaTransmissao1Mascara = 0.425;
        static public double taxaTransmissao2Mascara = 0.2375;
        static public double taxaTransmissaoMascara = 0.5;
        public static StringBuilder sb = new StringBuilder();
        private List<Pessoa> ListaPessoasTotal;
        public static string path = $@"C:\Users\Wilson\Desktop\Projetos\PandemicSimulator\TestesTxt\Teste";
        #endregion
        #region "main"
        static void Main(string[] args)
        {
            Console.WriteLine("Quantas pessoas gostaria de simular?: ");
            int nPessoas = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Qual o número de infecções iniciais?: ");
            int nInfeccoesIniciais = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Quantos dias gostaria de simular?: ");
            int nDias = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Qual a porcentagem de adesâo às máscaras?: ");
            double porcentMascara = Double.Parse(Console.ReadLine())/100;
          
            List<Pessoa> ListaPessoasTotal = new List<Pessoa>();
            for (var i = 1; i<=nPessoas; i++ )
            {
                var pessoa = CriarPessoa(i,porcentMascara);
                ListaPessoasTotal.Add(pessoa);
            }
            Console.WriteLine("Número de Pessoas na lista: " + ListaPessoasTotal.Count);
            setLigacoes(ListaPessoasTotal);
            InserirVirus(ListaPessoasTotal, nInfeccoesIniciais, nPessoas);
            for(var i = 0; i <= nDias; i++)
            {
                Console.WriteLine($"====== Dia: {i} ======");
                ChecarTransmitiu(ListaPessoasTotal);
                ListAllInformacoes(ListaPessoasTotal);
                ExportarLigacoes(ListaPessoasTotal);
            }
            ExportToTXT(ListaPessoasTotal);
            int nInfeccoesFinais = 0;
            foreach(Pessoa pessoa in ListaPessoasTotal)
            {
                if (pessoa.Status == 1) {
                    nInfeccoesFinais++;
                }
            }

            Console.WriteLine($"Número inicial de infecções:{nInfeccoesIniciais}");
            Console.WriteLine($"Número final de infecções:{nInfeccoesFinais}");
            getColors(ListaPessoasTotal);
        }
        #endregion

        #region "funções"
        static Pessoa CriarPessoa(int nome, double porcentagem){
            List<String> listaLigacoes = new List<string>();
            List<int> ligacoesArray = new List<int>();
            var pessoaCriada = new Pessoa(nome, 0, getNumeroLigacoes(), listaLigacoes, ColocarMascara(porcentagem), false, nome,ligacoesArray);
            Console.WriteLine($"Nome: {pessoaCriada.GetNome()} Número de ligações: {pessoaCriada.GetNLigacoes()}");
            return pessoaCriada;
        }

        static int getNumeroLigacoes(){
            // 40% de chance de 5 ligações
            // 30% de chance de 10 ligações
            // 25% de chance de 30 ligações
            // 5% de chance de 50 ligações

            int num = Program.rnd.Next(101);
            if(num <= 40){ 
                return 1; //original 5
            }
            if(num > 41 && num <= 70){ 
                return 2; //original 10
            }
            if(num > 71 && num <= 95){
                return 3; //original 30
            }
            else{
                return 4; //original 50
            }
        }   
        static void setLigacoes(List<Pessoa> lista){
            foreach(Pessoa pessoa in lista){
                var nlig = pessoa.GetNLigacoes();
                for(var i = 0; i < nlig; i++){
                    var rndLigacao = rnd.Next(1, lista.Count);
                    if (pessoa.LigacoesArray.Contains(rndLigacao) || rndLigacao == pessoa.Nome)
                    {}
                    else
                    {
                        pessoa.setLigacoes(rndLigacao.ToString() + ", ");
                        pessoa.LigacoesArray.Add(rndLigacao);
                    }
                }
                Console.WriteLine($"A pessoa {pessoa.GetNome()} tem ligações com os números:{pessoa.GetLigacoes()}");
            }
        }
        static void InserirVirus(List<Pessoa> lista, int numeroInfeccoesIniciais, int nPessoas){
            for(var i = 0; i<= numeroInfeccoesIniciais; i++){
                lista[rnd.Next(1,nPessoas)].Status = 1;
            }
        }

        public int this[int index]
        {
            get 
            {
             return ListaPessoasTotal.FirstOrDefault(pessoa => pessoa.Posicao == index).Status;
            }
            set
            {
                 ListaPessoasTotal.FirstOrDefault(pessoa => pessoa.Posicao == index).Status = 1;
            }
        }
        static void ExportarLigacoes(List<Pessoa> lista){
            Console.WriteLine("Todas as Ligações: ");
            foreach(Pessoa pessoa in lista){
            Console.Write($"{pessoa.GetAllLigacoes()}");  
            }
        }

        static void ListAllInformacoes(List<Pessoa> lista){
            foreach (Pessoa pessoa in lista){

            Console.WriteLine($"A pessoa {pessoa.Nome} tem ligações com:{pessoa.GetLigacoes()},status do dia:{pessoa.Status},trasmitindo:{pessoa.Trasmitindo}, mascara:{pessoa.UsaMascara}");
            }
        }

        static void ChecarTransmitiu(List<Pessoa> lista){
            foreach(Pessoa pessoa in lista){ //pega status da pessoa
                if(pessoa.GetStatus() == 1) 
                {    
                bool check = getTransmitionByType(pessoa.UsaMascara);
                if(check == true){
                    pessoa.Trasmitindo = true;
                    foreach(int conexao in pessoa.LigacoesArray)
                    {
                        ChecarPegou(lista[conexao]);
                    }
                }
                }
                Console.WriteLine($"Pessoa: {pessoa.Nome}, transmitindo: {pessoa.Trasmitindo}");
                
            }
            foreach (Pessoa pessoa in lista)
            {
                pessoa.Trasmitindo = false;
            }
        }
        static bool getTransmitionByType(bool mascara){
            if (mascara == true)
            {
                return rnd.NextDouble() > taxaTransmissaoMascara;
            }
            else 
            {
                return true;
            }
        }
        static void ChecarPegou(Pessoa pessoa)
        {
 
             if(pessoa.GetStatus() == 0) 
                {    
                    bool check = getTransmitionByType2(pessoa.UsaMascara);
                    if(check == true)
                    {
                        pessoa.Status = 1;
                        Console.WriteLine($"A pessoa {pessoa.Nome} foi infectada");
                    }
            }
        }
        static bool getTransmitionByType2(bool mascara) {
            if (mascara == true)
            {
                return rnd.NextDouble() >0.558;
            }
            else
            {
                return true;
            }
        }

        static void ExportToTXT(List<Pessoa> lista){
            foreach(Pessoa pessoa in lista)
            {
                sb.AppendFormat($"A pessoa {pessoa.Nome} tem ligações com:{pessoa.GetLigacoes()},status do dia:{pessoa.Status},trasmitindo:{pessoa.Trasmitindo}, usa Mascara:");
                sb.AppendLine();
            }
            
            File.WriteAllText(path, sb.ToString());
        }
        static void ListarLigacoes(List<Pessoa> lista)
        {
            foreach(Pessoa pessoa in lista)
            {
                if (pessoa.LigacoesArray.Count == 0)
                {
                    Console.WriteLine("lsita vazia");
                }
                else
                {
                    foreach (int ligacao in pessoa.LigacoesArray)
                    {
                        Console.WriteLine(ligacao);
                    }

                }

            }
        }
        static bool ColocarMascara(double porcentagemMascara)
        {
            if (porcentagemMascara != 0)
            {
                if (rnd.NextDouble() < porcentagemMascara)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else {
                return false;
                 }
            
        }
        static void getColors(List<Pessoa> lista)
        {
            string listaCores = "";
            foreach(Pessoa pessoa in lista)
            {
                if(pessoa.UsaMascara == true && pessoa.Status == 0) //verde saudavel com mascara
                {
                    listaCores += "1, ";
                }
                if (pessoa.UsaMascara == false && pessoa.Status == 0) //azul saudavel sem mascara
                {
                    listaCores += "2, ";
                }
                if (pessoa.UsaMascara == true && pessoa.Status == 1)//laranja infectado com mascara
                {
                    listaCores += "3, ";
                }
                if (pessoa.UsaMascara == false && pessoa.Status == 1)//vermelho infectado sem mascara
                {
                    listaCores += "4, ";
                }
                if (pessoa.UsaMascara == true && pessoa.Status == 2)//cinza morto com mascara
                {
                    listaCores += "5, ";
                }
                if (pessoa.UsaMascara == false && pessoa.Status == 2)//preto morto sem mascara
                {
                    listaCores += "6, ";
                }
            }
            Console.WriteLine($"Lista de cores de cada node: {listaCores}");
        }
        #endregion
    }
}
