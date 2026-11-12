/**
 *Function take the grid html element and removes certain column to 0px in the grid template
 * @param element DataGrid html element
 * @param columnNumber Number of the column that should be removed from grid template
 *
 * @returns New grid template style that could be used to override existing style
 */
export declare const removeColumnWidthFromGridTemplate: (columnNumber: number, element?: HTMLDivElement | null) => string;
