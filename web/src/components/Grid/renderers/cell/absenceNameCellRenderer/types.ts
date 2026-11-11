import { MouseEvent } from 'react';

import { GridCellDefaultProps } from '@/components/Grid';

export interface IconContainerProps {
	background: string;
	color: string;
}

export interface AbsenceNameCellProps extends GridCellDefaultProps {
	inquiryType: string;
}

export interface AbsenceNameCellRendererOptions<T> {
	onCellClick?: (
		row: T,
		columnKey: string,
		e: MouseEvent<HTMLDivElement>
	) => void;
	isClickable?: boolean;
}
