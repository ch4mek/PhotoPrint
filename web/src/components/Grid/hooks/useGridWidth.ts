import { useCallback, useEffect, useRef, useState } from 'react';
import { DataGridHandle } from 'react-data-grid';

import { useResize } from '@/hooks/useResize';

export const useDataGridOffsetWidth = () => {
	const [windowWidth] = useResize();
	const [gridOffsetWidth, setGridOffsetWidth] = useState<number>(0);
	const elementRef = useRef<HTMLElement>(null);

	const gridRef = useCallback((node: DataGridHandle | null) => {
		elementRef.current = node?.element || null;

		if (elementRef.current) {
			setGridOffsetWidth(elementRef.current.offsetWidth);
		}
	}, []);

	useEffect(() => {
		const element = elementRef.current;

		if (!element) {
			return;
		}

		const resizeObserver = new ResizeObserver(() => {
			setGridOffsetWidth(element.offsetWidth || windowWidth);
		});

		resizeObserver.observe(element);

		return () => {
			resizeObserver.disconnect();
		};
	}, []);

	return {
		gridRef,
		gridOffsetWidth,
	};
};
