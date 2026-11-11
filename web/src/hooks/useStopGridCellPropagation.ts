import { useCallback, MouseEvent, KeyboardEvent } from 'react';

export const useStopGridCellPropagation = () => {
	const handleClick = useCallback(
		(e: MouseEvent<HTMLInputElement>) => e.stopPropagation(),
		[]
	);

	const handleKeyDown = useCallback((e: KeyboardEvent<HTMLDivElement>) => {
		e.stopPropagation();
	}, []);

	return {
		handleClick,
		handleKeyDown,
	};
};
