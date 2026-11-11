import { useEffect, useState } from 'react';

export const useAddScrollListener = <T extends HTMLElement>(
	autoSelectWrapperRef: React.MutableRefObject<T | null>
): [boolean, React.Dispatch<React.SetStateAction<boolean>>] => {
	const [isIntersecting, setIsIntersecting] = useState(false);

	useEffect(() => {
		if (isIntersecting && autoSelectWrapperRef.current) {
			const intersectionObserver = new IntersectionObserver(
				([{ intersectionRatio }]) => {
					if (intersectionRatio < 1) {
						setIsIntersecting(false);
					}
				},
				{
					threshold: [1],
				}
			);

			intersectionObserver.observe(autoSelectWrapperRef.current);

			return () => {
				intersectionObserver.disconnect();
			};
		}
	}, [isIntersecting, autoSelectWrapperRef]);

	return [isIntersecting, setIsIntersecting];
};
