import { useEffect, useRef, useState } from 'react';
import { DataGridHandle } from 'react-data-grid';

import { TIMESHEET_FREEZE_COLUMNS_BREAKPOINT } from '../constants';

import { useResize } from '@/hooks/useResize';

export const useDataGridOffsetWidth = () => {
	const [windowWidth] = useResize();

	const gridRef = useRef<DataGridHandle | null>(null);
	const [gridOffsetWidth, setGridOffsetWidth] = useState(windowWidth);

	const areColumnsFrozen =
		gridOffsetWidth >= TIMESHEET_FREEZE_COLUMNS_BREAKPOINT;

	useEffect(() => {
		if (!gridRef.current?.element) {
			return;
		}

		const resizeObserver = new ResizeObserver(() => {
			if (gridRef.current?.element) {
				setGridOffsetWidth(gridRef.current.element.offsetWidth || windowWidth);
			}
		});

		resizeObserver.observe(gridRef.current.element);

		return () => {
			resizeObserver.disconnect();
		};
	}, [gridRef.current]);

	return {
		gridRef,
		gridOffsetWidth,
		areColumnsFrozen,
	};
};
