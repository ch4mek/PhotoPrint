import { MutableRefObject } from 'react';
import { DropTargetMonitor } from 'react-dnd';

import { DropIndicatorPositionType, Option } from '@/pages/Wiki/types';
import { isSiblingNode } from '@/pages/Wiki/utils/isSiblingNode';

const { Top, None, Middle, Bottom } = DropIndicatorPositionType;

export const getDropIndicatorPosition = (
	monitor: DropTargetMonitor,
	node: Option,
	ref: MutableRefObject<HTMLDivElement | null>
) => {
	const item = monitor.getItem() as Option;

	if (!item) {
		return None;
	}

	const hoverBoundingRect = ref.current?.getBoundingClientRect();

	if (!hoverBoundingRect) {
		return None;
	}

	const hoverMiddleY = (hoverBoundingRect.bottom - hoverBoundingRect.top) / 2;
	const hoverTop = Math.floor(hoverBoundingRect.height / 3);
	const hoverMiddle = Math.floor((hoverBoundingRect.height / 3) * 2);
	const clientOffset = monitor.getClientOffset();

	if (!clientOffset) {
		return Top;
	}

	const hoverClientY = clientOffset.y - hoverBoundingRect.top;

	if (item.isGroup) {
		if (!isSiblingNode(item, node)) {
			return None;
		}

		return hoverClientY < hoverMiddleY ? Top : Bottom;
	}

	if (!node.parentGroupId) {
		return Middle;
	}

	if (node.items) {
		if (hoverClientY < hoverTop) {
			return Top;
		}

		if (hoverClientY > hoverTop && hoverClientY < hoverMiddle) {
			return Middle;
		}

		return Bottom;
	}

	return hoverClientY < hoverMiddleY ? Top : Bottom;
};
