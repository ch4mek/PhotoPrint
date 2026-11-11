import { Skeleton } from '@mui/material';
import { MouseEvent, Dispatch, SetStateAction, useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

import { checkIsTextCut } from '@/helpers/checkIsTextCut';
import { TooltipContent } from '@/types/hooks';

import { CellContentWrapper } from './styles';

interface CellProps {
	cellMaxWidth?: string;
	cellHeight?: string;
}

export const CellWithTooltipAlternativeRenderer = <
	T extends GridCellDefaultProps
>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	setTooltipContent: Dispatch<SetStateAction<TooltipContent>>,
	isCopyEnabled = false,
	cellProps?: CellProps
) => {
	const info = row[key as keyof T] as string;
	const isLoading = row.showSkeleton;

	const { cellHeight, cellMaxWidth } = cellProps || {
		cellMaxWidth: 'auto',
		cellHeight: '100%',
	};

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

	const handleMouseOver = (e: MouseEvent<HTMLDivElement>) => {
		const firstElementChild = e.currentTarget
			.firstElementChild as HTMLDivElement;

		if (firstElementChild ?? checkIsTextCut(firstElementChild, 0)) {
			setTooltipContent({
				text: info,
				anchorElement: firstElementChild,
			});
		}
	};

	const handleMouseOut = () => {
		setTooltipContent(null);
	};

	return isLoading ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellContentWrapper
			onMouseOver={handleMouseOver}
			onMouseOut={handleMouseOut}
		>
			<CellWrapper
				onClick={handleClick}
				disabled={row.disabled}
				maxWidth={cellMaxWidth}
				height={cellHeight}
			>
				{row.showCell === false ? '' : info}
			</CellWrapper>
		</CellContentWrapper>
	);
};
