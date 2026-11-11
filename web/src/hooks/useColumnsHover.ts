import { useState, useTransition, useCallback } from 'react';

const DATA_ATTRIBUTE_NAME = 'hovercolindex';

export const useColumnsHover = () => {
	const [hoveredColumn, setHoveredColumn] = useState<number | null>(null);
	const [, startTransition] = useTransition();

	const handleMouseOver = useCallback((event: React.MouseEvent) => {
		startTransition(() => {
			const hoverColIndex = (event.target as HTMLElement)
				.closest(`[data-${DATA_ATTRIBUTE_NAME}]`)
				?.getAttribute(`data-${DATA_ATTRIBUTE_NAME}`);

			if (hoverColIndex) {
				setHoveredColumn(Number(hoverColIndex));
			}
		});
	}, []);

	const handleMouseLeave = useCallback(() => {
		setHoveredColumn(null);
	}, []);

	return {
		hoveredColumn,
		handleMouseOver,
		handleMouseLeave,
	};
};
