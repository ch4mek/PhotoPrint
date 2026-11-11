import { useCallback, useMemo, useTransition } from 'react';

import { getVerticalEllipsisRowsCount } from '../utils/getVerticalEllipsisRowsCount';

import { DynamicRowsHeightProps } from '@/components/Grid/hooks/types';
import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';

export const useDynamicRowsHeight = <T extends DynamicHeightCellDefaultProps>({
	setRows,
	defaultRowHeight = DATA_GRID_ROW_HEIGHT_WITH_TAG,
	lineHeight,
}: DynamicRowsHeightProps<T>) => {
	const [, startTransition] = useTransition();

	const heightsMap = useMemo(
		() => new Map<number, { [key: number]: number }>(),
		[]
	);

	const getHeight = useCallback(
		(row: T) => row?.height || defaultRowHeight,
		[defaultRowHeight]
	);

	const handleUpdateRows = (
		updatedRows: Array<T>,
		{ indexes }: { indexes: number[] }
	) => {
		startTransition(() => {
			setRows((prevGridRows) => {
				const prevRowsCopy = [...prevGridRows];
				indexes.forEach((index) => {
					const rowId = updatedRows[index].id;
					const columnHeights =
						rowId !== undefined ? heightsMap.get(Number(rowId)) : undefined;

					prevRowsCopy[index] = updatedRows[index];
					const rowHeight = columnHeights
						? Math.max(...Object.values(columnHeights), defaultRowHeight!)
						: defaultRowHeight;
					prevRowsCopy[index].height = rowHeight;

					if (lineHeight) {
						prevRowsCopy[index].verticalEllipsisLineCount =
							getVerticalEllipsisRowsCount(rowHeight, lineHeight);
					}
				});

				return prevRowsCopy;
			});
		});
	};

	return {
		handleUpdateRows,
		getHeight,
		heightsMap,
	};
};
