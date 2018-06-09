using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISavable
{
    /// <summary>
    /// Сохранить данные в хранилище
    /// </summary>
    void Save();
    
    /// <summary>
    /// Загрузить данные из хранилища
    /// </summary>
    void Load();
}
