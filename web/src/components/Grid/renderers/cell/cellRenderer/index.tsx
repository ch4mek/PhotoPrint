import { Skeleton } from '@mui/material';
import { MouseEvent, useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

export const CellRenderer = <T extends GridCellDefaultProps>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	isCopyEnabled = false
) => {
	const info = row[key as keyof T] as string;
	const isLoading = row?.showSkeleton;

	const handleClick = useMemo(() => {
		if (isCopyEnabled) {
			return (e: MouseEvent<HTMLDivElement>) => {
				if (
					'textContent' in e.target &&
					typeof e.target.textContent === 'string'
				) {
					navigator.clipboard.writeText(e.target.textContent);
				}
			};
		}

		return undefined;
	}, [isCopyEnabled]);

	return isLoading ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellWrapper onClick={handleClick} disabled={row.disabled}>
			{info}
		</CellWrapper>
	);
};
