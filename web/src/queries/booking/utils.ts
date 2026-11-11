import { RequestTypes } from '@/constants/booking';

export const getRequestType = (
	requestType: RequestTypes,
	userId: number | null | undefined
) => {
	if (requestType === RequestTypes.CreateForGuest) {
		return RequestTypes.CreateForGuest;
	}

	if (requestType === RequestTypes.CreateBlock) {
		return RequestTypes.CreateBlock;
	}

	if (userId) {
		return RequestTypes.CreateForUser;
	}

	return requestType;
};
