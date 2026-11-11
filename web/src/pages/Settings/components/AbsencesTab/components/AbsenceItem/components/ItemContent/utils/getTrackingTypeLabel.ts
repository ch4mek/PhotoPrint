import { START_OF_TRACKING_TYPE_LABEL } from '../constants';

export const getTrackingTypeLabel = (type?: string) => {
	if (!type) {
		return null;
	}

	return `${START_OF_TRACKING_TYPE_LABEL} ${type}`;
};
