import { RefObject, useLayoutEffect, useState } from 'react';

import { Position } from '@/types/hooks';

export const usePosition = <T extends HTMLElement>(
	ref: RefObject<T | null>
): Position | null => {
	const [position, setPosition] = useState<Position | null>(null);

	const updatePosition = () => {
		if (ref.current) {
			const { top, left, right, bottom } = ref.current.getBoundingClientRect();
			setPosition({ top, left, right, bottom });
		}
	};

	useLayoutEffect(() => {
		const currentElement = ref.current;

		if (!currentElement) {
			return;
		}

		updatePosition();

		const observer = new ResizeObserver(updatePosition);
		observer.observe(currentElement);

		return () => {
			observer.disconnect();
		};
	}, []);

	return position;
};
