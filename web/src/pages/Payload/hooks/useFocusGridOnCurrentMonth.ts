import { useEffect, useRef, useState } from 'react';
import { DataGridHandle } from 'react-data-grid';

import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { CURRENT_YEAR } from '@/constants/date';
import { useResize } from '@/hooks/useResize';
import dayjs from '@/lib/dayjs';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export function useFocusGridOnCurrentMonth(
	selectedYear: number,
	columnsWidths: GridColumnWidthsConfiguration
) {
	const [gridElement, setGridElement] = useState<HTMLElement | null>(null);
	const isFirstRender = useRef(true);
	const [windowWidth] = useResize();

	const handleSetGridElement = (ref: DataGridHandle | null): void => {
		setGridElement(ref ? ref.element : null);
	};

	useEffect(() => {
		if (
			gridElement &&
			Number(selectedYear) === CURRENT_YEAR &&
			isFirstRender.current
		) {
			const currentMonth = dayjs().format('MMM').toLowerCase();
			const currentMonthIndex = Object.keys(columnsWidths).findIndex(
				(key) => key === currentMonth
			);
			const columnsWidthsValues = Object.values(columnsWidths).slice(
				0,
				currentMonthIndex
			);

			const scrollLeft = columnsWidthsValues.reduce<number>(
				(acc, configValue) => {
					const columnWidth = getColumnWidth(configValue).width;
					const newScrollWidth = acc + Number(columnWidth);

					return newScrollWidth;
				},
				0
			);

			const gridVisibleArea = gridElement.clientWidth;
			const divider = windowWidth > 1107 ? 4 : 2;

			gridElement.scroll({
				left: scrollLeft - gridVisibleArea + gridVisibleArea / divider,
				behavior: 'smooth',
			});

			isFirstRender.current = false;
		}
	}, [columnsWidths, selectedYear, gridElement]);

	return { gridElement, handleSetGridElement };
}
