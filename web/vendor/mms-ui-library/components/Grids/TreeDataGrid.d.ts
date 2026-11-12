import React, { type Key, JSX } from 'react';
import { DataGridHandle } from 'react-data-grid';
import { TreeDataGridProps } from './types';
export declare const TreeDataGrid: <R, SR = unknown, K extends Key = Key>(props: TreeDataGridProps<R, SR, K> & React.RefAttributes<DataGridHandle>) => JSX.Element;
