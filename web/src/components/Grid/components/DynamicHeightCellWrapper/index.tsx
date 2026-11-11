import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useContext, useEffect, useRef } from 'react';

import { DYNAMIC_CELL_VERTICAL_PADDINGS } from '@/components/Grid/components/DynamicHeightCellWrapper/constants';
import { DynamicCellWrapper } from '@/components/Grid/components/DynamicHeightCellWrapper/styles';
import {
	DynamicHeightWrapperProps,
	GridCellPropsWithHeight,
} from '@/components/Grid/components/DynamicHeightCellWrapper/types';
import { DYNAMIC_CELL_CLASS } from '@/components/Grid/constants';
import { HeightsContext } from '@/context/CellHeightsContext';

export function DynamicHeightWrapper<T extends GridCellPropsWithHeight>({
	row,
	idx,
	onRowChange,
	maxTextRowCount,
	tooltipText,
	tooltip = false,
	children,
}: DynamicHeightWrapperProps<T>) {
	const ref = useRef<HTMLDivElement | null>(null);
	const rowRef = useRef(row);
	const heightMap = useContext(HeightsContext);

	useEffect(() => {
		rowRef.current = row;
	}, [row]);

	useEffect(() => {
		if (!ref.current) {
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
			onRowChange({ ...rowRef.current });
		});

		resizeObserver.observe(ref.current);

		return () => resizeObserver.disconnect();
	}, [row.height, heightMap]);

	const content = (
		<DynamicCellWrapper
			ref={ref}
			className={DYNAMIC_CELL_CLASS}
			maxTextRowCount={maxTextRowCount}
		>
			{children}
		</DynamicCellWrapper>
	);

	if (tooltip && tooltipText) {
		return (
			<Tooltip
				useIfTextCut
				text={tooltipText}
				textCutOrientation="vertical"
				position="bottom"
			>
				{content}
			</Tooltip>
		);
	}

	return content;
}
