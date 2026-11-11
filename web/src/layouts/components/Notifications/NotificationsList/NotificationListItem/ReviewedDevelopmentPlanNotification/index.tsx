import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { CheckCircleIcon } from '@mms/mms-ui-library/icons/base/CheckCircleIcon';
import { NotificationCrossCircleIcon } from '@mms/mms-ui-library/icons/base/NotificationCrossCircleIcon';
import { useContext } from 'react';

import { NotificationsParameteresContext } from '../../context/NotificationsContext';

import { UserProfileTabsId } from '@/components/UserProfile/types';
import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';
import { DevelopmentPlanReviewedNotification } from '@/types/notifications';

import { DevelopmentPlanReviewedNotificationWrapper } from './styles';

interface ReviewedDevelopmentPlanNotificationProps {
	item: DevelopmentPlanReviewedNotification;
}

export function ReviewedDevelopmentPlanNotification({
	item: { id, read, Message, Status },
}: ReviewedDevelopmentPlanNotificationProps) {
	const { readNotification, handleNotificationsModalClose } = useContext(
		NotificationsParameteresContext
	);
	const theme = useTheme();
	const navigate = useCustomNavigate();

	const handleClick = () => {
		if (!read) {
			readNotification(id);
		}

		handleNotificationsModalClose();
		navigate({
			tabId: {
				key: USER_PROFILE_TAB_ID_PARAM,
				value: UserProfileTabsId.DEVELOPMENT_PLAN,
			},
		});
	};

	return (
		<DevelopmentPlanReviewedNotificationWrapper onClick={handleClick}>
			{Status === DevelopmentPlanStatus.Approved ? (
				<CheckCircleIcon color={theme.palette['feedback-success']} />
			) : (
				<NotificationCrossCircleIcon color={theme.palette['feedback-danger']} />
			)}
			<Typography variant="m-400">{Message}</Typography>
		</DevelopmentPlanReviewedNotificationWrapper>
	);
}
