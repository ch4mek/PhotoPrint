import { Dispatch, SetStateAction } from 'react';

export interface DynamicRowsHeightProps<T> {
	setRows: Dispatch<SetStateAction<Array<T>>>;
	defaultRowHeight?: number;
	lineHeight?: number;
}
