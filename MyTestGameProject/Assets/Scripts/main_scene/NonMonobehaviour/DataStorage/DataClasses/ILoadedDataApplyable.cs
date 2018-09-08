using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ILoadedDataApplyable
{
    /// <summary>
    /// Применить загруженные данные, без потери подписок на события и т.п.
    /// </summary>
    void ApplyLoadedData(object data);
}
