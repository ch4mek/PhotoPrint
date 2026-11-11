import { ReactNode } from 'react';
import {
	Column,
	CalculatedColumn,
	RenderCellProps,
	RenderHeaderCellProps,
} from 'react-data-grid';

export interface ColumnWithHoverIndex<TRow, TSummaryRow = unknown>
	extends Omit<Column<TRow, TSummaryRow>, 'renderCell' | 'renderHeaderCell'> {
	hoverIndex: number;
	isMobileView?: boolean;
	renderCell?: (
		props: RenderCellPropsWithHoverIndex<TRow, TSummaryRow>
	) => ReactNode;
	renderHeaderCell?: (
		props: RenderHeaderCellPropsWithHoverIndex<TRow, TSummaryRow>
	) => ReactNode;
}

interface CalculatedColumnWithHoverIndex<TRow, TSummaryRow = unknown>
	extends CalculatedColumn<TRow, TSummaryRow> {
	hoverIndex: number;
	isMobileView?: boolean;
}

export interface RenderCellPropsWithHoverIndex<TRow, TSummaryRow = unknown>
	extends Omit<RenderCellProps<TRow, TSummaryRow>, 'column'> {
	column: CalculatedColumnWithHoverIndex<TRow, TSummaryRow>;
}

export interface RenderHeaderCellPropsWithHoverIndex<
	TRow,
	TSummaryRow = unknown
> extends Omit<RenderHeaderCellProps<TRow, TSummaryRow>, 'column'> {
	column: CalculatedColumnWithHoverIndex<TRow, TSummaryRow>;
}
