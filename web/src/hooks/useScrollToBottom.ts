import { useEffect, useRef } from 'react';

const SCROLL_PAGINATION_OFFSET = 50;

export const useScrollToBottom = (
	element: HTMLElement | null | undefined,
	action: () => void,
	offset = SCROLL_PAGINATION_OFFSET,
	shouldCheckScrollOnChange = false
) => {
	const isFirstRenderRef = useRef(true);

	useEffect(() => {
		if (!element) {
			return;
		}

		if (shouldCheckScrollOnChange) {
			if (
				element.scrollTop >=
				element.scrollHeight - element.offsetHeight - offset
			) {
				action();
			}
			isFirstRenderRef.current = false;
		}

		const handleScroll = () => {
			if (
				element.scrollTop >=
				element.scrollHeight - element.offsetHeight - offset
			) {
				action();
			}
		};

		element.addEventListener('scroll', handleScroll);

		return () => element.removeEventListener('scroll', handleScroll);
	}, [element, action, offset]);
};
