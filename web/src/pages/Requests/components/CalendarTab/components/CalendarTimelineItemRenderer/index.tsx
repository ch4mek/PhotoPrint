import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { CALENDAR_ITEM_CLASSNAME } from '../AbsenceCalendarGrid/constants';

import { getAbsenceTypeFromValue } from '@/components/Grid/renderers/cell/absenceNameCellRenderer/utils/getAbsenceTypeFromValue';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { noTimelineIconStyles, timelineIconStyles } from '@/constants/absence';
import { PopUpSearchParams } from '@/constants/searchParams';
import { AbsenceRequestStatus } from '@/types/absences';

import { ABSENCE_ICON_SIZE } from './constants';
import {
	CalendarTimelineContent,
	CalendarTimelineItemContainer,
	CalendarTimelineTypography,
	CalendarTimelineWrapper,
	IconWrapper,
} from './styles';
import { CalendarAbsenceType, CalendarTimelineItemProps } from './types';
import { getCalendarTimelineItemWidth } from './utils/getCalendarTimelineItemWidth';
import { getTooltipText } from './utils/getTooltipText';

export function CalendarTimelineItem({
	cellWidth,
	level,
	inquiry: { inquiryId, inquiryType, inquiryName, inquiryStatus },
	startDate,
	endDate,
	totalApproversCount,
	alreadyApprovedCount,
	totalHours,
	isStartPart = false,
	isEndPart = false,
}: CalendarTimelineItemProps) {
	const theme = useTheme();
	const handleOpenPopUp = useOpenPopUpViaUrl(PopUpSearchParams.InquiryIdParam);

	const handleOpen = () => {
		handleOpenPopUp(inquiryId.toString());
	};

	const {
		icon: Icon,
		color,
		background,
	} = timelineIconStyles(theme)[
		getAbsenceTypeFromValue(inquiryType) as CalendarAbsenceType
	] || noTimelineIconStyles(theme);

	const isCalendarTimelineItemFilled =
		inquiryStatus !== AbsenceRequestStatus.Open;

	const tooltipText = getTooltipText({
		inquiry: { inquiryId, inquiryType, inquiryName, inquiryStatus },
		totalHours,
		alreadyApprovedCount,
		totalApproversCount,
	});

	const calendarTimelineItemWidth = getCalendarTimelineItemWidth(
		startDate,
		endDate,
		cellWidth
	);

	const containerStyles = isCalendarTimelineItemFilled
		? { background, borderColor: 'transparent' }
		: {
				background: theme.palette['background-white'],
				borderColor: color,
		  };

	return (
		<CalendarTimelineItemContainer
			{...{ level }}
			className={CALENDAR_ITEM_CLASSNAME}
			onClick={handleOpen}
		>
			<Tooltip text={tooltipText}>
				<CalendarTimelineWrapper
					{...containerStyles}
					width={calendarTimelineItemWidth}
					{...{ isStartPart, isEndPart }}
				>
					{!isEndPart && (
						<CalendarTimelineContent>
							<IconWrapper>
								<Icon
									color={color}
									width={ABSENCE_ICON_SIZE}
									height={ABSENCE_ICON_SIZE}
								/>
							</IconWrapper>

							<CalendarTimelineTypography color={color} variant="m-400">
								{inquiryName}
							</CalendarTimelineTypography>
						</CalendarTimelineContent>
					)}
				</CalendarTimelineWrapper>
			</Tooltip>
		</CalendarTimelineItemContainer>
	);
}
