import { useLayoutEffect, useRef, useState } from 'react';

export const useMaxHeightLock = <T extends HTMLElement>() => {
	const ref = useRef<T>(null);
	const [maxContentHeight, setMaxContentHeight] = useState(0);

	useLayoutEffect(() => {
		const updateMaxHeight = () => {
			if (ref.current) {
				const currentContentHeight = ref.current.clientHeight;

				if (currentContentHeight > maxContentHeight) {
					setMaxContentHeight(currentContentHeight);
				}
			}
		};

		const resizeObserver = new ResizeObserver(updateMaxHeight);

		if (ref.current) {
			resizeObserver.observe(ref.current);
		}

		return () => {
			resizeObserver.disconnect();
		};
	}, [maxContentHeight]);

	return { ref, maxHeightLock: maxContentHeight };
};
