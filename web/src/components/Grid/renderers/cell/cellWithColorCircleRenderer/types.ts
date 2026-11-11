import { MouseEvent } from 'react';

import { GridCellDefaultProps } from '../../types';

export interface CellWithColorCircleProps extends GridCellDefaultProps {
	color: string;
	isSmallCircle?: boolean;
	isSmallestCircle?: boolean;
}

export interface CellWithColorCircleRendererOptions<T> {
	onCellClick?: (
		row: T,
		columnKey: string,
		e: MouseEvent<HTMLDivElement>
	) => void;
	fieldToDisplay?: keyof T;
	enableCellClick?: (row: T) => boolean;
}
