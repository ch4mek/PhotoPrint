import { TinelineScrollPosition } from '../types';

export const mapScrollPositionToScrollOffset = (
	position: TinelineScrollPosition,
	timelineWidth: number
) => {
	switch (position) {
		case 'center':
			return timelineWidth / 2;
		case 'start':
			return 0;
		case 'end':
			return timelineWidth;
		default:
			return timelineWidth / 2;
	}
};
