using System;
using System.Collections.Generic;

namespace PandemicSimulator
{
    public class Pessoa
    {
        
        public int Nome { get; set; }
        public int Status { get; set; } //0: Saudável, 1:Infectado, 2:Morto
        public int NLigacoes { get; set; }
        public List<String> Ligacoes { get; set; }
        public bool UsaMascara  { get; set; }
        public bool Trasmitindo {get; set; }
        public int Posicao {get; set; }
        public List<int> LigacoesArray { get; set; }

        public Pessoa(int nome, int status, int Nligacoes, List<String> ligacoes, bool mascara, bool trasmitindo, int posicao, List<int> ligacoesArray){
            this.Nome = nome;
            this.Status = status;
            this.NLigacoes = Nligacoes;
            this.Ligacoes = ligacoes;
            this.UsaMascara = mascara;
            this.Trasmitindo = trasmitindo;
            this.Posicao = posicao;
            this.LigacoesArray = ligacoesArray;
        }

        public int GetNome(){
            return this.Nome;
        } 
        public int GetStatus(){
            return this.Status;
        }
        public int GetNLigacoes(){
            return this.NLigacoes;
        }
        public string GetLigacoes(){
            String ligacoesTexto = "";
            foreach(String ligacao in this.Ligacoes){
                ligacoesTexto = ligacoesTexto + ligacao;
            }
            return ligacoesTexto;
        }

        public string GetAllLigacoes(){
            String allLigacoes = "";
            foreach(String ligacao in this.Ligacoes){
                allLigacoes += $"{this.Nome.ToString()}, {ligacao}";
            }
            return allLigacoes;
        }
        public bool getUsaMascara(){
            return this.UsaMascara;
        }

        public void setLigacoes(String ligacao){
            this.Ligacoes.Add(ligacao);
        }
    }
}