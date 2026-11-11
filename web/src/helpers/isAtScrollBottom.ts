import { UIEvent } from 'react';

const SCROLL_PAGINATION_OFFSET = 50;

export function isAtScrollBottom({
	currentTarget,
}: UIEvent<HTMLDivElement>): boolean {
	return (
		currentTarget.scrollTop >=
		currentTarget.scrollHeight -
			currentTarget.offsetHeight -
			SCROLL_PAGINATION_OFFSET
	);
}

export function isAtScrollTop({
	currentTarget,
}: UIEvent<HTMLUListElement>): boolean {
	return currentTarget.scrollTop <= SCROLL_PAGINATION_OFFSET;
}
