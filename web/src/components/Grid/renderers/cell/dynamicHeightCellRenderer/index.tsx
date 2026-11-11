import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { Skeleton } from '@mui/material';
import { useContext, useEffect, useRef } from 'react';
import { RenderCellProps } from 'react-data-grid';

import {
	DYNAMIC_CELL_CLASS,
	TOOLTIP_FALLBACK_PLACEMENTS,
} from '@/components/Grid/constants';
import { GridCellDefaultProps } from '@/components/Grid/renderers/types';
import { HeightsContext } from '@/context/CellHeightsContext';

import { DYNAMIC_CELL_VERTICAL_PADDINGS } from './constants';
import { DynamicCellWrapper } from './styles';

interface GridCellPropsWithHeight extends GridCellDefaultProps {
	height?: number;
}

export const DynamicHeightCellRenderer = <T extends GridCellPropsWithHeight>(
	{ row, column: { key, idx }, onRowChange }: RenderCellProps<T, unknown>,
	maxTextRowCount?: number,
	enableTooltip = true
) => {
	const info = row[key as keyof T] as string;

	const ref = useRef<HTMLDivElement | null>(null);
	const rowRef = useRef(row);
	const heightMap = useContext(HeightsContext);

	useEffect(() => {
		rowRef.current = row;
	}, [row]);

	useEffect(() => {
		if (!ref.current || row.showSkeleton) {
			return;
		}

		const resizeObserver = new ResizeObserver(() => {
			const newLocal =
				(ref.current?.offsetHeight || 0) + DYNAMIC_CELL_VERTICAL_PADDINGS;
			const height = Math.max(newLocal, DEFAULT_GRID_ROW_HEIGHT);
			const columnsHeights = heightMap.get(rowRef.current.id as number) || {};
			const currentHeight = columnsHeights[idx] || 0;

			if (row.height && height === currentHeight) {
				return;
			}

			columnsHeights[idx] = height;
			heightMap.set(rowRef.current.id as number, columnsHeights);
			onRowChange({
				...rowRef.current,
			});
		});

		resizeObserver.observe(ref.current);

		return () => {
			resizeObserver.disconnect();
		};
	}, [row.showSkeleton, row.height]);

	if (row.showSkeleton) {
		return (
			<DynamicCellWrapper>
				<Skeleton animation="wave" width="100%" />
			</DynamicCellWrapper>
		);
	}

	if (enableTooltip) {
		return (
			<Tooltip
				useIfTextCut
				text={row.showCell === false ? '' : info}
				textCutOrientation="vertical"
				position="bottom"
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				ref={ref}
			>
				<DynamicCellWrapper
					maxTextRowCount={maxTextRowCount}
					className={DYNAMIC_CELL_CLASS}
				>
					{row.showCell === false ? '' : info}
				</DynamicCellWrapper>
			</Tooltip>
		);
	}

	return (
		<DynamicCellWrapper
			className={DYNAMIC_CELL_CLASS}
			{...{ ref, maxTextRowCount }}
		>
			{row.showCell === false ? '' : info}
		</DynamicCellWrapper>
	);
};
