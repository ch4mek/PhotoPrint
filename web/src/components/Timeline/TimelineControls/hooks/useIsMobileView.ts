import { useLayoutEffect, useRef, useState } from 'react';

export const useIsMobileView = (breakpoint: number) => {
	const containerRef = useRef<HTMLDivElement | null>(null);
	const [isMobile, setIsMobile] = useState(false);

	useLayoutEffect(() => {
		if (containerRef.current) {
			const checkIsMobile = () => {
				if (containerRef.current) {
					setIsMobile(containerRef.current.offsetWidth < breakpoint);
				}
			};

			checkIsMobile();

			const resizeObserver = new ResizeObserver(() => {
				checkIsMobile();
			});

			resizeObserver.observe(containerRef.current);

			return () => {
				resizeObserver.disconnect();
			};
		}
	}, []);

	return {
		containerRef,
		isMobile,
	};
};
