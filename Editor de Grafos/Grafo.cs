using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Editor_de_Grafos
{
    public class Grafo : GrafoBase, iGrafo
    {
        Random randNum = new Random();
        private bool[] visitado;

        private Color corAleatoria(List<Color> coresUsadas)
        {   // lista de cores com 26 cores contrastantes
            List<Color> cores = new List<Color> 
            {   Color.Silver, Color.DimGray, Color.SlateGray, Color.DarkSlateGray, 
                Color.Salmon, Color.Crimson, Color.DarkRed, Color.BurlyWood, 
                Color.SaddleBrown, Color.DarkKhaki, Color.Gold, Color.DeepPink, 
                Color.OrangeRed, Color.YellowGreen, Color.GreenYellow, Color.Lime, 
                Color.MediumSpringGreen, Color.SeaGreen, Color.DarkGreen, Color.Olive, 
                Color.Teal, Color.Cyan, Color.DodgerBlue, Color.BlueViolet, Color.DarkMagenta, Color.Indigo 
            };

            //remove as cores usadas da lista
            foreach (Color cor in coresUsadas)
                cores.Remove(cor);

            if (cores.Count != 0)
                return cores[randNum.Next(0, cores.Count - 1)]; //retorna uma cor aleatoria da lista que nao foi usada
            else
                return Color.FromArgb(randNum.Next(256), randNum.Next(256), randNum.Next(256)); //se todas as cores foram utilizadas, retorna uma cor criada randomicamente
        }

        public void desmarcarGrafo()
        {
            List<Vertice> vertice = vertices.FindAll(x => x.getCor() != Color.Blue); //vai desmarcar aqueles que foram coloridos por algum algoritmo 
            List<Aresta> aresta = arestas.FindAll(x => x.getCor() != Color.Blue); //vai desmarcar aqueles que foram coloridos por algum algoritmo 

            // pinta  vértices e arestas que estavam com outra cor
            foreach (Vertice v in vertice)
                v.setCor(Color.Blue);

            foreach (Aresta a in aresta)
                a.setCor(Color.Blue);
        }

        public void completarGrafo()
        {
            for (int i = 0; i < getN(); i++)
            {
                if (this.Controls.Contains(getVertice(i))) //excluindo as vertices deletadas
                {
                    for (int j = 0; j < getN(); j++)
                    {
                        if (getAresta(i, j) == null && i != j && this.Controls.Contains(getVertice(j))) //excluindo as vertices deletadas
                        {
                            if (getPesosAleatorios())
                                setAresta(i, j, randNum.Next(1, 100));
                            else
                                setAresta(i, j, 1);
                        }
                    }
                }
            }
        }

        public String paresOrdenados()
        {
            string str = "E = {";
            for (int i = 0; i < getN(); i++)
            {
                for (int j = 0; j < getN(); j++)
                {
                    if (getAresta(i, j) != null)
                    {
                        str += " (" + getVertice(i).getRotulo() + ", " + getVertice(j).getRotulo() + ")";
                    }
                }
            }
            str += " }";
            return str;
        }

        public bool isEuleriano()
        {
            for (int i = 0; i < getN(); i++)
            {
                if (grau(i) % 2 != 0) // VERIFICA SE vértices têm grau par
                    return false;
            }
            return (getN() > 0);
        }

        public bool isUnicursal()
        {
            int impar = 0;
            for (int i = 0; i < getN(); i++)
            {
                if (grau(i) % 2 != 0) // Verifica se possui exatamente dois vértices têm grau ímpar.
                    impar++;
            }
            return impar == 2;
        }

        public void preProfundidade(int v)
        {
            desmarcarGrafo();
            visitado = new bool[getN()];
            visitado[v] = true;
            getVertice(v).setCor(Color.MediumVioletRed); // Colore vértices e arestas visitados de rosa
            for (int i = 0; i < getN(); i++)
            {
                if (getAresta(v, i) != null && !visitado[i])
                {
                    getAresta(v, i).setCor(Color.MediumVioletRed);  // Colore vértices e arestas visitados de rosa
                    profundidade(i);
                }
            }
        }

        public void profundidade(int v)
        {
            visitado[v] = true;
            getVertice(v).setCor(Color.MediumVioletRed); // Colore vértices e arestas visitados de rosa
            for (int i = 0; i < getN(); i++)
            {
                if (getAresta(v, i) != null && !visitado[i])
                {
                    getAresta(v, i).setCor(Color.MediumVioletRed); // Colore vértices e arestas visitados de rosa
                    profundidade(i);
                }
            }
        }

        public bool isArvore()
        {
            preProfundidade(0);
            for (int i = 0; i < getN(); i++)
            {
                for (int j = 0; j < getN(); j++)
                {
                    if (getAresta(i, j) != null && getAresta(i, j).getCor() != Color.Red)
                        return false;
                }
            }

            return true;
        }
        public void largura(int v)
        {
            desmarcarGrafo();
            visitado = new bool[getN()];
            Fila f = new Fila(getN());
            f.enfileirar(v);
            visitado[v] = true; // visita os vizinhos 
            getVertice(v).setCor(Color.LimeGreen); //pinta vértices e arestas de verde-limão
            while (!f.vazia())
            {
                v = f.desenfileirar();
                for (int i = 0; i < getN(); i++)
                {
                    if (getAresta(v, i) != null && !visitado[i])
                    {
                        visitado[i] = true;
                        f.enfileirar(i);
                        getVertice(i).setCor(Color.LimeGreen);
                        getAresta(v, i).setCor(Color.LimeGreen);
                    }
                }
            }
        }

        public void caminhoMinimo(int origin, int destination)
        {
            // algoritmo de Dijkstra
            // encontrar o caminho mais curto
            Vertice[] predecessor = new Vertice[getN()];
            int[] estimated = new int[getN()];
            bool[] closed = new bool[getN()];

            for (int i = 0; i < estimated.Length; i++)
                estimated[i] = int.MaxValue;

            predecessor[origin] = getVertice(origin);
            estimated[origin] = 0;
            closed[origin] = true;
            int v = origin;

            while (!closed[destination])
            {
                foreach (Vertice vertice in getAdjacentes(v))
                {
                    if (!closed[vertice.getNum()])
                    {
                        int novaestimated = estimated[v] + getAresta(v, vertice.getNum()).getPeso();
                        if (novaestimated < estimated[vertice.getNum()])
                        {
                            estimated[vertice.getNum()] = novaestimated;
                            predecessor[vertice.getNum()] = getVertice(v);
                        }
                    }
                }

                int menorestimated = int.MaxValue;
                for (int k = 0; k < getN(); k++)
                {
                    if (!closed[k] && estimated[k] < menorestimated)
                    {
                        menorestimated = estimated[k];
                        v = k;
                    }
                }

                closed[v] = true;
            }
            //colorir grafo
            desmarcarGrafo();

            v = destination;
            while (v != origin)
            {
                getAresta(v, predecessor[v].getNum()).setCor(Color.Red);
                v = predecessor[v].getNum();
            }

            getVertice(origin).setCor(Color.Olive);
            getVertice(destination).setCor(Color.Peru);

            MessageBox.Show("Custo mínimo: " + estimated[destination]);
        }
        public void AGM()
        {
            int totalCost = 0;
            int auxV = 0;
            Aresta smallestEdge = null;
            List<int> visitados = new List<int>();

            desmarcarGrafo();
            getVertice(0).setCor(Color.Red);
            visitados.Add(0);

            while (visitados.Count < getControls()) //getControls pega todas vertices que nao foram excluidas
            {
                foreach (int vi in visitados)
                {
                    foreach (Vertice vertice in getAdjacentes(vi)) //Implementa Prim, a partir do vértice 0:
                    {
                        if (!visitados.Contains(vertice.getNum()))
                        {
                            if (smallestEdge == null || getAresta(vi, vertice.getNum()).getPeso() < smallestEdge.getPeso())
                            {
                                smallestEdge = getAresta(vi, vertice.getNum());
                                auxV = vertice.getNum();
                            }
                        }
                    }
                }

                totalCost += smallestEdge.getPeso();
                smallestEdge.setCor(Color.Red);
                getVertice(auxV).setCor(Color.Red);
                visitados.Add(auxV);
                smallestEdge = null;
            }

            MessageBox.Show("Custo total: " + totalCost);
        }

        public int numeroCromatico()
        {
            //menor número de cores necessárias para colorir os vértices 
            int v = 0;

            setVerticeMarcado(null);
            desmarcarGrafo();

            List<Color> colors = new List<Color>();
            Color ca = corAleatoria(colors);                                                         //Coloração gulosa dos vértices
            visitado = new bool[getN()]; //Marca quais vértices já foram visitados.                //Usa o menor número de cores diferentes 
            Fila f = new Fila(getN());                                                               //sem que vértices adjacentes tenham a mesma cor

            f.enfileirar(v);
            visitado[v] = true;
            getVertice(v).setCor(ca); // vértice inicial com a primeira cor
            colors.Add(ca);

            while (!f.vazia())
            {
                v = f.desenfileirar();
                for (int i = 0; i < getN(); i++)
                {
                    if (getAresta(v, i) != null && !visitado[i])
                    {
                        List<Color> colorsAux = new List<Color>();
                        colorsAux.AddRange(colors.ToArray());
                        visitado[i] = true;
                        f.enfileirar(i);
                        getAresta(v, i).setCor(Color.Red);

                        foreach (Vertice vertice in getAdjacentes(i))
                            colorsAux.RemoveAll(x => x == vertice.getCor()); //Remove as cores que os vizinhos já usados

                        if (colorsAux.Count != 0)
                            getVertice(i).setCor(colorsAux[0]);
                        else
                        {
                            ca = corAleatoria(colors);
                            colors.Add(ca);
                            getVertice(i).setCor(ca);
                        }

                    }
                }
            }
            return colors.Count;
        }

        public int indiceCromatico()
        {
            //menor número de cores necessárias para colorir as arestas
            if (arestas.Count > 0) //Verifica se existem arestas:
            {
                //Reseta o estado do grafo
                setVerticeMarcado(null);
                desmarcarGrafo();

                List<Color> colors = new List<Color>();
                Color ca;
                List<Aresta> visitedEdges = new List<Aresta>(); // evita visitar a mesma aresta mais de uma vez

                for (int i = 0; i < getN(); i++)
                {
                    for (int j = 0; j < getN(); j++)
                    { //Percorre a matriz de adjacência
                        Aresta a = getAresta(i, j); //pega a aresta correspondente
                        if (a != null && !visitedEdges.Contains(getAresta(i, j)))
                        {
                            List<Color> colorsAux = new List<Color>();
                            colorsAux.AddRange(colors.ToArray()); // Arestas adjacentes não possuem a mesma cor
                            visitedEdges.Add(a);

                            foreach (Vertice v in getAdjacentes(i))
                                colorsAux.RemoveAll(x => x == getAresta(i, v.getNum()).getCor()); //Remove as cores já usadas pelas arestas 

                            foreach (Vertice v in getAdjacentes(j))
                                colorsAux.RemoveAll(x => x == getAresta(j, v.getNum()).getCor()); //Remove as cores já usadas pelas vertices 

                            if (colorsAux.Count != 0)
                                a.setCor(colorsAux[0]); //// usa uma cor já existente
                            else
                            {
                                ca = corAleatoria(colors); //// cria uma nova cor
                                colors.Add(ca);
                                a.setCor(ca);
                            }
                        }
                    }
                }

                return colors.Count;
            }
            else
                return 0;
        }
    }
}