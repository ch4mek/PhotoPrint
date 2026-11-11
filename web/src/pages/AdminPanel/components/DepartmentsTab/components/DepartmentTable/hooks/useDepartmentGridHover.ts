import { useCallback, useTransition, MouseEvent, useRef } from 'react';

import {
	DEPARTMENT_SUBROW_HOVERED_CLASS_NAME,
	DEPARTMENT_SUBROW_CLASS_NAME,
	GROUP_SUBROW_HOVERED_CLASS_NAME,
	GROUP_SUBROW_CLASS_NAME,
	GROUP_SUBROW_LAST_CELL_CLASS_NAME,
} from '../constants';

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

export const useDepartmentGridHover = <T extends HTMLElement>() => {
	const targetRef = useRef<T>(null);
	const prevHoveredSubRowsRef = useRef<Element[] | null>(null);

	const [, startTransition] = useTransition();

	const removePrevHoveredSubRowsHover = () => {
		const prevHoveredSubRows = prevHoveredSubRowsRef.current;

		prevHoveredSubRows?.forEach((elem) => {
			elem.classList.remove(DEPARTMENT_SUBROW_HOVERED_CLASS_NAME);
			elem.classList.remove(GROUP_SUBROW_HOVERED_CLASS_NAME);
			elem.classList.remove(GROUP_SUBROW_LAST_CELL_CLASS_NAME);
		});

		prevHoveredSubRowsRef.current = null;
	};

	const handleSubRowHover = (subrowHoverIndex: string | null) => {
		if (targetRef.current) {
			removePrevHoveredSubRowsHover();

			const departmentRowItems = Array.from(
				selectSubRows(targetRef.current, subrowHoverIndex)
			);

			departmentRowItems.forEach((elem, index) => {
				const elementClassList = elem.classList;

				if (elementClassList.contains(DEPARTMENT_SUBROW_CLASS_NAME)) {
					elementClassList.add(DEPARTMENT_SUBROW_HOVERED_CLASS_NAME);
				}

				if (elementClassList.contains(GROUP_SUBROW_CLASS_NAME)) {
					elementClassList.add(GROUP_SUBROW_HOVERED_CLASS_NAME);

					if (index === departmentRowItems.length - 1) {
						elementClassList.add(GROUP_SUBROW_LAST_CELL_CLASS_NAME);
					}
				}
			});

			prevHoveredSubRowsRef.current = departmentRowItems;
		}

		return subrowHoverIndex ?? '';
	};

	const handleMouseOver = useCallback((event: MouseEvent) => {
		startTransition(() => {
			const target = event.target as HTMLElement;

			const hoverProjectRowId = getAttributeValue(
				target,
				DATA_SUBROW_ATTRIBUTE_NAME
			);

			handleSubRowHover(hoverProjectRowId);
		});
	}, []);

	return {
		targetRef,
		handleMouseOver,
	};
};
