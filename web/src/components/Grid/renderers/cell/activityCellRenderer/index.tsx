import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

export const logicalTextCellRenderer = (logicalTextOptions: [string, string]) =>
	function <T extends GridCellDefaultProps>({
		row,
		column: { key },
	}: RenderCellProps<T, unknown>) {
		const isActive = row[key as keyof T] as boolean;

		const { showSkeleton, disabled } = row;

		if (isActive === undefined) {
			return <CellWrapper disabled={disabled} />;
		}

		const [optionIfTrue, optionIfFalse] = logicalTextOptions;

		return showSkeleton ? (
			<Skeleton animation="wave" width="100%" />
		) : (
			<CellWrapper disabled={disabled}>
				{isActive ? optionIfTrue : optionIfFalse}
			</CellWrapper>
		);
	};
