export const checkIsTextCut = (
	element: HTMLElement,
	scrollOffsetSubstraction?: number
) =>
	element.offsetWidth < element.scrollWidth - (scrollOffsetSubstraction || 0);
