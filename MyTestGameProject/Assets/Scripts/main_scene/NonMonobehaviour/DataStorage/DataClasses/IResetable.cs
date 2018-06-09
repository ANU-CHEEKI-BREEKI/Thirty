using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IResetable
{    
    /// <summary>
    /// Вызываем чтобы установить значения полей по умолчанию, но оставить подписки на события
    /// </summary>
    void Reset();
}
