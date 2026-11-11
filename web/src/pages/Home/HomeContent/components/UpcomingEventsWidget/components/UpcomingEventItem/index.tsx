import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { formatEventDate } from '../../utils/formatEventDate';
import { getEventConfig } from '../../utils/getEventConfig';
import { UpcomingEventTypeIcon } from '../UpcomingEventTypeIcon';

import { DateTag } from '@/components/DateTag';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { Tag } from '@/components/Tag';
import {
	PopUpSearchParams,
	USER_PROFILE_TAB_ID_PARAM,
} from '@/constants/searchParams';
import { getRequestStatusColor } from '@/helpers/getRequestStatusColor';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { UpcomingEventType } from '@/types/home';

import { eventTypeToTabIdMap, STATUS_TAG_WIDTH } from './constants';
import {
	EventItem,
	EventTitleTypography,
	UpcomingEventContent,
	UpcomingEventItemWrapper,
} from './styles';
import { UpcomingEventItemProps } from './types';

export function UpcomingEventItem({
	item,
	isDesktopView = true,
}: UpcomingEventItemProps) {
	const theme = useTheme();
	const { type, performanceReviewDateType, status, name, inquiryId } = item;

	const navigate = useCustomNavigate();
	const handleOpenInquiryPopUp = useOpenPopUpViaUrl(
		PopUpSearchParams.InquiryIdParam
	);

	const {
		icon,
		title = name,
		...styledConfig
	} = getEventConfig(theme, theme.mode, item);

	const eventDate = formatEventDate(item);

	const isAbsenceType = type === UpcomingEventType.Absence && item.inquiryType;
	const isPerformanceType = type === UpcomingEventType.PerformanceReview;

	const statusBackgroundColor =
		status && isAbsenceType ? getRequestStatusColor(status) : undefined;

	const handleEventItemClick = () => {
		if (inquiryId && isAbsenceType) {
			handleOpenInquiryPopUp(inquiryId.toString());

			return;
		}

		navigate({
			tabId: {
				key: USER_PROFILE_TAB_ID_PARAM,
				value: eventTypeToTabIdMap[type],
			},
		});
	};

	return (
		<UpcomingEventItemWrapper isDesktopView={isDesktopView}>
			<EventItem isDesktopView={isDesktopView} onClick={handleEventItemClick}>
				<UpcomingEventTypeIcon {...styledConfig}>{icon}</UpcomingEventTypeIcon>

				<UpcomingEventContent>
					<Tooltip position="bottom" text={title} useIfTextCut>
						<EventTitleTypography variant="m-700">{title}</EventTitleTypography>
					</Tooltip>
					{isPerformanceType ? (
						<DateTag
							date={eventDate}
							type={performanceReviewDateType}
							onClick={handleEventItemClick}
							width="fit-content"
						/>
					) : (
						<Typography variant="m-400">{eventDate}</Typography>
					)}
				</UpcomingEventContent>

				{isAbsenceType && status && (
					<Tag width={STATUS_TAG_WIDTH} backgroundColor={statusBackgroundColor}>
						{status}
					</Tag>
				)}
			</EventItem>
		</UpcomingEventItemWrapper>
	);
}
