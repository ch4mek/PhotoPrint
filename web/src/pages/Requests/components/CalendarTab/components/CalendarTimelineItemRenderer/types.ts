import { AbsenceRequestStatus, AbsenceType } from '@/types/absences';

export interface CalendarTimelineItemProps {
	cellWidth: number;
	level: number;
	inquiry: {
		inquiryId: number;
		inquiryType: AbsenceType;
		inquiryName: string;
		inquiryStatus: AbsenceRequestStatus;
	};
	startDate: string;
	endDate: string;
	totalApproversCount?: number;
	alreadyApprovedCount?: number;
	totalHours?: number;
	isStartPart?: boolean;
	isEndPart?: boolean;
}

export type GetTooltipTextParams = Pick<
	CalendarTimelineItemProps,
	'totalHours' | 'inquiry' | 'totalApproversCount' | 'alreadyApprovedCount'
>;

export type CalendarAbsenceType = Exclude<AbsenceType, AbsenceType.Documents>;
