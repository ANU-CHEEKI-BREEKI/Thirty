using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMergeable
{
    /// <summary>
    /// Разрешить конфликт сохранений.
    /// Тут нужно решить что куда сохранить
    /// </summary>
    void Merge(object data);
}
