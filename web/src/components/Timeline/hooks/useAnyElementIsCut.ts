import { RefObject, useState, useEffect } from 'react';

const checkIsTextCut = (
	element: HTMLElement,
	scrollOffsetSubstraction: number
) => element.offsetWidth < element.scrollWidth - scrollOffsetSubstraction;

export const useAnyElementIsCut = <T extends HTMLElement | null>(
	refs: RefObject<T>[]
) => {
	const [isCut, setCut] = useState(false);

	useEffect(() => {
		const isAnyElementCut = refs.some((ref) => {
			if (ref.current) {
				return checkIsTextCut(ref.current, 1);
			}

			return false;
		});

		setCut(isAnyElementCut);
	}, [refs]);

	return { isCut };
};
