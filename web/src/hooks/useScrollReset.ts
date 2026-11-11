import { Action } from 'history';
import throttle from 'lodash/throttle';
import { useCallback, useEffect, useRef } from 'react';
import { useLocation, useNavigationType } from 'react-router-dom';

const THROTTLE_LIMIT = 500;

export const useScrollReset = () => {
	const { pathname } = useLocation();
	const navigationType = useNavigationType();

	const prevPathname = useRef(pathname);
	const containerRef = useRef<HTMLDivElement | null>(null);

	useEffect(() => {
		const isPathChanged = prevPathname.current !== pathname;
		prevPathname.current = pathname;

		if (!isPathChanged) {
			return;
		}

		const targetScroll =
			navigationType === Action.Pop
				? window.history.state?.scrollPosition ?? 0
				: 0;

		if (containerRef.current) {
			containerRef.current.scrollTo(0, targetScroll);
		} else {
			window.scrollTo(0, targetScroll);
		}
	}, [pathname, navigationType]);

	const handleScroll = useCallback(() => {
		const scrollTop = containerRef.current
			? containerRef.current.scrollTop
			: window.scrollY;

		window.history.replaceState(
			{ ...window.history.state, scrollPosition: scrollTop },
			''
		);
	}, []);

	const throttledOnScroll = useRef(
		throttle(handleScroll, THROTTLE_LIMIT)
	).current;

	useEffect(() => () => throttledOnScroll.cancel(), []);

	const scrollCallbackRef = useCallback(
		(node: HTMLDivElement | null) => {
			if (containerRef.current) {
				containerRef.current.removeEventListener('scroll', throttledOnScroll);
			}

			if (node) {
				node.scrollTo(0, 0);
				node.addEventListener('scroll', throttledOnScroll, { passive: true });
			}

			containerRef.current = node;
		},
		[throttledOnScroll]
	);

	return { scrollCallbackRef };
};
