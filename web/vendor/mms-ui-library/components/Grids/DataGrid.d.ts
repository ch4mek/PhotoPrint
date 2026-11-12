import React, { type Key, JSX } from 'react';
import { DataGridHandle } from 'react-data-grid';
import { DataGridProps } from './types';
export declare const DataGrid: <R, SR = unknown, K extends Key = Key>(props: DataGridProps<R, SR, K> & React.RefAttributes<DataGridHandle>) => JSX.Element;
