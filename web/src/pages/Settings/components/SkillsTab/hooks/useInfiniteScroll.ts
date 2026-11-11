import { useEffect, useRef } from 'react';

interface UseInfiniteScrollProps {
	hasNextPage?: boolean;
	isFetchingNextPage: boolean;
	fetchNextPage: () => void;
	enabled?: boolean;
}

export function useInfiniteScroll({
	hasNextPage,
	isFetchingNextPage,
	fetchNextPage,
	enabled = true,
}: UseInfiniteScrollProps) {
	const containerRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		if (!enabled || !containerRef.current || !hasNextPage) {
			return;
		}

		const element = containerRef.current;

		const resizeObserver = new ResizeObserver(() => {
			if (
				element.scrollHeight === element.clientHeight &&
				hasNextPage &&
				!isFetchingNextPage
			) {
				fetchNextPage();
			}
		});

		const handleScroll = () => {
			if (!hasNextPage || isFetchingNextPage) {
				return;
			}

			const { scrollTop, scrollHeight, clientHeight } = element;
			const isNearBottom = scrollTop + clientHeight >= scrollHeight - 100;

			if (isNearBottom) {
				fetchNextPage();
			}
		};

		resizeObserver.observe(element);
		element.addEventListener('scroll', handleScroll);

		return () => {
			resizeObserver.disconnect();
			element.removeEventListener('scroll', handleScroll);
		};
	}, [hasNextPage, isFetchingNextPage, fetchNextPage, enabled]);

	return containerRef;
}
