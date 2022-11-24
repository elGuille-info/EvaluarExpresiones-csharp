// Evaluar expresiones con tipo double. (22/nov/22 12.41)
// Basado en el código creado con Java.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using boolean = System.Boolean;
using Character = System.Char;
using Integer = System.Int32;

namespace EvaluarExpresiones;

public class Evaluar
{
    /**
 * Si se deben mostrar los resultados parciales de la evaluación de la expresión.
 */
    public static boolean mostrarParciales = false;

    /**
     * Los operadores multiplicativos.
     * Se puede usar la x para multiplicar y los dos puntos para dividir.
     */
    private static readonly String operadoresMultiplicativos = "x*:/%";

    /**
     * Los operadores aditivos.
     */
    private static readonly String operadoresAditivos = "+-";

    /**
     * Los operadores en el orden de precedencia.
     * Sin incluir los paréntesis que se procesan por separado.
     */
    private static readonly String losOperadores = operadoresMultiplicativos + operadoresAditivos;

    /**
     * Array de tipo char con los operadores en el orden de precedencia.
     */
    private static readonly char[] operadores = losOperadores.ToCharArray();

    /**
     * Evalúa una expresión. Punto de entrada para evaluar expresiones.
     *
     * @param expression La expresión a evaluar.
     * @return El valor entero de la expresión evaluada.
     */
    public static double evaluar(String expression)
    {
        if (expression == null)
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("La expresión a analizar es nula.");
            Console.ForegroundColor= ConsoleColor.White;
            return -1;
        }

        // Quitar todos los caracteres en blanco.
        expression = expression.Replace(" ", "");
        if (expression.Equals(""))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("La expresión a analizar es una cadena vacía.");
            Console.ForegroundColor = ConsoleColor.White;
            return 0;
        }

        // Comprobar si tenemos paréntesis. (21/nov/22 06.59)
        int iniApertura = expression.IndexOf('(');
        String res;

        if (iniApertura > -1)
        {
            // Comprobar que haya los mismos paréntesis de apertura que de cierre.
            if (!balancedParenthesis(expression))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Los paréntesis no están balanceados '{0}'.\n", expression);
                Console.ForegroundColor = ConsoleColor.White;
                return -1;
            }

            // Antes de evaluar las expresiones entre paréntesis
            //  comprobar si hay paréntesis precedidos de dígitos.
            expression = comprobarDigitParenthesis(expression);

            // Primero se evalúan todas las expresiones entre paréntesis.
            res = evaluarParenthesis(expression);
        }
        else
        {
            res = expression;
        }

        // En evaluarExp se comprueba si hay operadores.
        return evaluarExp(res);
    }

    /**
     * Comprueba si los paréntesis en la expresión están balanceados.
     *
     * @param expression La expresión a evaluar.
     * @return True si hay los mismos de apertura que de cierre, false en otro caso.
     * @since 1.1.2.1
     */
    static boolean balancedParenthesis(String expression)
    {
        int apertura = cuantasVeces(expression, '(');
        int cierre = cuantasVeces(expression, ')');
        return apertura == cierre;
    }

    /**
     * Cuenta las veces que el carácter indicado está en la expresión.
     *
     * @param expression La expresión a evaluar.
     * @param character El carácter a comprobar.
     * @return El número de veces que está el carácter en la expresión.
     * @since 1.1.2.1
     */
    static int cuantasVeces(String expression, char character)
    {
        int total = 0;
        for (int i = 0; i < expression.Length; i++)
        {
            //if (expression.charAt(i) == character)
            if (expression[i] == character)
            {
                total++;
            }
        }
        return total;
    }

    /**
     * Comprobar si hay paréntesis de apertura precedido por un dígito.
     *      Si es así, cambiarlo por dígito * (.
     * O si hay un paréntesis de apertura precedido por uno de cierre.
     *      Si es así, cambiarlo por )*(.
     *
     * @param expression La expresión a evaluar.
     * @return La expresión con el cambio realizado.
     * @since 1.1.2.0
     */
    private static String comprobarDigitParenthesis(String expression)
    {
        int desde = 0;
        int ini;

        while (true)
        {
            ini = expression.IndexOf('(', desde);
            // Si no hay más paréntesis de apertura, salir.
            if (ini == -1)
            {
                //return expression;
                break;
            }
            if (ini - 1 >= 0)
            {
                char digit = expression[ini - 1]; //.charAt(ini - 1);
                // Si lo precede un dígito o un )
                if (Character.IsDigit(digit) || digit == ')')
                {
                    // Cambiar este paréntesis por *(
                    //expression = expression.Substring(0, ini) + "*" + expression.Substring(ini);
                    expression = string.Concat(expression.AsSpan(0, ini), "*", expression.AsSpan(ini));
                    ini++;
                }
            }
            desde = ini + 1;
            if (desde > expression.Length)
            {
                //return expression;
                break;
            }
        }
        // Comprobar si hay paréntesis de cierre seguido de uno de apertura o de un dígito,
        //  si es así, cambiarlo poner un * entre los dos.
        desde = 0;
        while (true)
        {
            ini = expression.IndexOf(')', desde);
            // Si no hay más paréntesis de cierre, salir.
            if (ini == -1)
            {
                //return expression;
                break;
            }
            if (ini + 1 < expression.Length)
            {
                char digit = expression[ini + 1];
                // Si lo sigue un dígito o (
                if (Character.IsDigit(digit) || digit == '(')
                {
                    // Cambiar este paréntesis por )*
                    //expression = expression.substring(0, ini + 1) + "*" + expression.substring(ini + 1);
                    expression = string.Concat(expression.AsSpan(0, ini + 1), "*", expression.AsSpan(ini + 1));
                    ini++;
                }
            }
            desde = ini + 1;
            if (desde > expression.Length)
            {
                //return expression;
                break;
            }
        }

        return expression;
    }

    /**
     * Evalúa el contenido de las expresiones entre paréntesis.
     * Se permite NÚMERO(EXPRESIÓN) que se convierte en NÚMERO*(EXPRESIÓN).
     *
     * @param expression Expresión a evaluar (puede tener o no paréntesis).
     * @return La cadena sin los paréntesis y con lo que haya entre paréntesis ya evaluado.
     */
    private static String evaluarParenthesis(String expression)
    {
        boolean hay;
        do
        {
            // Posición del paréntesis de apertura.
            int ini = expression.IndexOf('(');
            // Si hay paréntesis de apertura...
            if (ini > -1)
            {
                // Posición del paréntesis de cierre.
                int fin = expression.IndexOf(')', ini);
                // Si hay paréntesis de cierre...
                if (fin > -1)
                {
                    // Comprobar si hay otro de empiece antes del cierre.
                    int ini2;
                    // Repetir hasta encontrar la pareja del de cierre.
                    while (true)
                    {
                        ini2 = expression.IndexOf('(', ini + 1);
                        if (ini2 > -1 && ini2 < fin)
                        {
                            // Hay uno de apertura antes del de cierre, evaluar desde ahí.
                            ini = ini2;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // En Java, substring, es desde inicio inclusive hasta fin exclusive.
                    // En .NET es desde inicio con la cantidad de caracteres del segundo parámetro.
                    //var exp = expression.substring(ini + 1, fin);
                    //var exp = expression.Substring(ini + 1, fin - ini);
                    var exp = expression[(ini + 1).. fin];
                    // Evaluar el resultado de la expresión.
                    double res = evaluarExp(exp);
                    // Asignar el resultado a la expresión.
                    //  Si hay varias expresiones (entre paréntesis) como la evaluada,
                    //      se reemplazarán por el resultado.
                    //
                    // Esto es seguro, ya que al estar entre paréntesis
                    //  las mismas expresiones tendrán los mismos resultados,
                    //  a diferencia de lo que ocurriría si no estuvieran entre paréntesis.
                    expression = expression.Replace("(" + exp + ")", res.ToString());
                }
            }

            // Aquí llegará se haya evaluado o no la expresión entre paréntesis.
            // Si había alguna expresión entre paréntesis, se habrá evaluado, pero puede que haya más.

            // Para no repetir la comprobación en caso de que no haya más paréntesis. (17/nov/22 14.10)
            //      Nota: Esta optimización no es estrictamente necesaria, pero...
            // Ya que, en el primer if se comprueba como mínimo si hay de apertura.
            // Si lo hubiera, después se revisará si hay de cierre.
            // Si no se cumplen los dos casos,
            //  en el if del bloque else, como mínimo, se vuelve a evaluar si hay de apertura.
            // Si no hay de apertura el primer if fallará y en el segundo solo se chequeará si hay de cierre.
            boolean hayApertura = expression.IndexOf('(') > -1;

            // Si no hay más paréntesis, salir.
            // Por seguridad, comprobar que estén los dos paréntesis.
            // Si hay de apertura y cierre, continuar.
            if (hayApertura && expression.IndexOf(')') > -1)
            {
                hay = true;
            }
            else
            {
                // Quitar los que hubiera (si no están emparejados).
                if (hayApertura || expression.IndexOf(')') > -1)
                {
                    expression = expression.Replace("(", "").Replace(")", "");
                }
                hay = false;
            }

            // Repetir si hay más expresiones entre paréntesis de apertura y cierre.
            //  Si hay paréntesis y no están emparejados, no se comprueba nada más.
        } while (hay);

        return expression;
    }

    /**
     * Evalúa la expresión indicada quitando los espacios en blanco, (no hay expresiones entre paréntesis).
     * Se evalúan las operaciones (entre dobles) de suma, resta, multiplicación y división.
     *
     * @param expression La expresión a evaluar.
     * @return Un valor doble con el resultado de la expresión evaluada.
     */
    private static double evaluarExp(String expression)
    {
        // Quitar todos los caracteres en blanco.
        expression = expression.Replace(" ", "");
        // Si la expresión es una cadena vacía, se devuelve cero.
        if (expression.Equals(""))
        {
            return 0;
        }

        int cuantos;
        String op2;
        // Para que tenga un valor asignado,
        //  antes del return resultado del final ya estará asignada correctamente.
        double resultado = -1;
        TuplePair<Character, Integer> donde;
        int desde;
        double res1, res2;

        while (true)
        {
            // Comprobar si se ha indicado el signo de factorial ! (23/nov/22 14.18)
            desde = expression.IndexOf('!');
            if (desde == -1)
            {
                break;
            }
            //op2 = expression.substring(0, desde);
            op2 = expression[0..desde];
            op2 = buscarUnNumero(op2, true);
            res1 = Double.Parse(op2);
            res2 = Fact(res1);
            expression = expression.Replace(op2 + "!", res2.ToString());
        }

        // Para asegurarme que no use op1 antes de este bucle while.
        String op1 = null;

        while (true)
        {
            desde = 0;
            // Buscar la operación a realizar.
            donde = siguienteOperadorConPrecedencia(expression, desde);
            // Si no hay más operadores.
            if (donde == null)
            {
                // Si no hay operadores y el resultado no se ha procesado, devolver el valor de la expresión.
                //
                // Este caso se me ha dado al evaluar una expresión entre paréntesis sin operadores.
                // Esto normalmente se dará si toda la expresión estaba entre paréntesis,
                //  se ha evaluado y la cadena contiene el resultado,
                //  pero la variable 'resultado' aún no se ha calculado.
                /* NOTA:
                 * Comprobar si op1 es null, no comprobar, por ejemplo, si resultado es -1,
                 * ya que puede haber un resultado que sea -1.
                 */
                if (op1 == null)
                {
                    return Double.Parse(expression);// Double.parseDouble(expression);
                }
                // Si llega aquí es que 'resultado' tiene un valor asignado
                //  con el resultado final de la expresión evaluada,
                //  por tanto, salir del while (con break) o devolver el resultado (con return resultado).
                // Así queda más claro que se sale de la función devolviendo el resultado.
                return resultado;
            }

            // Si la posición es cero es que delante no hay nada.
            // O es un número negativo. (18/nov/22 16.27)
            // O hay una expresión a evaluar. (20/nov/22 09.23)
            if (donde.position == 0)
            {
                if (donde.operador == '-')
                {
                    // Comprobar si hay más operaciones. (20/nov/22 09.23)
                    cuantos = cuantosOperadores(expression);
                    if (cuantos == 1)
                    {
                        return Double.Parse(expression);
                    }
                    // Desglosar la operación, teniendo en cuenta que el primero es negativo.
                    // Buscar el siguiente operador desde donde.position + 1.
                    desde = donde.position + 1;
                    var donde2 = siguienteOperadorConPrecedencia(expression, desde);
                    if (donde2 == null || donde2.position == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No puedo evaluar '{0}'.", expression);
                        Console.ForegroundColor = ConsoleColor.White;
                        // No lanzar una excepción, devolver -1.
                        return -1;
                    }
                    // Poner nuevamente el punto desde donde se analizó.
                    desde = donde.position;
                    // Reasignar la posición.
                    donde = donde2;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("La posición del operador es cero.");
                    Console.ForegroundColor = ConsoleColor.White;
                    // No lanzar una excepción, devolver -1.
                    return -1;
                }
            }

            // Asignar todos los caracteres hasta el signo al primer operador.
            //op1 = expression.substring(desde, donde.position).trim();
            //op1 = expression.Substring(desde, donde.position - desde).Trim();
            op1 = expression[desde..donde.position].Trim();
            // La variable op1 puede tener la expresión 16.5--20.0 y al convertirla a doble falla.
            // Ahora en buscarUnNumero se comprueba si la expresión tiene un número negativo.
            op1 = buscarUnNumero(op1, true);
            res1 = Double.Parse(op1);

            // op2 tendrá el resto de la expresión.
            //op2 = expression.substring(donde.position + 1).trim();
            op2 = expression.Substring(donde.position + 1).Trim();
            //op2 = expression[(donde.position + 1)..].Trim();
            // Buscar el número hasta el siguiente operador.
            op2 = buscarUnNumero(op2, false);
            res2 = Double.Parse(op2);

            // Hacer el cálculo de la operación
            resultado = donde.operador switch
            {
                '+' => res1 + res2,
                '-' => res1 - res2,
                '*' or 'x' => res1 * res2,
                '/' or ':' => res1 / res2,
                '%' => res1 % res2,
                _ => 0
            };
            var laOperacion = op1 + donde.operador + op2;
            var elResultado = resultado.ToString();

            // Si se deben mostrar las operaciones parciales. (18/nov/22 15.08)
            if (mostrarParciales)
            {
                // Mostrar los valores parciales en otro color.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\t{0} = {1:.##}", laOperacion, resultado);
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Cambiar por el resultado esta expresión. (18/nov/22 00.20)

            // La posición donde está esta operación (si hay más de una solo se busca la primera).
            var posOp = expression.IndexOf(laOperacion);

            // Si no se encuentra la operación es porque se ha podido quitar un operador.
            if (posOp == -1)
            {
                var laOp2 = op1 + donde.operador + donde.operador + op2;
                posOp = expression.IndexOf(laOp2);
                if (posOp == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\tError no se ha encontrado {0} (ni {1}) en la expresión.", laOperacion, laOp2);
                    Console.ForegroundColor = ConsoleColor.White;
                    return resultado;
                }
                else
                {
                    expression = expression.Replace(laOp2, elResultado);
                    continue;
                }
            }
            // Si está al principio de la cadena asignar el resultado más lo que haya tras la operación.
            if (posOp == 0)
            {
                //expression = elResultado + expression.substring(laOperacion.length());
                expression = elResultado + expression.Substring(laOperacion.Length);
            }
            // Si no está al principio,
            //  añadir lo que hubiera antes de esta operación, el resultado y lo que haya después de la operación.
            else
            {
                //expression = expression.substring(0,  posOp) + elResultado + expression.substring(posOp + laOperacion.length());
                expression = expression[..posOp] + elResultado + expression.Substring(posOp + laOperacion.Length);
            }
        }

        // Si no hay break en el bucle while, aquí no llegará nunca.
        //return resultado;
    }

    /**
     * Cuenta cuántos operadores hay en la expresión.
     *
     * @param expression La expresión a comprobar.
     * @return El número total de operadores en la expresión.
     */
    private static int cuantosOperadores(String expression)
    {
        int cuantos = 0;
        // Hacer una copia para que el original no se quede ordenado.
        char[] copiaOperadores = (char[])operadores.Clone(); //  Arrays.copyOf(operadores, operadores.length);
        Array.Sort(copiaOperadores);
        foreach (char c in expression.ToCharArray())
        {
            int p = Array.IndexOf(copiaOperadores, c); //  Arrays.binarySearch(copiaOperadores, c);
            if (p >= 0)
            {
                cuantos++;
            }
        }
        return cuantos;
    }

    /**
     * Comprueba si hay un solo operador, si lo hay devuelve un tuple con el carácter y la posición.
     *
     * @param expression La expresión a evaluar.
     * @return Si solo hay un operador, devuelve un tuple con el operador y la posición,
     *         en otro caso devuelve '\u0000' y -1.
     */
    private static TuplePair<Character, Integer> hayUnOperador(String expression)
    {
        int cuantos = 0;
        TuplePair<Character, Integer> res = new TuplePair<Character, Integer>('\u0000', -1);
        foreach (char op in operadores)
        {
            int pos = expression.IndexOf(op);
            if (pos > -1)
            {
                cuantos++;
                // Asignar solo el primero.
                if (cuantos == 1)
                {
                    res = new TuplePair<Character, Integer>(op, pos);
                    // Puede que este mismo operador está más de una vez.
                    if (pos + 1 < expression.Length)
                    {
                        int pos2 = expression.IndexOf(op, pos + 1);
                        if (pos2 > -1)
                        {
                            cuantos++;
                        }
                    }
                }
            }
        }
        if (cuantos != 1)
        {
            res = new TuplePair<Character, Integer>('\u0000', -1);
        }

        return res;
    }

    /**
     * Busca el siguiente signo de operación (teniendo en cuenta la precedencia: * / % + -).
     *
     * @param expression La expresión a evaluar.
     * @param fromIndex La posición desde la que se buscará en la cadena.
     * @return Una tuple con el operador hallado y la posición en la expresión o null si no se ha hallado.
     */
    private static TuplePair<Character, Integer> siguienteOperadorConPrecedencia(String expression, int fromIndex)
    {
        // Buscar primero los de más precedencia
        TuplePair<Character, Integer> posChar = firstIndexOfAny(expression, operadoresMultiplicativos.ToCharArray(), fromIndex);
        if (posChar != null)
        {
            return posChar;
        }
        // Después buscar en los de menos precedencia.
        posChar = firstIndexOfAny(expression, operadoresAditivos.ToCharArray(), fromIndex);
        return posChar;
    }

    /**
     * Busca el número anterior o siguiente.
     *
     * @param expression  La expresión a evaluar.
     * @param elAnterior True si se busca el número anterior (desde el final),
     *                   en otro caso se busca el número siguiente (desde el principio).
     * @return La cadena con el número hallado.
     *         Si el número hallado lo precede - y delante hay otro operador es que es un número negativo.
     */
    private static String buscarUnNumero(String expression, boolean elAnterior)
    {
        StringBuilder sb = new StringBuilder();
        var a = expression.ToCharArray();
        // Cuando se busca el anterior se hace desde el final,
        //  ya que la cadena tendrá un número precedido por un signo de operación o nada más.
        // Cuando se busca el siguiente, se hace desde el principio,
        //  porque la cadena tendrá el resto de la expresión a evaluar.
        int inicio = elAnterior ? a.Length - 1 : 0;
        int fin = elAnterior ? 0 : a.Length - 1;

        // Si la expresión solo contiene el operador - considerarlo como un número negativo.
        var unOp = hayUnOperador(expression);
        if (unOp.position > -1)
        {
            if (unOp.operador == '-')
            {
                // Solo si empieza con ese operador.
                if (expression[0] == unOp.operador)
                {
                    return expression;
                }
            }
        }
        // Si la expresión empieza por un operador, quitarlo. (18/nov/22 16.59)
        // Salvo si es el signo menos (-), ya que puede ser negativo.
        var ch = losOperadores.IndexOf(a[inicio]);
        if (ch > -1)
        {
            if (operadores[ch] == '-')
            {
                sb.Append(operadores[ch]);
            }
            if (elAnterior)
            {
                inicio--;
            }
            else
            {
                inicio++;
            }
        }

        int i = inicio;

        while (elAnterior ? i >= fin : i <= fin)
        {
            if (losOperadores.IndexOf(a[i]) == -1)
            {
                sb.Append(a[i]);
            }
            else
            {
                // Si es el signo menos...
                if (a[i] == '-')
                {
                    // Comprobar si a[i-1] es un operador.
                    if (i > 0)
                    {
                        if (losOperadores.IndexOf(a[i - 1]) > -1)
                        {
                            sb.Append(a[i]);
                            // Salir, porque es un número negativo.
                            break;
                        }
                    }
                    else
                    {
                        // Es el primer carácter y es un operador.
                        sb.Append(a[i]);
                    }
                }
                break;
            }
            if (elAnterior)
            {
                i--;
            }
            else
            {
                i++;
            }
        }
        // Si se ha encontrado algo y se busca el número anterior,
        //  invertirlo ya que se habrá añadido desde el final.
        if (elAnterior && sb.Length > 1)
        {
            //sb.reverse();
            var sb2 = new StringBuilder();
            for(i = sb.Length -1; i >= 0; i--) {
                sb2.Append(sb[i]);
            }
            sb = sb2;
        }
        return sb.ToString().Trim();
    }

    /**
     * Busca en la cadena los caracteres indicados y devuelve la primera ocurrencia.
     * Si alguno de los caracteres está en la cadena, devuelve el que esté antes.
     *
     * @param expression La cadena a evaluar.
     * @param anyOf Los caracteres a comprobar en la cadena.
     * @return La posición y el carácter del primero que encuentre en la cadena o un valor null si no hay ninguno.
     */
    private static TuplePair<Character, Integer> firstIndexOfAny(String expression, char[] anyOf, int fromIndex)
    {
        TuplePair<Character, Integer> menor = null;
        foreach (char c in anyOf)
        {
            int pos = expression.IndexOf(c, fromIndex);
            if (pos > -1)
            {
                if (menor == null)
                {
                    menor = new TuplePair<Character, Integer>(c, pos);
                }
                else if (menor.position > pos)
                {
                    menor = new TuplePair<Character, Integer>(c, pos);
                }
            }
        }
        return menor;
    }

    /**
     * Tuple de dos valores para usar al buscar un operador y la posición del mismo.
     *
     * @param operador Un valor del tipo T1.
     * @param position Un valor del tipo T2.
     * @param <T1> El tipo (por referencia) del primer parámetro.
     * @param <T2> El tipo (por referencia) del segundo parámetro.
     */
    record TuplePair<T1, T2>(T1 operador, T2 position)
    {
    }

    //
    // Estos no los utilizo en analizar las expresiones, solo en el método Main.
    //

    #region No usados en evaluar expresiones

    // Buscar en una cadena cualquiera de los caracteres indicados. (19/nov/22 03.58)

    /**
     * Busca en la cadena cualquiera de los caracteres indicados.
     *
     * @param expression La cadena a evaluar.
     * @param anyOf Los caracteres a comprobar en la cadena.
     * @return La posición y el carácter del primer carácter que encuentre o null si no hay ninguno.
     */
    private static TuplePair<Character, Integer> indexOfAny(String expression, char[] anyOf)
    {
        foreach (char c in anyOf)
        {
            int pos = expression.IndexOf(c);
            if (pos > -1)
            {
                return new TuplePair<Character, Integer>(c, pos);
            }
        }
        return null;
    }

    /**
    * Busca desde el principio de la cadena cualquiera de los caracteres de anyOf
    * y devuelve el que esté más cerca del final.
    *
    * @param expression La cadena a evaluar.
    * @param anyOf Los caracteres a comprobar en la cadena.
    * @return El carácter y la posición del último carácter que encuentre en la cadena o nulo si no hay ninguno.
    */
    static TuplePair<Character, Integer> lastIndexOfAny(String expression, char[] anyOf)
    {
        return lastIndexOfAny(expression, anyOf, 0);
    }

    /**
     * Busca desde la posición indicada de la cadena cualquiera de los caracteres de ofAny
     * y devuelve el que esté más cerca del final.
     *
     * @param expression La cadena a evaluar.
     * @param anyOf Los caracteres a comprobar en la cadena.
     * @param fromIndex El índice desde donde se empezará a buscar.
     * @return El carácter y la posición del último carácter que encuentre en la cadena o nulo si no hay ninguno.
     */
    static TuplePair<Character, Integer> lastIndexOfAny(String expression, char[] anyOf, int fromIndex)
    {
        // Recorrer la cadena desde el final.
        for (int i = expression.Length - 1; i >= fromIndex; i--)
        {
            // Comprueba si el carácter examinado está en anyOf.
            foreach (char c in anyOf)
            {
                // Si el carácter examinado es uno de los caracteres, devolver la posición.
                if (c == expression[i])
                {
                    return new TuplePair<Character, Integer>(c, i);
                }
            }
        }
        return null;
    }

    #endregion

    internal static void Main(string[] args)
    {
        String hola;
        String anyOf;
        Evaluar.TuplePair<Character, Integer> pos;
        String esta;
        String res;
        String vocales = "aeiou";
        String expression;
        double resD;
        double pruebaD;

        pruebaD = 30;
        var pruebaBI = new System.Numerics.BigInteger(pruebaD);
        var resBI = Fact(pruebaBI);
        Console.WriteLine("La factorialB de {0} es {1}", pruebaBI, resBI);
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = -3;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = 3;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = 17;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = 18;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaBI = new System.Numerics.BigInteger(pruebaD);
        resBI = Fact(pruebaBI);
        Console.WriteLine("La factorialB de {0} es {1}", pruebaBI, resBI);
        pruebaD = 19;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaBI = new System.Numerics.BigInteger(pruebaD);
        resBI = Fact(pruebaBI);
        Console.WriteLine("La factorialB de {0} es {1}", pruebaBI, resBI);
        pruebaD = 0.8;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = -0.8;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = 1.44;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        pruebaD = -1.44;
        resD = Fact(pruebaD);
        Console.WriteLine("La factorial  de {0} es {1}", pruebaD, resD);
        //return;

        Console.Write("Indica las letras a comprobar (0 para no comprobar cadenas) [{0}]: ", vocales);

        res = Console.ReadLine();

        if (!res.Equals("0"))
        {
            if (res.Equals(""))
            {
                res = "aeiou";
            }
            anyOf = res;

            hola = "Hola Radiola y Ole!";
            Console.Write("Indica la palabra para comprobar si tiene alguna de las letras indicadas [{0}]: ", hola);
            res = Console.ReadLine();
            if (res.Equals(""))
            {
                res = hola;
            }
            hola = res;
            Console.WriteLine();

            pos = indexOfAny(hola, anyOf.ToCharArray());
            Console.WriteLine("Usando indexOfAny:");
            if (pos == null)
            {
                Console.WriteLine("En '{0}' no está ninguno de los caracteres de '{1}'.\n", hola, anyOf);
            }
            else
            {
                Console.WriteLine("En '{0}' el primero de los caracteres de '{1}' ('{3}') hallado está en la posición {2}.\n", hola, anyOf, pos.position, pos.operador);
            }
            Console.WriteLine();

            Console.WriteLine("Usando firstIndexOfAny:");
            pos = firstIndexOfAny(hola, anyOf.ToCharArray(), 0);
            if (pos == null)
            {
                Console.WriteLine("En '%s' no está ninguno de los caracteres de '%s'.\n", hola, anyOf);
            }
            else
            {
                Console.WriteLine("En '{0}' de los caracteres de '{1}', el primero ('{3}') está en la posición {2}.\n", hola, anyOf, pos.position, pos.operador);
            }
            Console.WriteLine();

            Console.WriteLine("Usando lastIndexOfAny:");
            pos = lastIndexOfAny(hola, anyOf.ToCharArray());
            if (pos == null)
            {
                Console.WriteLine("En '{0}' no está ninguno de los caracteres de '{1}'.\n", hola, anyOf);
            }
            else
            {
                Console.WriteLine("En '{0}' de los caracteres de '{1}', el último ('{3}') está en la posición {2}.\n", hola, anyOf, pos.position, pos.operador);
            }
            Console.WriteLine();

            int veces = cuantasVeces(hola, 'a');
            Console.WriteLine("En '{0}' el carácter 'a' está {1} veces.\n", hola, veces);

        }

        expression = "25+(2(3+2)!*2)";
        expression = "(" + expression + ")";
        Console.Write("Escribe una expresión a evaluar (0 para mostrar las pruebas) [{0}] ", expression);
        res = Console.ReadLine();
        if (!res.Equals("0"))
        {
            if (!res.Equals(""))
            {
                expression = res;
            }
            if (expression.Equals("1.5*3.0+12-(-15+5)*2 + 10%3"))
            {
                pruebaD = 1.5 * 3.0 + 12 - (-15 + 5) * 2 + 10 % 3;
                Console.Write("Con Java: {0} = {1}\n", expression, pruebaD);
            }
            mostrarParciales = true;
            resD = Evaluar.evaluar(expression);

            // Mostrar 4 decimales (sin separador de miles).
            Console.Write("Con Evaluar: {0} = {1:#.####}", expression, resD);
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Pruebas operaciones (C# y Evaluar):");
            expression = "25+(2*(7*2)+2)";
            pruebaD = 25 + (2 * (7 * 2) + 2);
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            expression = "25+(2(7*2)+2)";
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);

            expression = "1.5*3+12-(-15+5)*2 + 10%3";
            pruebaD = 1.5 * 3 + 12 - (-15 + 5) * 2 + 10 % 3;
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);

            expression = "17 * ((12+5) * (7-2)) ";
            pruebaD = 17 * ((12 + 5) * (7 - 2));
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);

            expression = "1+2*3+6";
            pruebaD = 1 + 2 * 3 + 6;
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);
            expression = "99-15+2*7";
            pruebaD = 99 - 15 + 2 * 7;
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);

            // 6^2 / 2(3) + 4 (6 ^ 2 es 6 OR 2)
            pruebaD = 36.0 / 2 * (3) + 4;
            expression = "36.0 / 2*(3) +4";
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);
            // 6/2(2+1)
            pruebaD = 6.0 / 2 * (2 + 1);
            expression = "6.0/2*(2+1)";
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);
            //mostrarParciales = true;
            //6/(2(2+1))
            pruebaD = 6.0 / (2 * (2 + 1));
            expression = "6.0/(2*(2+1))";
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);
            // 2 – (10 x 2) / 6
            pruebaD = 2 - (10.0 * 2) / 6;
            expression = "2 - (10.0 * 2) / 6";
            Console.Write("C# dice: {0} = {1}\n", expression, pruebaD);
            Console.Write("Evaluar dice: {0} = ", expression);
            resD = Evaluar.evaluar(expression);
            Console.WriteLine(resD);
        }
    }

    /// <summary>
    /// Muestra la factorial del número indicado (solo para números enteros positivos).
    /// </summary>
    /// <param name="number">El número para saber la factorial.</param>
    /// <returns>Un valor de tipo BigInteger con la factorial.</returns>
    public static System.Numerics.BigInteger Fact(System.Numerics.BigInteger number)
    {
        if (number <= 1) return 1;
        if (number == 2) return number;
        return number * Fact(number - 1);
    }

    // El valor más alto sin mostrar notación exponencial es 18

    /// <summary>
    /// Muestra la factorial del número indicado para todos los números salvo los enteros negativos.
    /// </summary>
    /// <param name="number">El número para saber la factorial.</param>
    /// <returns>Un valor de tipo double con la factorial.</returns>
    /// <remarks>El valor más alto sin mostrar notación exponencial es 18</remarks>
    public static double Fact(double number)
    {
        if (number == 0) return 1;
        if (number == 2) return number;
        //return a * Fact(a - 1);

        // Comprobar si el número es un entero negativo sin decimales.
        if (number < 0)
        {
            double numAbs = Math.Abs(number);
            double fPart = numAbs - (long)numAbs;
            if (fPart == 0)
            {
                Console.WriteLine("\tError en factorial de {0} (entero negativo), se asigna 1.", number);
                return 1;
            }
        }

        // Si no es un número natural (o menor de cero) usar la función gamma.
        if (number % 1 != 0 || number < 0)
        {
            return gamma(number + 1);
        }

        // Calcularlo multiplicando el número hasta el 2.
        //for (double i = number - 1; i > 1; --i)
        for (double i = number - 1; i > 1; i--)
        {
            number *= i;
        }
        return number;
    }

    static double gamma(double z)
    {
        var g = 7;
        double[] C = { 0.99999999999980993, 676.5203681218851, -1259.1392167224028, 771.32342877765313, -176.61502916214059, 12.507343278686905, -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7 };

        if (z < 0.5) return Math.PI / (Math.Sin(Math.PI * z) * gamma(1 - z));
        else
        {
            z -= 1;

            var x = C[0];
            for (var i = 1; i < g + 2; i++)
                x += C[i] / (z + i);

            var t = z + g + 0.5;
            return Math.Sqrt(2 * Math.PI) * Math.Pow(t, (z + 0.5)) * Math.Exp(-t) * x;
        }
    }

    //function gamma(n)
    //{  // accurate to about 15 decimal places
    //   //some magic constants 
    //    var g = 7, // g represents the precision desired, p is the values of p[i] to plug into Lanczos' formula
    //        p = [0.99999999999980993, 676.5203681218851, -1259.1392167224028, 771.32342877765313, -176.61502916214059, 12.507343278686905, -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7];
    //    if (n < 0.5)
    //    {
    //        return Math.PI / Math.sin(n * Math.PI) / gamma(1 - n);
    //    }
    //    else
    //    {
    //        n--;
    //        var x = p[0];
    //        for (var i = 1; i < g + 2; i++)
    //        {
    //            x += p[i] / (n + i);
    //        }
    //        var t = n + g + 0.5;
    //        return Math.sqrt(2 * Math.PI) * Math.pow(t, (n + 0.5)) * Math.exp(-t) * x;
    //    }
    //}
    //function factorial(n)
    //{
    //    return gamma(n + 1);
    //}

}
