import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useContext } from 'react';

import { NotificationsParameteresContext } from '../../context/NotificationsContext';
import { NavigationNotificationWrapper } from '../../styles';

import { Routes } from '@/constants/routes';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { TeamBoardTabsId } from '@/pages/TeamBoardUser/types';
import { DevelopmentPlanSentForReviewNotification } from '@/types/notifications';

interface SentForReviewDevelopmentPlanNotificationProps {
	item: DevelopmentPlanSentForReviewNotification;
}

export function SentForReviewDevelopmentPlanNotification({
	item: { id, Message, UserId, read },
}: SentForReviewDevelopmentPlanNotificationProps) {
	const { readNotification, handleNotificationsModalClose } = useContext(
		NotificationsParameteresContext
	);
	const navigate = useCustomNavigate();

	const handleClick = () => {
		if (!read) {
			readNotification(id);
		}

		handleNotificationsModalClose();
		navigate({
			to: `${Routes.teamBoard}`,
			userId: UserId,
			tabId: TeamBoardTabsId.DEVELOPMENT_PLAN,
		});
	};

	return (
		<NavigationNotificationWrapper onClick={handleClick}>
			<Typography variant="m-400">{Message}</Typography>
		</NavigationNotificationWrapper>
	);
}
