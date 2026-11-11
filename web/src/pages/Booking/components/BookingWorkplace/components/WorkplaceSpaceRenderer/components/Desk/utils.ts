import { DeskSize } from '@/types/booking';

import {
	LARGE_DESK_HEIGHT,
	LARGE_DESK_WIDTH,
	SMALL_DESK_HEIGHT,
	SMALL_DESK_WIDTH,
} from './constants';

const isHorizontalDesk = (rotationDegree: number) =>
	rotationDegree === 0 || rotationDegree === 180;

export const getDeskSizes = (size: DeskSize, rotationDegree: number) => {
	if (size === DeskSize.Small) {
		if (isHorizontalDesk(rotationDegree)) {
			return {
				height: SMALL_DESK_HEIGHT,
				width: SMALL_DESK_WIDTH,
			};
		}

		return {
			height: SMALL_DESK_WIDTH,
			width: SMALL_DESK_HEIGHT,
		};
	}

	if (isHorizontalDesk(rotationDegree)) {
		return {
			height: LARGE_DESK_HEIGHT,
			width: LARGE_DESK_WIDTH,
		};
	}

	return {
		height: LARGE_DESK_WIDTH,
		width: LARGE_DESK_HEIGHT,
	};
};
