import {
	GREEN_TAG_COLOR,
	GREY_TAG_COLOR,
	RED_TAG_COLOR,
	WHITE,
	YELLOW_TAG_COLOR,
} from '@/theme';
import { AbsenceRequestStatus } from '@/types/absences';

const { Approved, Rejected, Open, Canceled } = AbsenceRequestStatus;

export const getRequestStatusColor = (value: AbsenceRequestStatus) => {
	switch (value) {
		case Approved:
			return GREEN_TAG_COLOR;
		case Rejected:
			return RED_TAG_COLOR;
		case Open:
			return YELLOW_TAG_COLOR;
		case Canceled:
			return GREY_TAG_COLOR;
		default:
			return WHITE;
	}
};
