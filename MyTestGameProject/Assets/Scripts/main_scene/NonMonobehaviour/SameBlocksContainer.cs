using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameBlocksContainer
{
    /// <summary>
    /// имя типа блока (его выходы )
    /// </summary>
    public string Name;

    /// <summary>
    /// вче блоки одного типа
    /// </summary>
    public List<Block> SameBlocks;

    /// <summary>
    /// блоки этого же типа, но с выходом
    /// в массиве будут блоки сгруппированы в списки по направлению выхода.
    /// </summary>
    public List<Block>[] ExitBlocks = new List<Block>[Enum.GetValues(typeof(MapBlock.Direction)).Length];
    
    public class Block
    {
        /// <summary>
        /// имя блока (его тип и номер)
        /// </summary>
        public string Name;

        /// <summary>
        /// сам блок
        /// </summary>
        public MapBlock block;
    }
}
