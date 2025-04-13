using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MockExams.Helper;

public class PhoneHelper
{
    public static string FormatPhoneNumber(string phoneNumber)
    {
        // Remover todos os caracteres não numéricos do número de telefone
        string phoneFmt = Regex.Replace(phoneNumber, "[^0-9]", "");

        // Verificar se os números contêm o DDI do Brasil (+55) e o código de área
        if (phoneFmt.Length >= 11 && phoneFmt.StartsWith("55"))
        {
            // Se o número já tiver o DDI do Brasil (+55), apenas retorne os números sem formatação
            return "+" + phoneFmt;
        }
        else
        {
            // Caso contrário, adicione o DDI do Brasil (+55) e retorne os números formatados
            return "+55" + phoneFmt;
        }
    }
}
