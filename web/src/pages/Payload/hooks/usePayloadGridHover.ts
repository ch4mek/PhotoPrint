import { useCallback, MouseEvent, useRef } from 'react';

import {
	PROJECT_SUBROW_CLASS_NAME,
	PROJECT_SUBROW_HOVERED_CLASS_NAME,
	WORKLOAD_SUBROW_CLASS_NAME,
	WORKLOAD_SUBROW_HOVERED_CLASS_NAME,
	WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME,
} from '../contants';

const DATA_COL_ATTRIBUTE_NAME = 'hovercolindex';
const DATA_SUBROW_ATTRIBUTE_NAME = 'subrowhoverindex';

const selectSubRows = <T extends HTMLElement>(
	target: T,
	value: string | null
) => target.querySelectorAll(`[data-${DATA_SUBROW_ATTRIBUTE_NAME}='${value}']`);

const getAttributeValue = (target: HTMLElement, dataAttributeName: string) => {
	const closestElement = target.closest(`[data-${dataAttributeName}]`);
	const attributeValue = closestElement
		? closestElement.getAttribute(`data-${dataAttributeName}`)
		: null;

	return attributeValue;
};

export const usePayloadGridHover = <T extends HTMLElement>() => {
	const targetRef = useRef<T>(null);
	const prevHoveredSubRowsRef = useRef<Element[] | null>(null);
	const prevHoverProjectRowIdRef = useRef('');

	const removePrevHoveredSubRowsHover = () => {
		const prevHoveredSubRows = prevHoveredSubRowsRef.current;

		prevHoveredSubRows?.forEach((elem) => {
			elem.classList.remove(PROJECT_SUBROW_HOVERED_CLASS_NAME);
			elem.classList.remove(WORKLOAD_SUBROW_HOVERED_CLASS_NAME);
			elem.classList.remove(WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME);
		});

		prevHoveredSubRowsRef.current = null;
		prevHoverProjectRowIdRef.current = '';
	};

	const resetHoverState = useCallback(() => {
		removePrevHoveredSubRowsHover();
	}, []);

	const handleSubRowHover = (subrowHoverIndex: string | null) => {
		if (
			prevHoverProjectRowIdRef.current !== subrowHoverIndex &&
			targetRef.current
		) {
			removePrevHoveredSubRowsHover();

			const projectRowItems = Array.from(
				selectSubRows(targetRef.current, subrowHoverIndex)
			);

			projectRowItems.forEach((elem, index) => {
				const elementClassList = elem.classList;

				if (elementClassList.contains(PROJECT_SUBROW_CLASS_NAME)) {
					elementClassList.add(PROJECT_SUBROW_HOVERED_CLASS_NAME);
				}

				if (elementClassList.contains(WORKLOAD_SUBROW_CLASS_NAME)) {
					elementClassList.add(WORKLOAD_SUBROW_HOVERED_CLASS_NAME);

					if (index === projectRowItems.length - 1) {
						elementClassList.add(WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME);
					}
				}
			});

			prevHoveredSubRowsRef.current = projectRowItems;
		}
	};

	const handleMouseOver = useCallback(
		(event: MouseEvent) => {
			const target = event.target as HTMLElement;

			const hoverColIndex = getAttributeValue(target, DATA_COL_ATTRIBUTE_NAME);
			const hoverProjectRowId = getAttributeValue(
				target,
				DATA_SUBROW_ATTRIBUTE_NAME
			);

			if (!hoverColIndex && !hoverProjectRowId) {
				resetHoverState();

				return;
			}

			handleSubRowHover(hoverProjectRowId);
		},
		[resetHoverState]
	);

	return {
		targetRef,
		handleMouseOver,
		handleMouseLeave: resetHoverState,
	};
};
