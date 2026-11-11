import { useState, useRef, useEffect } from 'react';

export const useResizeObserver = () => {
	const [height, setHeight] = useState(0);
	const ref = useRef<HTMLDivElement>(null);

	useEffect(() => {
		if (!ref.current) {
			return;
		}

		const observer = new ResizeObserver((entries) => {
			const entry = entries[0];
			const currentHeight =
				entry.borderBoxSize?.[0]?.blockSize || entry.contentRect.height;
			setHeight(currentHeight);
		});

		observer.observe(ref.current);

		return () => observer.disconnect();
	}, []);

	return { ref, height };
};
